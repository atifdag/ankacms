using AnkaCMS.Data.BaseEntities;
using System;

namespace AnkaCMS.Data.DataEntities
{
    public class ContentLanguageLineHistory : LanguageLineBaseHistoryEntity
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
        public int ViewCount { get; set; }
        public Guid ContentId { get; set; }
    }
}
