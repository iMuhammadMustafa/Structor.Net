namespace Structor.Net.Infrastructure.DTOs.REST;

public class IResponse<T>
{
    public T? Data { get; set; }
    public object? Error { get; set; }
    public int StatusCode { get; set; }
    public ResponseStatus? Status { get; set; } = null;
    public string? Message { get; set; }
    public IPagination? Pagination { get; set; }


    public IResponse<T> WithData(T data, int statusCode = 200)
    {
        //this.Status = ResponseStatus.Success;
        this.Data = data;

        this.StatusCode = statusCode;
        return this;
    }
    public IResponse<T> WithError(object error, int statusCode = 500)
    {
        this.Status = ResponseStatus.Failure;
        this.Error = error;
        this.StatusCode = statusCode;
        return this;
    }
    public IResponse<T> WithPagination(IPagination pagination)
    {
        this.Pagination = pagination;
        return this;
    }
    public IResponse<T?> WithMessage(string message)
    {
        this.Message = message;
        return this;
    }
    public IResponse<T> WithStatusCode(int statusCode)
    {
        this.StatusCode = statusCode;
        return this;
    }
    public IResponse<T> WithSuccess()
    {
        this.Status = ResponseStatus.Success;
        return this;
    }
    public IResponse<T> WithFailure()
    {
        this.Status = ResponseStatus.Failure;
        return this;
    }
}

public class IPagination
{
    private int _PageNumber;
    private int _PageSize;
    public int PageNumber
    {
        get
        {
            return _PageNumber;
        }
        set
        {
            _PageNumber = value < 0 ? 0 : value;
        }

    }
    public int PageSize
    {
        get
        {
            return _PageSize;
        }
        set
        {
            if (value < 0) { _PageSize = 10; }
            if (value > 10) { _PageSize = value; }
            if (value > 100) { _PageSize = 100; }
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