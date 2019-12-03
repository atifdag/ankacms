namespace AnkaCMS.Core.Exceptions
{
    public class NotificationException : BaseApplicationException
    {
        public NotificationException(string message) : base(message) { }
        public string ErrorType { get; set; }
    }
}