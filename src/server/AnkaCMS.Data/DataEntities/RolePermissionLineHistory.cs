using AnkaCMS.Data.BaseEntities;
using System;

namespace AnkaCMS.Data.DataEntities
{
    public class RolePermissionLineHistory : LineBaseHistoryEntity
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
