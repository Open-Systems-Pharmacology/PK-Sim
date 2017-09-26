using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace PKSim.Core.Snapshots.Services
{
   public interface ISnapshotSerializer
   {
      Task Serialize(object snapshot, string fileName);
      Task<object[]> DeserializeAsArray(string fileName, Type snapshotType);
   }

   public class SnapshotSerializer : ISnapshotSerializer
   {
      private readonly JsonSerializerSettings _settings = createSerializerSettings();

      private static JsonSerializerSettings createSerializerSettings() => new JsonSerializerSettings
      {
         TypeNameHandling = TypeNameHandling.Auto,
         NullValueHandling = NullValueHandling.Ignore,
         Converters =
         {
            new StringEnumConverter()
         },
         ContractResolver = new WritablePropertiesOnlyResolver()
      };

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

      class WritablePropertiesOnlyResolver : DefaultContractResolver
      {
         protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
         {
            var props = base.CreateProperties(type, memberSerialization);
            return props.Where(p => p.Writable).ToList();
         }
      }
   }
}