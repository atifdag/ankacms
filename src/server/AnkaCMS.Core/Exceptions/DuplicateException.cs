namespace AnkaCMS.Core.Exceptions
{

    /// <inheritdoc />
    /// <summary>
    /// Mükerrer kayıt işlemlerinde kullanılacak istisna sınıfı
    /// </summary>
    public class DuplicateException : BaseApplicationException
    {
        public DuplicateException(string message) : base(message)
        {

        }
    }
}