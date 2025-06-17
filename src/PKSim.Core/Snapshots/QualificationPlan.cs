using System.Collections.Generic;
using OSPSuite.Core.Snapshots;

namespace PKSim.Core.Snapshots
{
   public class QualificationPlan : SnapshotBase
   {
      public QualificationStep[] Steps { get; set; }
   }
}