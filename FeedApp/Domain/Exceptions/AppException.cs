namespace FeedApp.Domain.Exceptions
{
    public abstract class AppException : Exception
    {
        public string ErrorCode { get; }

        protected AppException(string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        protected AppException(string errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
