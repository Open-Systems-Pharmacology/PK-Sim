using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Infrastructure.Serialization.Json
{
   public class SnapshotSerializer : ISnapshotSerializer
   {
      private readonly JsonSerializerSettings _settings = new PKSimJsonSerializerSetings();

      public async Task Serialize(object snapshot, string fileName)
      {
         var data = JsonConvert.SerializeObject(snapshot, Formatting.Indented, _settings);

         using (var sw = new StreamWriter(fileName))
         {
            await sw.WriteAsync(data);
         }
      }

      public async Task<object[]> DeserializeAsArray(string fileName, Type snapshotType)
      {
         string json;
         using (var reader = new StreamReader(fileName))
         {
            json = await reader.ReadToEndAsync();
         }

         var deserializedSnapshot = JsonConvert.DeserializeObject(json, _settings);
         //TODO. We should validate that the json actually matches our type. If not, we could throw an explicit error
         //This should be done using  /jsonschema
         switch (deserializedSnapshot)
         {
            case JObject jsonObject:
               return new[] {jsonObject.ToObject(snapshotType)};
            case JArray array:
               return array.Select(x => x.ToObject(snapshotType)).ToArray();
            default:
               return null;
         }
      }
   }
}