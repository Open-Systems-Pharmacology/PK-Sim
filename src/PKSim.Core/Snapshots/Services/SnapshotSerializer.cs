using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace PKSim.Core.Snapshots.Services
{
   public interface ISnapshotSerializer
   {
      void Serialize(object snapshot, string fileName);
      object[] DeserializeAsArray(string fileName, Type snapshotType);
   }

   public class SnapshotSerializer : ISnapshotSerializer
   {
      private readonly JsonSerializerSettings _settings = createSerializerSettings();

      private static JsonSerializerSettings createSerializerSettings() => new JsonSerializerSettings
      {
         TypeNameHandling = TypeNameHandling.Auto,
         NullValueHandling = NullValueHandling.Ignore
      };

      public void Serialize(object snapshot, string fileName)
      {
         File.WriteAllText(fileName, JsonConvert.SerializeObject(snapshot, Formatting.Indented, _settings));
      }

      public object[] DeserializeAsArray(string fileName, Type snapshotType)
      {
         var deserializedSnapshot = JsonConvert.DeserializeObject(File.ReadAllText(fileName), _settings);

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