using AngleSharp.Css.Dom;
using Ganss.Xss;

namespace InputSanitizer.Demo.WebApi
{
    public static class InputSanitizerPolicies
    {
        public static InputSanitizerPolicy[] Policies { get; set; } =
        {
            new InputSanitizerPolicy()
            {
                Name = "Default",
                AllowedTags = new HashSet<string>(HtmlSanitizerDefaults.AllowedTags, StringComparer.OrdinalIgnoreCase),
                AllowedSchemes = new HashSet<string>(HtmlSanitizerDefaults.AllowedSchemes, StringComparer.OrdinalIgnoreCase),
                AllowedAttributes = new HashSet<string>(HtmlSanitizerDefaults.AllowedAttributes, StringComparer.OrdinalIgnoreCase),
                UriAttributes = new HashSet<string>(HtmlSanitizerDefaults.UriAttributes, StringComparer.OrdinalIgnoreCase),
                AllowedCssProperties = new HashSet<string>(HtmlSanitizerDefaults.AllowedCssProperties, StringComparer.OrdinalIgnoreCase),
                AllowedAtRules = new HashSet<CssRuleType>(HtmlSanitizerDefaults.AllowedAtRules),
                AllowedCssClasses = new HashSet<string>(HtmlSanitizerDefaults.AllowedClasses),
                ExceptionMessage = "What are you doing?",
                InvalidInputBehaviour = InvalidInputBehaviour.EncodeHtmlAndSanitizeRegex
            }
        };
    }
}
