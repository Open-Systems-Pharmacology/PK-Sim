using System;
using PKSim.Assets;

namespace PKSim.Core.Snapshots
{
   public class SnapshotNotFoundException : Exception
   {
      public SnapshotNotFoundException(Type modelType) : base(PKSimConstants.Error.SnapshotNotFoundFor(modelType.FullName))
      {
      }
   }
}