using Trimly.Core.Domain.Enum;

namespace Trimly.Core.Domain.Utils
{
    /// <summary>
    /// Represents an error with a code, description, and type.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="description">A description of the error.</param>
        /// <param name="errorType">The type of the error.</param>
        private Error(
            string code,
            string description,
            ErrorType errorType)
        {
            Code = code;
            Description = description;
            ErrorType = errorType;
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets a description of the error.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the type of the error.
        /// </summary>
        public ErrorType ErrorType { get; }

        /// <summary>
        /// Creates a new <see cref="Error"/> instance representing a general failure.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="description">A description of the error.</param>
        /// <returns>A new <see cref="Error"/> instance indicating failure.</returns>
        public static Error Failure(string code, string description) =>
            new Error(code, description, ErrorType.Failure);

        /// <summary>
        /// Creates a new <see cref="Error"/> instance representing a not found error.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="description">A description of the error.</param>
        /// <returns>A new <see cref="Error"/> instance indicating not found.</returns>
        public static Error NotFound(string code, string description) =>
            new Error(code, description, ErrorType.NotFound);
    }

}
