using AnkaCMS.Data.BaseEntities;

namespace AnkaCMS.Data.DataEntities
{
    public class RoleUserLine : LineBaseEntity
    {
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
