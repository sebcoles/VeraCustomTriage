﻿using System;
using System.Collections.Generic;
using System.Text;

namespace VeraCustomTriage.Shared.Models
{
    public class PropertyCondition : BaseEntity
    {
        public string Property { get; set; }
        public string Condition { get; set; }
    }
}
