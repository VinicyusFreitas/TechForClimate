using System;
using System.Collections.Generic;
using System.Text;

namespace TechForClimate.Models
{
    public class Occurrence
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string RiskLevel { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
