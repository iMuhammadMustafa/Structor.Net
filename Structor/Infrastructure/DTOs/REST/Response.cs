namespace Structor.Infrastructure.DTOs.REST;

public class Response<T>
{
    public T? Data { get; set; }
    public object? Error { get; set; }
    public string? ErrorMessage { get; set; }
    public ResponseStatus Status { get; set; }
    public int StatusCode { get; set; } = 200;
    public Pagination? Pagination { get; set; }

    //public static implicit operator T(Response<T> response) => response.Data;
    //public static implicit operator string(Response<T> response) => response.Status.ToString();
    //public static explicit operator Exception(Response<T> response) => response.Error;
    //public static explicit operator int(Response<T> response) => response.StatusCode;


    public Response<T> WithData(T data, int statusCode = 200)
    {
        Data = data;

        StatusCode = statusCode;
        Status = ResponseStatus.Success;
        return this;
    }
    public Response<T> WithPagination(Pagination pagination)
    {
        Pagination = pagination;
        return this;
    }
    public Response<T> WithError(Exception? error  = null, int statusCode = 500)
    {
        Error = error;
        Status = ResponseStatus.Failure;
        StatusCode = statusCode;
        return this;
    }
    public Response<T?> WithError(string message, int statusCode = 500)
    {
        ErrorMessage = message;
        Status = ResponseStatus.Failure;
        StatusCode = statusCode;

        return this;
    }
    public Response<T> WithStatusCode(int statusCode)
    {
        StatusCode = statusCode;
        return this;
    }
    public Response<T> WithSuccess()
    {
        Status = ResponseStatus.Success;
        return this;
    }
    public Response<T> WithFailure()
    {
        Status = ResponseStatus.Failure;
        return this;
    }
}

public class Pagination
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
//    public static Response<T> WithData<T>(this Response<T> response, T data) where T : class
//    {
//        response.Data = data;
//        return response;
//    }
//    public static Response<T> WithSuccess<T>(this Response<T> response) where T : class
//    {
//        response.Status = "Success";
//        return response;
//    }
//    public static Response<T> WithFailure<T>(this Response<T> response) where T : class
//    {
//        response.Status = "Failure";
//        return response;
//    }
//    public static Response<T> WithError<T>(this Response<T> response, object error) where T : class
//    {
//        response.Error = error;
//        return response;
//    }

//}