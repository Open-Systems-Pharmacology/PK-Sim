using System;
using System.Threading.Tasks;

namespace PKSim.Core.Snapshots.Services
{
   public interface ISnapshotSerializer
   {
      Task Serialize(object snapshot, string fileName);
      Task<object[]> DeserializeAsArray(string fileName, Type snapshotType);
   }
}