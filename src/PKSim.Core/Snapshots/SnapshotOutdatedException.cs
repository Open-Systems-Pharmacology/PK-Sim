using OSPSuite.Utility.Exceptions;
using PKSim.Assets;

namespace PKSim.Core.Snapshots
{
   public class SnapshotOutdatedException : OSPSuiteException
   {
      public SnapshotOutdatedException(string reason) : base($"{PKSimConstants.Error.SnapshotIsOutdated}\n\n{reason}")
      {
      }
   }
}