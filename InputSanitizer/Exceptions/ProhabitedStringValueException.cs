using System;

namespace InputSanitizer
{
    /// <summary>
    /// The Exception that related to this library
    /// </summary>
    [Serializable]
    public class ProhabitedStringValueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProhabitedStringValueException"/> class
        /// </summary>
        public ProhabitedStringValueException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProhabitedStringValueException"/> class with the
        ///     specified error message.
        /// </summary>
        /// <param name="message">The error message string.</param>
        public ProhabitedStringValueException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProhabitedStringValueException"/> class with the
        ///     specified error message and a reference to the inner exception that is the cause
        ///     of this exception.
        /// </summary>
        /// <param name="message">The error message string.</param>
        /// <param name="inner">The inner exception reference.</param>
        public ProhabitedStringValueException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
