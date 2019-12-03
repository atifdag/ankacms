using AnkaCMS.Core;
using AnkaCMS.Core.ValueObjects;
using System;

namespace AnkaCMS.Service.Models
{
    public class ContentModel : IServiceModel
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
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string ContentDetail { get; set; }
        public string Url { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public string ImageFileType { get; set; }
        public int ImageFileLength { get; set; }
        public File File { get; set; }
        public int ViewCount { get; set; }
        public IdCodeName Category { get; set; }
        public Guid ContentId { get; set; }
        public IdCodeName Language { get; set; }

    }
}
