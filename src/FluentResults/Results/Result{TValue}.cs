using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace FluentResults
{
    /// <summary>
    /// A result containing a value of type <typeparamref name="TValue"/>
    /// </summary>
    /// <typeparam name="TValue">The type of the value</typeparam>
    public class Result<TValue> : ResultBase<Result<TValue>>, IResult<TValue>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Result()
        { }

        private TValue? _value;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TValue? ValueOrDefault => _value;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TValue Value
        {
            get
            {
                ThrowIfFailed();

                return _value!;
            }
            private set
            {
                ThrowIfFailed();

                _value = value;
            }
        }

        /// <summary>
        /// Set value
        /// </summary>
        public Result<TValue> WithValue(TValue value)
        {
            // TODO: should this only be used for Successful results? Not sure, many callers seem to potentially pass null
            Value = value;
            return this;
        }

        /// <summary>
        /// Map all errors of the result via errorMapper
        /// </summary>
        /// <param name="errorMapper"></param>
        /// <returns></returns>
        public Result<TValue> MapErrors(Func<IError, IError> errorMapper)
        {
            if (IsSuccess)
                return this;

            return new Result<TValue>()
                .WithErrors(Errors.Select(errorMapper))
                .WithSuccesses(Successes);
        }

        /// <summary>
        /// Map all successes of the result via successMapper
        /// </summary>
        /// <param name="successMapper"></param>
        /// <returns></returns>
        public Result<TValue> MapSuccesses(Func<ISuccess, ISuccess> successMapper)
        {
            return new Result<TValue>()
                .WithValue(ValueOrDefault)
                .WithErrors(Errors)
                .WithSuccesses(Successes.Select(successMapper));
        }

        /// <summary>
        /// Convert result with value to result without value
        /// </summary>
        public Result ToResult()
        {
            return new Result()
                .WithReasons(Reasons);
        }

        /// <summary>
        /// Convert result with value to result with another value. Use valueConverter parameter to specify the value transformation logic.
        /// </summary>
        public Result<TNewValue> ToResult<TNewValue>(Func<TValue, TNewValue> valueConverter = null)
        {
            return Map(valueConverter);
        }

        /// <summary>
        /// Convert result with value to result with another value. Use valueConverter parameter to specify the value transformation logic.
        /// </summary>
        public Result<TNewValue> Map<TNewValue>(Func<TValue, TNewValue> valueConverter)
        {
            if (IsSuccess && valueConverter == null)
                throw new ArgumentException("If result is success then valueConverter should not be null");

            return new Result<TNewValue>()
                   .WithValue(IsFailed ? default : valueConverter(Value))
                   .WithReasons(Reasons);
        }

        /// <summary>
        /// Convert result with value to result with another value that may fail.
        /// </summary>
        /// <example>
        /// <code>
        ///  var bakeryDtoResult = result
        ///     .Bind(GetWhichMayFail)
        ///     .Bind(ProcessWhichMayFail)
        ///     .Bind(FormattingWhichMayFail);
        /// </code>
        /// </example>
        /// <param name="bind">Transformation that may fail.</param>
        public Result<TNewValue> Bind<TNewValue>(Func<TValue, Result<TNewValue>> bind)
        {
            var result = new Result<TNewValue>();
            result.WithReasons(Reasons);
            
            if (IsSuccess)
            {
                var converted = bind(Value);
                result.WithValue(converted.ValueOrDefault);
                result.WithReasons(converted.Reasons);
            }

            return result;
        }
        
        /// <summary>
        /// Convert result with value to result with another value that may fail asynchronously.
        /// </summary>
        /// <example>
        /// <code>
        ///  var bakeryDtoResult = await result.Bind(GetWhichMayFail);
        /// </code>
        /// </example>
        /// <param name="bind">Transformation that may fail.</param>
        public async Task<Result<TNewValue>> Bind<TNewValue>(Func<TValue, Task<Result<TNewValue>>> bind)
        {
            var result = new Result<TNewValue>();
            result.WithReasons(Reasons);
            
            if (IsSuccess)
            {
                var converted = await bind(Value);
                result.WithValue(converted.ValueOrDefault);
                result.WithReasons(converted.Reasons);
            }

            return result;
        }
        
        /// <summary>
        /// Convert result with value to result with another value that may fail asynchronously.
        /// </summary>
        /// <example>
        /// <code>
        ///  var bakeryDtoResult = await result.Bind(GetWhichMayFail);
        /// </code>
        /// </example>
        /// <param name="bind">Transformation that may fail.</param>
        public async ValueTask<Result<TNewValue>> Bind<TNewValue>(Func<TValue, ValueTask<Result<TNewValue>>> bind)
        {
            var result = new Result<TNewValue>();
            result.WithReasons(Reasons);
            
            if (IsSuccess)
            {
                var converted = await bind(Value);
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
        public Result Bind(Func<TValue, Result> action)
        {
            var result = new Result();
            result.WithReasons(Reasons);

            if (IsSuccess)
            {
                var converted = action(Value);
                result.WithReasons(converted.Reasons);
            }

            return result;
        }
        
        /// <summary>
        /// Execute an action which returns a <see cref="Result"/> asynchronously.
        /// </summary>
        /// <example>
        /// <code>
        ///  var done = await result.Bind(ActionWhichMayFail);
        /// </code>
        /// </example>
        /// <param name="action">Action that may fail.</param>
        public async Task<Result> Bind(Func<TValue, Task<Result>> action)
        {
            var result = new Result();
            result.WithReasons(Reasons);

            if (IsSuccess)
            {
                var converted = await action(Value);
                result.WithReasons(converted.Reasons);
            }

            return result;
        }
        
        /// <summary>
        /// Execute an action which returns a <see cref="Result"/> asynchronously.
        /// </summary>
        /// <example>
        /// <code>
        ///  var done = await result.Bind(ActionWhichMayFail);
        /// </code>
        /// </example>
        /// <param name="action">Action that may fail.</param>
        public async ValueTask<Result> Bind(Func<TValue, ValueTask<Result>> action)
        {
            var result = new Result();
            result.WithReasons(Reasons);

            if (IsSuccess)
            {
                var converted = await action(Value);
                result.WithReasons(converted.Reasons);
            }

            return result;
        }

        /// <summary>
        /// ToString implementation
        /// </summary>
        public override string ToString()
        {
            var baseString = base.ToString();
            var valueString = ValueOrDefault.ToLabelValueStringOrEmpty(nameof(Value));
            return $"{baseString}, {valueString}";
        }

        /// <summary>
        /// Implicit conversion from <see cref="Result{TValue}"/> having a value to <see cref="Result{object}"/>
        /// </summary>
        public static implicit operator Result<object>(Result<TValue> result)
        {
            return result.ToResult<object>(value => value);
        }

        /// <summary>
        /// Implicit conversion of a value to <see cref="Result{TValue}"/>
        /// </summary>
        public static implicit operator Result<TValue>(TValue value)
        {
            if (value is Result<TValue> r)
                return r;

            // If value is already a Result you don't want to just wrap it again (possibly converting a Fail into a Success)
            Type t;
            if (value != null && typeof(IResultBase).IsAssignableFrom(t = value.GetType()))
            {
                // Exactly like the convertion above to Result<object>, but can be to any supertype of TValue
                var typeTo = typeof(TValue);
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var typeFrom = t.GetGenericArguments()[0];
                    if (typeTo.IsAssignableFrom(typeFrom))
                    {
                        // Invoke Map using reflection
                        var m = t.GetMethod("Map").MakeGenericMethod(typeTo); // Result<TNewValue> Map<TValue>
                        var genericTypes = new Type[] { typeTo, typeFrom };
                        var func = typeof(Func<,>).MakeGenericType(genericTypes);
                        //https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/expression-trees/expression-trees-building#build-code-in-depth
                        var from = Expression.Parameter(typeFrom, "from");
                        var cast = Expression.Convert(from, typeTo); // cast
                        var del = Expression.Lambda(cast, from).Compile();
                        var mappedResult = m.Invoke(value, new object[] { del });
                        return (Result<TValue>) mappedResult;
                    }
                    //throw new ArgumentException($"Cannot convert automatically from Result<{typeFrom.Name}> to Result<{typeTo.Name}>");
                }
                // Can't infer conversion to TValue type, but we should at least preserve success/fail/errors
                var original = (IResultBase)value;
                if (original.IsFailed)
                    return Result.Fail<TValue>(original.Errors);
                else if (typeTo == typeof(object)) // if it's to object then it's ok that we can't map result to it
                    return Result.Ok<TValue>(default).WithReasons(original.Reasons);
                //else // converting a Successful Result<SalesOrder> to Result<Bananas> seems very ugly and unsafe
                //    return Result.Ok<TValue>(default).WithReasons(original.Reasons);

                throw new ArgumentException($"Cannot convert automatically from Result to Result<{typeTo.Name}>");
            }

            return Result.Ok(value);
        }
        
        /// <summary>
        /// Implicit conversion of an <see cref="Error"/> to <see cref="Result{TValue}"/>
        /// </summary>
        public static implicit operator Result<TValue>(Error error)
        {
            return Result.Fail(error).ToResult<TValue>(default);
        }

        /// <summary>
        /// Implicit conversion of a list of <see cref="Error"/> to <see cref="Result{TValue}"/>
        /// </summary>
        public static implicit operator Result<TValue>(List<Error> errors)
        {
            return Result.Fail(errors).ToResult<TValue>(default);
        }

        /// <summary>
        /// Deconstruct Result
        /// The nice thing about using Deconstructors (instead of accessing <see cref="ResultBase.Errors"/>) is that 
        /// the error object will be NULL in case of a success, so you don't have to check for empty arrays, etc.
        /// So you always get a OneOf semantic - either errors==null or value==null (or default(T) for non-nullable types)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="errors"></param>
        public void Deconstruct(out TValue? value, out List<IError>? errors)
        {
            value = IsSuccess ? Value : default;
            errors = IsFailed ? Errors : default;
        }

        private void ThrowIfFailed()
        {
            if (IsFailed)
                throw new InvalidOperationException($"Result is in status failed. Value is not set. Having: {ReasonFormat.ErrorReasonsToString(Errors)}");
        }
    }
}