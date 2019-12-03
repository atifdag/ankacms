using AnkaCMS.Data.BaseEntities;
using System;

namespace AnkaCMS.Data.DataEntities
{
    public class PermissionMenuLineHistory : LineBaseHistoryEntity
    {
        public Guid PermissionId { get; set; }
        public Guid MenuId { get; set; }
    }
}
