using AnkaCMS.Data.BaseEntities;
using System;

namespace AnkaCMS.Data.DataEntities
{
    public class UserHistory : BaseHistoryEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public Guid PersonId { get; set; }
    }
}
