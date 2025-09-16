namespace CityAid.Application.Common.Models;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string[]? Errors { get; }

    private Result(bool isSuccess, T? value, string[]? errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(params string[] errors) => new(false, default, errors);
}

public class Result
{
    public bool IsSuccess { get; }
    public string[]? Errors { get; }

    private Result(bool isSuccess, string[]? errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(params string[] errors) => new(false, errors);
}