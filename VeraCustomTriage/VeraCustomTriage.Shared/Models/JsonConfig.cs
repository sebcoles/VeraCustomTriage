﻿using System;
using System.Collections.Generic;
using System.Text;

namespace VeraCustomTriage.Shared.Models
{
    public class JsonConfig
    {
        public List<CategoryRename> CategoryRename { get; set; }
        public List<Template> Template { get; set; }
        public List<AutoResponse> AutoResponse { get; set; }
    }
}
