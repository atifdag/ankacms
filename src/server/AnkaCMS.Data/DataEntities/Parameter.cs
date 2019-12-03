using AnkaCMS.Data.BaseEntities;

namespace AnkaCMS.Data.DataEntities
{
    public class Parameter : BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool Erasable { get; set; }
        public string Description { get; set; }
        public virtual ParameterGroup ParameterGroup { get; set; }
    }
}
