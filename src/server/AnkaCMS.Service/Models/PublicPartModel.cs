using System;
using System.Collections.Generic;
using AnkaCMS.Core.ValueObjects;

namespace AnkaCMS.Service.Models
{
    public class PublicPartModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public DateTime LastModificationTime { get; set; }
        public Guid PartId { get; set; }
        public IdCodeName Language { get; set; }
        public List<PublicContentModel> Contents { get; set; }
    }
}
