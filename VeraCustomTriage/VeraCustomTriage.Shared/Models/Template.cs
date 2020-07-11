using System;
using System.Collections.Generic;
using System.Text;

namespace VeraCustomTriage.Shared.Models
{
    public class Template : BaseEntity
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }
}
