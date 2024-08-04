// ReSharper disable once CheckNamespace
namespace FluentResults
{
    /// <summary>
    /// Definition of a result with a value of type <typeparamref name="TValue"/>
    /// </summary>
    /// <typeparam name="TValue">The type of the value</typeparam>
    public interface IResult<out TValue> : IResultBase
    {
        /// <summary>
        /// Get the Value. If result is failed then an Exception is thrown because a failed result has no value. Opposite see property ValueOrDefault.
        /// </summary>
        TValue Value { get; }

        /// <summary>
        /// Get the Value. If result is failed then a default value is returned. Opposite see property Value.
        /// </summary>
        TValue ValueOrDefault { get; }
    }
}