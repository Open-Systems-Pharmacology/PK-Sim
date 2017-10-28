using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKSim.Core.Model
{
    public class PopulationAgeSettings
    {
        public string Population { get; set; }
        public double DefaultAge { get; set; }
        public double MinAge { get; set; }
        public double MaxAge { get; set; }
        public string DefaultAgeUnit { get; set; }
    }
}
