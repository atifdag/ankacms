﻿using System;
using System.Collections.Generic;

namespace AnkaCMS.Core.ValueObjects
{
    public class MenuObject
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public virtual ICollection<MenuObject> ChildMenus { get; set; }
    }
}
