using AnkaCMS.Core;
using AnkaCMS.Core.ValueObjects;
using System;

namespace AnkaCMS.Service.Models
{
    public class ParameterGroupModel : IServiceModel
    {
        public Guid Id { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsApproved { get; set; }
        public int Version { get; set; }
        public DateTime CreationTime { get; set; }
        public IdCodeName Creator { get; set; }
        public DateTime LastModificationTime { get; set; }
        public IdCodeName LastModifier { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
