namespace Common.Models.Response.Abstract;

public interface IResponse
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public object? ResponseResult { get; init; }
}