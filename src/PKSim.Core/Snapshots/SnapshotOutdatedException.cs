using System;
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

   public class SnapshotParameterNotFoundException : SnapshotOutdatedException
   {
      public SnapshotParameterNotFoundException(string parameterName, string container) : base(PKSimConstants.Error.SnapshotParameterNotFoundInContainer(parameterName, container))
      {
      }
   }
}