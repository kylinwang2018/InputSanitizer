using Ganss.Xss;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace InputSanitizer.Infrastructure
{
    internal class Cleaner
    {
        public static string CleanString(string keyName, string orginalTxt, string policyName, ModelStateDictionary modelState)
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
                    AllowedAtRules = policy.AllowedAtRules,
                    AllowedSchemes = policy.AllowedSchemes,
                    AllowedAttributes = policy.AllowedAttributes,
                    AllowedCssClasses = policy.AllowedCssClasses,
                    AllowedCssProperties = policy.AllowedCssProperties,
                    AllowedTags = policy.AllowedTags,
                    UriAttributes = policy.UriAttributes
                };

                htmlSanitizer = new HtmlSanitizer(htmlSanitizerOptions);
                patternSanitizer = new PatternSanitizer(policy);
            }
            else
            {
                htmlSanitizer = new HtmlSanitizer();
                patternSanitizer = new PatternSanitizer();
            }

            var value = orginalTxt;
            if (policy.InvalidInputBehaviour == InvalidInputBehaviour.EncodeHtmlAndSanitizeRegex)
                value = WebUtility.UrlEncode(value);
            else
                value = htmlSanitizer.Sanitize(value);
            value = patternSanitizer.Sanitize(value);
            if (policy != null)
            {
                switch (policy.InvalidInputBehaviour)
                {
                    case InvalidInputBehaviour.JustSanitize:
                    case InvalidInputBehaviour.EncodeHtmlAndSanitizeRegex:
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
