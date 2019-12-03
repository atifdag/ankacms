namespace AnkaCMS.Core.Exceptions
{

    /// <inheritdoc />
    /// <summary>
    /// Bulunamayan kayıtlar için istisna sınıfı
    /// </summary>
    public class NotFoundException : BaseApplicationException
    {
        public NotFoundException(string message) : base(message)
        {

        }
    }
}