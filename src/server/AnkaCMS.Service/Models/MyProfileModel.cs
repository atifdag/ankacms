using AnkaCMS.Core.ValueObjects;
using System;
using System.Collections.Generic;

namespace AnkaCMS.Service.Models
{
    [Serializable]
    public class MyProfileModel
    {
        public UserModel UserModel { get; set; }
        public string Message { get; set; }
        public DateTime LastLoginTime { get; set; }
        public List<RootMenu> RootMenus { get; set; }
    }
}
