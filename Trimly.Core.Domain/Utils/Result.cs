using System.Text.Json.Serialization;

namespace Trimly.Core.Domain.Utils
{
    /// <summary>
    /// Represents the result of an operation, indicating success or failure.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets the error details if the operation failed.
        /// </summary>
        /// <remarks>
        /// This property will be ignored during serialization when it has its default value.
        /// </remarks>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Error? Error { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class, indicating success.
        /// </summary>
        protected Result()
        {
            IsSuccess = true;
            Error = default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class with the specified error.
        /// </summary>
        /// <param name="error">The error details.</param>
        protected Result(Error error)
        {
            IsSuccess = false;
            Error = error;
        }

        /// <summary>
        /// Implicitly converts an <see cref="Error"/> to a <see cref="Result"/>.
        /// </summary>
        /// <param name="error">The error to convert.</param>
        /// <returns>A <see cref="Result"/> indicating failure with the specified error.</returns>
        public static implicit operator Result(Error error) =>
           new(error);

        /// <summary>
        /// Creates a successful <see cref="Result"/>.
        /// </summary>
        /// <returns>A <see cref="Result"/> indicating success.</returns>
        public static Result Success() =>
            new();

        /// <summary>
        /// Creates a failed <see cref="Result"/> with the specified error.
        /// </summary>
        /// <param name="error">The error details.</param>
        /// <returns>A <see cref="Result"/> indicating failure with the specified error.</returns>
        public static Result Failure(Error error) =>
            new(error);
    }

    /// <summary>
    /// Represents the outcome of an operation that can either succeed with a value of type 
    /// or fail with an associated error. Provides utility methods for implicit conversion and result handling.
    /// The type of the value contained in the result when the operation is successful.
    /// </summary>
    public class ResultT<TValue> : Result
    {
        private readonly TValue? _value;

        private ResultT(TValue value) : base()
        {
            _value = value;
        }

        private ResultT(Error error) : base(error)
        {
            _value = default;
        }

        /// <summary>
        /// Gets the value of the result if the operation was successful.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when attempting to access the value while <see cref="IsSuccess"/> is false.
        /// </exception>
        public TValue Value =>
            IsSuccess ? _value! : throw new InvalidOperationException("Value cannot be accessed when IsSuccess is false");

        /// <summary>
        /// Implicitly converts an <see cref="Error"/> to a <see cref="ResultT{TValue}"/>.
        /// </summary>
        /// <param name="error">The error to convert.</param>
        /// <returns>A <see cref="ResultT{TValue}"/> indicating failure with the specified error.</returns>
        public static implicit operator ResultT<TValue>(Error error) =>
            new(error);

        /// <summary>
        /// Implicitly converts a value of type <typeparamref name="TValue"/> to a <see cref="ResultT{TValue}"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A <see cref="ResultT{TValue}"/> indicating success with the specified value.</returns>
        public static implicit operator ResultT<TValue>(TValue value) =>
            new(value);

        /// <summary>
        /// Creates a successful result with the provided value.
        /// </summary>
        /// <param name="value">The value associated with the successful result.</param>
        /// <returns>An instance of <see cref="ResultT{TValue}"/> representing success.</returns>
        public static ResultT<TValue> Success(TValue value) =>
            new(value);

        /// <summary>
        /// Creates a failed result with the provided error.
        /// </summary>
        /// <param name="error">The error associated with the failed result.</param>
        /// <returns>An instance of <see cref="ResultT{TValue}"/> representing failure.</returns>
        public static ResultT<TValue> Failure(Error error) =>
            new(error);
    }
}
