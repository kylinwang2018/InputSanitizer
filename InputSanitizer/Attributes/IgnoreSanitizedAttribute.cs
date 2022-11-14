using System;

namespace InputSanitizer
{
    /// <summary>
    ///     object sanitizing will be ignored when this attribute is applied
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class IgnoreSanitizedAttribute : Attribute
    {
        
    }
}
