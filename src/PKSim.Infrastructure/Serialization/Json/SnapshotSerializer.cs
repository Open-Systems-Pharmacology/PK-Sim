using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Infrastructure.Serialization.Json
{
   public class SnapshotSerializer : ISnapshotSerializer
   {
      private readonly JsonSerializerSettings _settings = new PKSimJsonSerializerSetings();
      
      //Defines a static field as the free license only allows for a limited number of schema generation per hour
      private static readonly ConcurrentDictionary<Type, JSchema> _schemas = new ConcurrentDictionary<Type, JSchema>();

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

         var schema = validateSnapshot(snapshotType);
         var deserializedSnapshot = JsonConvert.DeserializeObject(json, _settings);

         switch (deserializedSnapshot)
         {
            case JObject jsonObject:
               return new[] {validatedObject(jsonObject, schema, snapshotType)};

            case JArray array:
               return array.Select(x => validatedObject(x, schema, snapshotType)).ToArray();
            default:
               return null;
         }
      }

      private object validatedObject(JToken jToken, JSchema schema, Type snapshotType)
      {
         if (jToken.IsValid(schema, out IList<string> errorMessages))
            return jToken.ToObject(snapshotType);

         throw new SnapshotFileMismatchException(snapshotType.Name, errorMessages);
      }

      private JSchema validateSnapshot(Type snapshotType)
      {
         return _schemas.GetOrAdd(snapshotType, createSchemaForType);
      }

      private JSchema createSchemaForType(Type snapshotType)
      {
         var generator = new JSchemaGenerator {DefaultRequired = Required.Default};
         generator.GenerationProviders.Add(new StringEnumGenerationProvider());
         return generator.Generate(snapshotType);
      }
   }
}