using PKSim.Core.Model;

namespace PKSim.Core.Snapshots
{
   public class CompoundProperties
   {
      public string Name { get; set; }
      public CalculationMethodCache CalculationMethods { get; set; }
      public CompoundGroupSelection[] Alternatives { get; set; }
      public CompoundProcessSelection[] Processes { get; set; }
      public ProtocolSelection Protocol { get; set; }
   }
}