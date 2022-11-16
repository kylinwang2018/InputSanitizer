using Ganss.Xss;
using System;
using System.Collections.Generic;

namespace InputSanitizer
{
    /// <summary>
    ///     A policy for how to sanitize the input values
    /// </summary>
    public class InputSanitizerPolicy : HtmlSanitizerOptions
    {
        /// <summary>
        ///     Policy name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     When the input contains string violate the policy, which is
        ///     the output and input are different, the behaviour of the 
        ///     sanitizer.
        /// </summary>
        public InvalidInputBehaviour InvalidInputBehaviour { get; set; } = InvalidInputBehaviour.JustSanitize;

        /// <summary>
        ///     If the input contains invalid keywords or pattern, the sanitizer
        ///     will return this exception message to the controller
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        ///     Gets or sets the disallowed string patterns.
        /// </summary>
        public ISet<string> ProhabitedRegexPatterns { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     The behaviour that when invalid input happened
    /// </summary>
    public enum InvalidInputBehaviour
    { 
        /// <summary>
        ///     just sanitize input, no alart
        /// </summary>
        JustSanitize,

        /// <summary>
        ///     Sanitize disallowed string patterns after encode all html string
        /// </summary>
        EncodeHtmlAndSanitizeRegex,

        /// <summary>
        ///     Throw an exception that need to be handle
        /// </summary>
        ThrowException,

        /// <summary>
        ///     Set ModelState to false, add error message to ModelState
        /// </summary>
        SetModelState
    }
}
