using AnkaCMS.Data.BaseEntities;
using System;

namespace AnkaCMS.Data.DataEntities
{
    public class PartContentLineHistory : LineBaseHistoryEntity
    {
        public Guid PartId { get; set; }
        public Guid ContentId { get; set; }
    }
}
