using InputSanitizer.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace InputSanitizer
{
    internal class PatternSanitizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternSanitizer"/> class
        /// with the default policy.
        /// </summary>
        /// <param name="policy">Options to control the sanitizing.</param>
        public PatternSanitizer(InputSanitizerPolicy policy = null)
        {
            if (policy == null)
                ProhabitedRegexPatterns = new HashSet<string>(PatternSanitizerDefaultOptions.ProhabitedRegexPatterns, StringComparer.OrdinalIgnoreCase);
            else
                ProhabitedRegexPatterns = policy.ProhabitedRegexPatterns;
        }

        /// <summary>
        /// Sanitizing the text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Sanitize(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            foreach (var pattern in ProhabitedRegexPatterns)
            {
                text = Regex.Replace(text, pattern, "", RegexOptions.IgnoreCase);
            }

            return text;
        }

        public ISet<string> ProhabitedRegexPatterns { get; set; }
    }
}
