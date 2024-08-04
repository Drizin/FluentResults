using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace FluentResults
{
    /// <summary>
    /// Definition of a ResultBase
    /// </summary>
    public interface IResultBase
    {
        /// <summary>
        /// Is true if Reasons contains at least one error
        /// </summary>
        bool IsFailed { get; }

        /// <summary>
        /// Is true if Reasons contains no errors
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Get all reasons (errors and successes)
        /// </summary>
        List<IReason> Reasons { get; }

        /// <summary>
        /// Get all errors (if any)
        /// If null it means it's a success
        /// </summary>
        List<IError>? Errors { get; }

        /// <summary>
        /// Get all successes
        /// </summary>
        List<ISuccess> Successes { get; }
    }

}