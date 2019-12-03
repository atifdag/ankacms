namespace AnkaCMS.Core.Exceptions
{
    public class ParentNotFoundException : BaseApplicationException
    {

        /// <inheritdoc />
        /// <summary>
        /// Üst bağı bulunamayan kayıtlar için istisna sınıfı
        /// </summary>

        public ParentNotFoundException(string message) : base(message)
        {

        }
    }
}