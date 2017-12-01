using System.Collections.Generic;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;

namespace PKSim.Core.Snapshots
{
   public class SnapshotOutdatedException : OSPSuiteException
   {
      public SnapshotOutdatedException(string reason) : base($"{PKSimConstants.Error.SnapshotIsOutdated}\n\n{reason}")
      {
      }
   }

   public class SnapshotFileMismatchException : OSPSuiteException
   {
      public SnapshotFileMismatchException(string desiredType, IEnumerable<string> reasons) : base($"{PKSimConstants.Error.SnapshotFileMismatch(desiredType)}\n\n{reasons.ToString("\n")}")
      {
      }
   }
}