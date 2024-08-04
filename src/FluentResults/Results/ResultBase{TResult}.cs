using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace FluentResults
{
    /// <summary>
    /// Default implementation of <see cref="IResultBase"/> generics
    /// </summary>
    public abstract class ResultBase<TResult> : ResultBase
        where TResult : ResultBase<TResult>

    {
        /// <summary>
        /// Add a reason (success or error)
        /// </summary>
        public TResult WithReason(IReason reason)
        {
            Reasons.Add(reason);
            return (TResult)this;
        }

        /// <summary>
        /// Add multiple reasons (success or error)
        /// </summary>
        public TResult WithReasons(IEnumerable<IReason> reasons)
        {
            Reasons.AddRange(reasons);
            return (TResult)this;
        }

        /// <summary>
        /// Add an error
        /// </summary>
        public TResult WithError(string errorMessage)
        {
            return WithError(Result.Settings.ErrorFactory(errorMessage));
        }

        /// <summary>
        /// Add an error
        /// </summary>
        public TResult WithError(IError error)
        {
            return WithReason(error);
        }

        /// <summary>
        /// Add multiple errors
        /// </summary>
        public TResult WithErrors(IEnumerable<IError> errors)
        {
            return WithReasons(errors);
        }

        /// <summary>
        /// Add multiple errors
        /// </summary>
        public TResult WithErrors(IEnumerable<string> errors)
        {
            return WithReasons(errors.Select(errorMessage => Result.Settings.ErrorFactory(errorMessage)));
        }

        /// <summary>
        /// Add an error
        /// </summary>
        public TResult WithError<TError>()
            where TError : IError, new()
        {
            return WithError(new TError());
        }

        /// <summary>
        /// Add a success
        /// </summary>
        public TResult WithSuccess(string successMessage)
        {
            return WithSuccess(Result.Settings.SuccessFactory(successMessage));
        }

        /// <summary>
        /// Add a success
        /// </summary>
        public TResult WithSuccess(ISuccess success)
        {
            return WithReason(success);
        }

        /// <summary>
        /// Add a success
        /// </summary>
        public TResult WithSuccess<TSuccess>()
            where TSuccess : Success, new()
        {
            return WithSuccess(new TSuccess());
        }

        /// <summary>
        /// Add multiple successes
        /// </summary>
        public TResult WithSuccesses(IEnumerable<ISuccess> successes)
        {
            foreach (var success in successes)
            {
                WithSuccess(success);
            }

            return (TResult)this;
        }

        /// <summary>
        /// Log the result. Configure the logger via Result.Setup(..)
        /// </summary>
        public TResult Log(LogLevel logLevel = LogLevel.Information)
        {
            return Log(string.Empty, null, logLevel);
        }

        /// <summary>
        /// Log the result. Configure the logger via Result.Setup(..)
        /// </summary>
        public TResult Log(string context, LogLevel logLevel = LogLevel.Information)
        {
            return Log(context, null, logLevel);
        }

        /// <summary>
        /// Log the result with a specific logger context. Configure the logger via Result.Setup(..)
        /// </summary>
        public TResult Log(string context, string content, LogLevel logLevel = LogLevel.Information)
        {
            var logger = Result.Settings.Logger;

            logger.Log(context, content, this, logLevel);

            return (TResult)this;
        }

        /// <summary>
        /// Log the result with a typed context. Configure the logger via Result.Setup(..)
        /// </summary>
        public TResult Log<TContext>(LogLevel logLevel = LogLevel.Information)
        {
            return Log<TContext>(null, logLevel);
        }

        /// <summary>
        /// Log the result with a typed context. Configure the logger via Result.Setup(..)
        /// </summary>
        public TResult Log<TContext>(string content, LogLevel logLevel = LogLevel.Information)
        {
            var logger = Result.Settings.Logger;

            logger.Log<TContext>(content, this, logLevel);

            return (TResult)this;
        }

        /// <summary>
        /// Log the result only when it is successful. Configure the logger via Result.Setup(..)
        /// </summary>
        public TResult LogIfSuccess(LogLevel logLevel = LogLevel.Information)
        {
            if (IsSuccess)
                return Log(logLevel);

            return (TResult)this;
        }

        /// <summary>
        /// Log the result with a specific logger context only when it is successful. Configure the logger via Result.Setup(..)
        /// </summary>
        public TResult LogIfSuccess(string context, string content = null, LogLevel logLevel = LogLevel.Information)
        {
            if (IsSuccess)
                return Log(context, content, logLevel);

            return (TResult)this;
        }

        /// <summary>
        /// Log the result with a typed context only when it is successful. Configure the logger via Result.Setup(..)
        /// </summary>
        public TResult LogIfSuccess<TContext>(string content = null, LogLevel logLevel = LogLevel.Information)
        {
            if (IsSuccess)
                return Log<TContext>(content, logLevel);

            return (TResult)this;
        }

        /// <summary>
        /// Log the result only when it is failed. Configure the logger via Result.Setup(..)
        /// </summary>
        public TResult LogIfFailed(LogLevel logLevel = LogLevel.Error)
        {
            if (IsFailed)
                return Log(logLevel);

            return (TResult)this;
        }

        /// <summary>
        /// Log the result with a specific logger context only when it is failed. Configure the logger via Result.Setup(..)
        /// </summary>
        public TResult LogIfFailed(string context, string content = null, LogLevel logLevel = LogLevel.Error)
        {
            if (IsFailed)
                return Log(context, content, logLevel);

            return (TResult)this;
        }

        /// <summary>
        /// Log the result with a typed context only when it is failed. Configure the logger via Result.Setup(..)
        /// </summary>
        public TResult LogIfFailed<TContext>(string content = null, LogLevel logLevel = LogLevel.Error)
        {
            if (IsFailed)
                return Log<TContext>(content, logLevel);

            return (TResult)this;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var reasonsString = Reasons.Any()
                                    ? $", Reasons='{ReasonFormat.ReasonsToString(Reasons)}'"
                                    : string.Empty;

            return $"Result: IsSuccess='{IsSuccess}'{reasonsString}";
        }
    }
}