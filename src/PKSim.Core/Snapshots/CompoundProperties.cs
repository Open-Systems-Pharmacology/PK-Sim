using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using CalculationMethodCache = OSPSuite.Core.Snapshots.CalculationMethodCache;

namespace PKSim.Core.Snapshots
{
   public class CompoundProperties : IWithName
   {
      [Required] public string Name { get; set; }

      public CalculationMethodCache CalculationMethods { get; set; }
      public CompoundGroupSelection[] Alternatives { get; set; }
      public CompoundProcessSelection[] Processes { get; set; }
      public ProtocolSelection Protocol { get; set; }
   }
}