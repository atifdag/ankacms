using System;
using AnkaCMS.Core.ValueObjects;

namespace AnkaCMS.Service.Models
{
    public class PublicContentModel
    {
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
        public DateTime LastModificationTime { get; set; }
        public IdCodeName Category { get; set; }
        public Guid ContentId { get; set; }
        public IdCodeName Language { get; set; }
    }
}
