using AnkaCMS.Data.BaseEntities;

namespace AnkaCMS.Data.DataEntities
{
    public class PartContentLine : LineBaseEntity
    {
        public virtual Part Part { get; set; }
        public virtual Content Content { get; set; }
    }
}
