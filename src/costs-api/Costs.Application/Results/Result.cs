using System;
using Costs.Domain.Results;

namespace Costs.Application.Results;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public Error? Error { get; private set; }

    public static Result<T> Success(T value) =>
        new Result<T> { IsSuccess = true, Value = value, Error = null };

    public static Result<T> Failure(Error error)
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error));
        return new Result<T> { IsSuccess = false, Value = default, Error = error };
    }
}
