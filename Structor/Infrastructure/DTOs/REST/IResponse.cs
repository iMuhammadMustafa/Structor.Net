namespace Structor.Infrastructure.DTOs.REST;

public class IResponse<T>
{
    public T? Data { get; set; }
    public int StatusCode { get; set; } = 200;
    public ResponseStatus Status { get; set; }
    public object? Error { get; set; }
    public string? Message { get; set; }
    public IPagination? Pagination { get; set; }


    public IResponse<T> WithData(T data, int statusCode = 200)
    {
        Data = data;

        StatusCode = statusCode;
        Status = ResponseStatus.Success;
        return this;
    }
    public IResponse<T> WithPagination(IPagination pagination)
    {
        Pagination = pagination;
        return this;
    }
    public IResponse<T> WithError(object error, int statusCode = 500)
    {
        Status = ResponseStatus.Failure;
        Error = error;
        StatusCode = statusCode;
        return this;
    }
    public IResponse<T?> WithMessage(string message)
    {
        Message = message;
        return this;
    }
    public IResponse<T> WithStatusCode(int statusCode)
    {
        StatusCode = statusCode;
        return this;
    }
    public IResponse<T> WithSuccess()
    {
        Status = ResponseStatus.Success;
        return this;
    }
    public IResponse<T> WithFailure()
    {
        Status = ResponseStatus.Failure;
        return this;
    }
}

public class IPagination
{
    private int _pageNumber;
    private int _pageSize;
    public int Page
    {
        get
        {
            return _pageNumber;
        }
        set
        {
            _pageNumber = value < 0 ? 0 : value;
        }

    }
    public int Size
    {
        get
        {
            return _pageSize;
        }
        set
        {
            if (value <= 0) { _pageSize = 10; }
            if (value >= 10) { _pageSize = value; }
            if (value > 100) { _pageSize = 100; }
        }
    }
    public int? TotalCount { get; set; }
    public int? TotalPages { get; set; }

}

public enum ResponseStatus
{
    Success,
    Failure
}


//public static class IResponseExtensions
//{
//    public static IResponse<T> WithData<T>(this IResponse<T> response, T data) where T : class
//    {
//        response.Data = data;
//        return response;
//    }
//    public static IResponse<T> WithSuccess<T>(this IResponse<T> response) where T : class
//    {
//        response.Status = "Success";
//        return response;
//    }
//    public static IResponse<T> WithFailure<T>(this IResponse<T> response) where T : class
//    {
//        response.Status = "Failure";
//        return response;
//    }
//    public static IResponse<T> WithError<T>(this IResponse<T> response, object error) where T : class
//    {
//        response.Error = error;
//        return response;
//    }

//}