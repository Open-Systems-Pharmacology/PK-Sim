using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Snapshots;

namespace PKSim.Core.Snapshots
{
   public class AdvancedParameter : ParameterContainerSnapshotBase
   {
      public int Seed { get; set; }
      public DistributionType DistributionType { get; set; }
   }
}