using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace PKSim.Core.Snapshots.Services
{
   public interface ISnapshotSerializer
   {
      Task Serialize(object snapshot, string fileName);
      Task<object[]> DeserializeAsArray(string fileName, Type snapshotType);
   }

}