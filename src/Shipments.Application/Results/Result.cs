using System;
using Shipments.Domain.Results;

namespace Shipments.Application.Results;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// Encapsulates the outcome and any associated error information.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
public class Result<T>
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// The value returned by a successful operation.
    /// Only valid when IsSuccess is true.
    /// </summary>
    public T? Value { get; private set; }

    /// <summary>
    /// The error information if the operation failed.
    /// Only valid when IsSuccess is false.
    /// </summary>
    public Error? Error { get; private set; }

    /// <summary>
    /// Creates a successful result with the given value.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <returns>A successful result.</returns>
    public static Result<T> Success(T value)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Value = value,
            Error = null
        };
    }

    /// <summary>
    /// Creates a failed result with the given error.
    /// </summary>
    /// <param name="error">The error information.</param>
    /// <returns>A failed result.</returns>
    public static Result<T> Failure(Error error)
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error), "Error cannot be null for a failure result");

        return new Result<T>
        {
            IsSuccess = false,
            Value = default,
            Error = error
        };
    }
}
