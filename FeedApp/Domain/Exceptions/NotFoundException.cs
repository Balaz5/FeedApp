namespace FeedApp.Domain.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string errorCode, string message)
            : base(errorCode, message) { }
    }
}
