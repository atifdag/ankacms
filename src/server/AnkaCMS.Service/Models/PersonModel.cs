using AnkaCMS.Core;
using AnkaCMS.Core.ValueObjects;
using System;

namespace AnkaCMS.Service.Models
{
    public class PersonModel : IServiceModel
    {
        public Guid Id { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsApproved { get; set; }
        public int Version { get; set; }
        public DateTime CreationTime { get; set; }
        public IdCodeName Creator { get; set; }
        public DateTime LastModificationTime { get; set; }
        public IdCodeName LastModifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentityCode { get; set; }
        public DateTime BirthDate { get; set; }
        public string DisplayName => FirstName + " " + LastName;

    }
}
