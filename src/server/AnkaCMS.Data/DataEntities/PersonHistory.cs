using AnkaCMS.Data.BaseEntities;
using System;

namespace AnkaCMS.Data.DataEntities
{
    public class PersonHistory : BaseHistoryEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentityCode { get; set; }
        public string Biography { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
