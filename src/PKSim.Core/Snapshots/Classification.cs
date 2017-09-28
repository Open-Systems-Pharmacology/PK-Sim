using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class Classification : SnapshotBase
   {
      public ClassificationType ClassificationType { set; get; }
      public Classification[] Classifications { set; get; }
   }
}