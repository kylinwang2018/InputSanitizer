using Ganss.Xss;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace InputSanitizer.Infrastructure
{
    internal class JsonSanitizer
    {
        public static JsonElement Sanitize(
            JsonElement json,
            ModelStateDictionary modelState,
            string policyName = null,
            string lastPropertyName = null)
        {
            switch (json.ValueKind)
            {
                case JsonValueKind.Undefined:
                    // do nothing
                    break;
                case JsonValueKind.Object:
                    // 
                    var objNode = JsonNode.Parse(json.GetRawText());
                    foreach (var prop in objNode.AsObject().ToList())
                    {
                        var result = Sanitize(
                            JsonSerializer.SerializeToElement(prop.Value),
                            modelState, 
                            policyName, 
                            prop.Key);
                        objNode[prop.Key] = JsonNode.Parse(result.GetRawText());
                    }
                    json = JsonSerializer.SerializeToElement(objNode);
                    break;
                case JsonValueKind.Array:
                    // 
                    var arrayNode = JsonNode.Parse(json.GetRawText());

                    for (int i = 0; i < arrayNode.AsArray().Count; i++)
                    {
                        var result = Sanitize(
                           JsonSerializer.SerializeToElement(arrayNode[i]),
                           modelState,
                           policyName,
                           lastPropertyName);
                        arrayNode[i] = JsonNode.Parse(result.GetRawText());
                    }

                    json = JsonSerializer.SerializeToElement(arrayNode);
                    break;
                case JsonValueKind.String:
                    var text = json.ToString();
                    text = CleanString(lastPropertyName ?? "User Input", text, policyName, modelState);
                    json = JsonSerializer.SerializeToElement(text);
                    break;
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    // do nothing
                    break;
                default:
                    break;
            }
            return json;
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
