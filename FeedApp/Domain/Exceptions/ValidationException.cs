namespace Domain.Exceptions
{
    public class ValidationException : AppException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(string errorCode, string message)
            : base(errorCode, message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string errorCode, string message, IDictionary<string, string[]> errors)
            : base(errorCode, message)
        {
            Errors = errors;
        }
    }
}
