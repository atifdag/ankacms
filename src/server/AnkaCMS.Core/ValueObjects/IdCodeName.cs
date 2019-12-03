using System;

namespace AnkaCMS.Core.ValueObjects
{
    public struct IdCodeName
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public IdCodeName(Guid id, string code, string name)
        {
            Id = id;
            Code = code;
            Name = name;
        }
    }
}
