using AnkaCMS.Core;
using System;

namespace AnkaCMS.Data.DataEntities
{

    public class Session : IEntity
    {
        /// <inheritdoc />
        /// <summary>
        /// Birincil anahtar
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Oluþturulma zamaný
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Oluþturan kullanýcý
        /// </summary>
        public virtual User Creator { get; set; }

    }
}
