using AnkaCMS.Data.BaseEntities;
using System;

namespace AnkaCMS.Data.DataEntities
{

    public class ContentHistory : LanguageBaseHistoryEntity
    {
        public Guid CategoryId { get; set; }

    }
}
