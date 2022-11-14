using System;
using System.Collections.Generic;
using System.Text;

namespace InputSanitizer.Options
{
    internal static class PatternSanitizerDefaultOptions
    {
        public static ISet<string> ProhabitedRegexPatterns { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "drop","--"
        };
    }
}
