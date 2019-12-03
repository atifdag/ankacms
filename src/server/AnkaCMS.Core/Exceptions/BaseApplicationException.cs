using System;

namespace AnkaCMS.Core.Exceptions
{

    /// <inheritdoc />
    /// <summary>
    /// Uygulama istisna işlemleri için temel sınıf
    /// </summary>
    public abstract class BaseApplicationException : ApplicationException
    {
        protected BaseApplicationException(string message) : base(message)
        {

        }
    }
}
