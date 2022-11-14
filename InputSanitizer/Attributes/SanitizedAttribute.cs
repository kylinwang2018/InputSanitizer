using Ganss.Xss;
using System;

namespace InputSanitizer
{
    /// <summary>
    ///     Object will be sanitized when applying this attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class SanitizedAttribute : Attribute
    {
        /// <summary>
        ///     Apply sanitizer with specific policy
        /// </summary>
        public string PolicyName { get; set; } = null;
    }
}
