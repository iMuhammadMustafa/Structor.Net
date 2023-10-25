namespace Structor.Infrastructure.DTOs.REST;

public class Response<T>
{
    public T? Data { get; set; }
    public object? Error { get; set; }
    public string? ErrorMessage { get; set; }
    public ResponseStatus Status { get; set; }
    public int StatusCode { get; set; } = 200;
    public Pagination? Pagination { get; set; }

    public static ResponseBuilder<T> Create()
    {
        return new ResponseBuilder<T>();
    }

    public Response<T> WithData(T data, int statusCode = StatusCodes.Status200OK)
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
    public Response<T> WithException(Exception? error = null, int statusCode = StatusCodes.Status500InternalServerError)
    {
        Error = error;
        Status = ResponseStatus.Failure;
        StatusCode = statusCode;
        return this;
    }
    public Response<T> WithError(string message, int statusCode = StatusCodes.Status500InternalServerError)
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


}
public class ResponseBuilder<T>
{
    private Response<T> _response;

    public ResponseBuilder()
    {
        _response = new Response<T>();
    }
    public ResponseBuilder<T> WithData(T data, int statusCode = StatusCodes.Status200OK)
    {
        _response.Data = data;

        _response.StatusCode = statusCode;
        _response.Status = ResponseStatus.Success;
        return this;
    }
    public ResponseBuilder<T> WithPagination(Pagination pagination)
    {
        _response.Pagination = pagination;
        return this;
    }
    public ResponseBuilder<T> WithException(Exception? error = null, int statusCode = StatusCodes.Status500InternalServerError)
    {
        _response.Error = error;
        _response.Status = ResponseStatus.Failure;
        _response.StatusCode = statusCode;
        return this;
    }
    public ResponseBuilder<T?> WithError(string message, int statusCode = StatusCodes.Status500InternalServerError)
    {
        _response.ErrorMessage = message;
        _response.Status = ResponseStatus.Failure;
        _response.StatusCode = statusCode;

        return this;
    }
    public ResponseBuilder<T> WithStatusCode(int statusCode)
    {
        _response.StatusCode = statusCode;
        return this;
    }
    public ResponseBuilder<T> WithSuccess()
    {
        _response.Status = ResponseStatus.Success;
        return this;
    }
    public ResponseBuilder<T> WithFailure()
    {
        _response.Status = ResponseStatus.Failure;
        return this;
    }
    public Response<T> Build()
    {
        return _response;
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

    //public static implicit operator T(Response<T> response) => response.Data;
    //public static implicit operator string(Response<T> response) => response.Status.ToString();
    //public static explicit operator Exception(Response<T> response) => response.Error;
    //public static explicit operator int(Response<T> response) => response.StatusCode;