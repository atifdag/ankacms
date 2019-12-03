namespace AnkaCMS.Core.Exceptions
{
    public class BusinessException : BaseApplicationException
    {
        public BusinessException(string message) : base(message) { }
    }
}