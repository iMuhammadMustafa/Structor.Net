namespace Structor.Core.Exceptions
{
    public class HttpException : Exception
    {

        public int StatusCode { get; set; }
        public HttpException()
        {
            
        }
        public HttpException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
        public HttpException(string message, Exception innerException, int statusCode) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
