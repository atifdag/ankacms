using AnkaCMS.Core;
using AnkaCMS.Core.ValueObjects;
using System;
using System.Collections.Generic;

namespace AnkaCMS.Service.Models
{
    public class PartModel : IServiceModel
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
        public string Keywords { get; set; }
        public Guid PartId { get; set; }
        public IdCodeName Language { get; set; }
        public List<IdCodeNameSelected> Contents { get; set; }

    }
}
