using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class ParameterIdentificationRunMode : SnapshotBase
   {
      public int? NumberOfRuns { get; set; }
      public CalculationMethodCache AllTheSameSelection { get; set; }
      public Dictionary<string, CalculationMethodCache> CalculationMethods { get; set; }
   }
}