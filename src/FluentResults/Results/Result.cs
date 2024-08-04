using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace FluentResults
{
    /// <summary>
    /// Implementation of a Result
    /// </summary>
    public partial class Result : ResultBase<Result>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Result()
        { }

        /// <summary>
        /// Map all errors of the result via errorMapper
        /// </summary>
        /// <param name="errorMapper"></param>
        /// <returns></returns>
        public Result MapErrors(Func<IError, IError> errorMapper)
        {
            if (IsSuccess)
                return this;

            return new Result()
                .WithErrors(Errors.Select(errorMapper))
                .WithSuccesses(Successes);
        }

        /// <summary>
        /// Map all successes of the result via successMapper
        /// </summary>
        /// <param name="successMapper"></param>
        /// <returns></returns>
        public Result MapSuccesses(Func<ISuccess, ISuccess> successMapper)
        {
            return new Result()
                .WithErrors(Errors)
                .WithSuccesses(Successes.Select(successMapper));
        }

        /// <summary>
        /// Convert result without value to a result containing a value
        /// </summary>
        /// <typeparam name="TNewValue">Type of the value</typeparam>
        /// <param name="newValue">Value to add to the new result</param>
        public Result<TNewValue> ToResult<TNewValue>(TNewValue newValue = default)
        {
            return new Result<TNewValue>()
                .WithValue(IsFailed ? default : newValue)
                .WithReasons(Reasons);
        }

        /// <summary>
        /// Convert result to result with value that may fail.
        /// </summary>
        /// <example>
        /// <code>
        ///  var bakeryDtoResult = result.Bind(GetWhichMayFail);
        /// </code>
        /// </example>
        /// <param name="bind">Transformation that may fail.</param>
        public Result<TNewValue> Bind<TNewValue>(Func<Result<TNewValue>> bind)
        {
            var result = new Result<TNewValue>();
            result.WithReasons(Reasons);
            
            if (IsSuccess)
            {
                var converted = bind();
                result.WithValue(converted.ValueOrDefault);
                result.WithReasons(converted.Reasons);
            }

            return result;
        }
        
        /// <summary>
        /// Convert result to result with value that may fail asynchronously.
        /// </summary>
        /// <example>
        /// <code>
        ///  var bakeryDtoResult = result.Bind(GetWhichMayFail);
        /// </code>
        /// </example>
        /// <param name="bind">Transformation that may fail.</param>
        public async Task<Result<TNewValue>> Bind<TNewValue>(Func<Task<Result<TNewValue>>> bind)
        {
            var result = new Result<TNewValue>();
            result.WithReasons(Reasons);
            
            if (IsSuccess)
            {
                var converted = await bind();
                result.WithValue(converted.ValueOrDefault);
                result.WithReasons(converted.Reasons);
            }

            return result;
        }
        
        /// <summary>
        /// Convert result to result with value that may fail asynchronously.
        /// </summary>
        /// <example>
        /// <code>
        ///  var bakeryDtoResult = result.Bind(GetWhichMayFail);
        /// </code>
        /// </example>
        /// <param name="bind">Transformation that may fail.</param>
        public async ValueTask<Result<TNewValue>> Bind<TNewValue>(Func<ValueTask<Result<TNewValue>>> bind)
        {
            var result = new Result<TNewValue>();
            result.WithReasons(Reasons);
            
            if (IsSuccess)
            {
                var converted = await bind();
                result.WithValue(converted.ValueOrDefault);
                result.WithReasons(converted.Reasons);
            }

            return result;
        }
        
        /// <summary>
        /// Execute an action which returns a <see cref="Result"/>.
        /// </summary>
        /// <example>
        /// <code>
        ///  var done = result.Bind(ActionWhichMayFail);
        /// </code>
        /// </example>
        /// <param name="action">Action that may fail.</param>
        public Result Bind(Func<Result> action)
        {
            var result = new Result();
            result.WithReasons(Reasons);
            
            if (IsSuccess)
            {
                var converted = action();
                result.WithReasons(converted.Reasons);
            }

            return result;
        }
        
        /// <summary>
        /// Execute an action which returns a <see cref="Result"/> asynchronously.
        /// </summary>
        /// <example>
        /// <code>
        ///  var done = result.Bind(ActionWhichMayFail);
        /// </code>
        /// </example>
        /// <param name="action">Action that may fail.</param>
        public async Task<Result> Bind(Func<Task<Result>> action)
        {
            var result = new Result();
            result.WithReasons(Reasons);
            
            if (IsSuccess)
            {
                var converted = await action();
                result.WithReasons(converted.Reasons);
            }

            return result;
        }
        
        /// <summary>
        /// Execute an action which returns a <see cref="Result"/> asynchronously.
        /// </summary>
        /// <example>
        /// <code>
        ///  var done = result.Bind(ActionWhichMayFail);
        /// </code>
        /// </example>
        /// <param name="action">Action that may fail.</param>
        public async ValueTask<Result> Bind(Func<ValueTask<Result>> action)
        {
            var result = new Result();
            result.WithReasons(Reasons);
            
            if (IsSuccess)
            {
                var converted = await action();
                result.WithReasons(converted.Reasons);
            }

            return result;
        }

        /// <summary>
        /// Deconstruct Result
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="errors"></param>
        public void Deconstruct(out bool isSuccess, out List<IError> errors)
        {
            isSuccess = IsSuccess;
            errors = IsFailed ? Errors : default;
        }

        /// <summary>
        /// Implict conversion from <see cref="Error"/> to a <see cref="Result"/>
        /// </summary>
        /// <param name="error">The error</param>
        public static implicit operator Result(Error error)
        {
            return Fail(error);
        }

        /// <summary>
        /// Implict conversion from <see cref="List{Error}"/> to a <see cref="Result"/>
        /// </summary>
        /// <param name="errors">The errors</param>
        public static implicit operator Result(List<Error> errors)
        {
            return Fail(errors);
        }
    }
}