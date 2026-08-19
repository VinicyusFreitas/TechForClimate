using System;
using System.Collections.Generic;
using System.Text;

namespace TechForClimate.Models
{
    public class WeatherData
    {
        public double Temperature { get; set; }
        public double ApparentTemperature { get; set; }
        public int Humidity { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string RiskLevel { get; set; } = string.Empty;
        public string RiskColor { get; set; } = string.Empty;
    }
}
