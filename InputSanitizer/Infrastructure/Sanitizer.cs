using Ganss.Xss;
using InputSanitizer.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InputSanitizer
{
    internal class Sanitizer
    {
        public static object Sanitize(
            object input,
            bool isAllInputs,
            ModelStateDictionary modelState,
            string policyName = null,
            string lastPropertyName = null)
        {
            try
            {
                // if it is a string
                if (input is string)
                    return CleanString(lastPropertyName??"User Input", (string)input, policyName, modelState);
            }
            catch (ProhabitedStringValueException ex)
            {
                throw ex;
            }

            // if the class need to be ignored
            if (input.GetType().GetCustomAttributes<IgnoreSanitizedAttribute>(false).Any())
                return input;

            var classSanitizedAttr = input.GetType()
                .GetCustomAttributes<SanitizedAttribute>(false)
                .FirstOrDefault();
            var isClassSanitizing = classSanitizedAttr != null;

            // get properties of object type, check the cache first
            var type = input.GetType();
            var objectPropertiesCache = Shared._ObjectPropertiesCache;
            List<PropertyInfo> props;
            lock (objectPropertiesCache)
            {
                if (!Shared._ObjectPropertiesCache.TryGetValue(type, out props))
                {
                    props = new List<PropertyInfo>(
                        type.GetProperties()
                            .Where(s => s.CanWrite == true ||   // if the property cannot be re-write
                                s.PropertyType.IsValueType == false ||  // if the property is a value type
                                !s.GetCustomAttributes<IgnoreSanitizedAttribute>(false).Any())); // if the property need to be ignored
                    Shared._ObjectPropertiesCache.Add(type, props);
                }
            }

            foreach (var prop in props)
            {
                var propAttr = prop
                    .GetCustomAttributes<SanitizedAttribute>(false)
                    .FirstOrDefault();

                // if the property no sanitized attribute and no class sanitized attr
                //      when not using sanitize all input filter
                if (!isAllInputs &&
                        !isClassSanitizing &&
                        propAttr == null)
                    continue;

                // if the value is null
                try
                {
                    if (prop.GetValue(input) == null)
                        continue;
                }
                catch { }
                

                // get closest option 
                var selectedPolicyName = policyName;
                if (isClassSanitizing && !string.IsNullOrEmpty(classSanitizedAttr.PolicyName))
                        selectedPolicyName = classSanitizedAttr.PolicyName;
                if (propAttr != null && !string.IsNullOrEmpty(propAttr.PolicyName))
                        selectedPolicyName = propAttr.PolicyName;

                // if it is string
                if (prop.PropertyType == typeof(string))
                {
                    try
                    {
                        var value = CleanString(
                            prop.Name.ToCamelCase(), 
                            (string)prop.GetValue(input), 
                            selectedPolicyName, 
                            modelState);
                        prop.SetValue(input, value);
                    }
                    catch (ProhabitedStringValueException ex)
                    {
                        throw ex;
                    }
                }

                // if it is a IDictionary only check the values
                if (typeof(IDictionary).IsAssignableFrom(prop.PropertyType))
                {
                    var dict = (IDictionary)prop.GetValue(input);
                    foreach (var key in dict.Keys)
                    {
                        dict[key] = Sanitize(dict[key], isAllInputs, modelState, selectedPolicyName, prop.Name.ToCamelCase());
                    }
                }

                // if it is IList type
                if (TypeIdentifier.IsGenericList(prop.PropertyType))
                {
                    var list = (IList)prop.GetValue(input);

                    for (int i = 0; i < (list).Count; i++)
                    {
                        var result = Sanitize(list[i], isAllInputs, modelState, selectedPolicyName, prop.Name.ToCamelCase());
                        list[i] = result;
                    }
                }
            }

            return input;
        }

        private static string CleanString(string keyName, string orginalTxt, string policyName, ModelStateDictionary modelState)
        {
            HtmlSanitizerOptions htmlSanitizerOptions;
            HtmlSanitizer htmlSanitizer;
            PatternSanitizer patternSanitizer;
            InputSanitizerPolicy policy = null;
            if (!string.IsNullOrEmpty(policyName) &&
                    PolicyCollection.Policies.TryGetValue(policyName, out policy))
            {
                htmlSanitizerOptions = new HtmlSanitizerOptions()
                {
                    AllowedAtRules= policy.AllowedAtRules,
                    AllowedSchemes= policy.AllowedSchemes,
                    AllowedAttributes= policy.AllowedAttributes,
                    AllowedCssClasses= policy.AllowedCssClasses,
                    AllowedCssProperties= policy.AllowedCssProperties,
                    AllowedTags= policy.AllowedTags,
                    UriAttributes= policy.UriAttributes
                };

                htmlSanitizer = new HtmlSanitizer(htmlSanitizerOptions);
                patternSanitizer = new PatternSanitizer(policy);
            }
            else
            {
                htmlSanitizer = new HtmlSanitizer();
                patternSanitizer = new PatternSanitizer();
            }

            var value = patternSanitizer.Sanitize(orginalTxt);
            value = htmlSanitizer.Sanitize(value);

            if (policy != null)
            {
                switch (policy.InvalidInputBehaviour)
                {
                    case InvalidInputBehaviour.JustSanitize:
                        // do nothing, already sanitized
                        break;
                    case InvalidInputBehaviour.ThrowException:
                        if (orginalTxt != value)
                            throw new ProhabitedStringValueException(policy.ExceptionMessage ?? "");
                        break;
                    case InvalidInputBehaviour.SetModelState:
                        if (orginalTxt != value)
                            modelState.TryAddModelError(keyName, policy.ExceptionMessage);
                        break;
                }
            }
            return value;
        }
    }
}
