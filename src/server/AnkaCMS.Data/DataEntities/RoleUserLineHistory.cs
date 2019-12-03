using AnkaCMS.Data.BaseEntities;
using System;

namespace AnkaCMS.Data.DataEntities
{
    public class RoleUserLineHistory : LineBaseHistoryEntity
    {
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
    }
}
