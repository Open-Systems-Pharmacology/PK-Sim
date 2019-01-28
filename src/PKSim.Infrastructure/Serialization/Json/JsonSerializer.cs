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
using OSPSuite.Utility.Extensions;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;

namespace PKSim.Infrastructure.Serialization.Json
{
   public class JsonSerializer : IJsonSerializer
   {
      private readonly JsonSerializerSettings _settings = new PKSimJsonSerializerSetings();

      //Defines a static field as the free license only allows for a limited number of schema generation per hour
      private static readonly ConcurrentDictionary<Type, JSchema> _schemas = new ConcurrentDictionary<Type, JSchema>();

      public async Task Serialize(object objectToSerialize, string fileName)
      {
         var data = JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented, _settings);

         using (var sw = new StreamWriter(fileName))
         {
            await sw.WriteAsync(data);
         }
      }

      public async Task<object[]> DeserializeAsArray(string fileName, Type objectType)
      {
         string json;
         using (var reader = new StreamReader(fileName))
         {
            json = await reader.ReadToEndAsync();
         }

         return deserializeAsArrayFromString(json, objectType);
      }

      public Task<object[]> DeserializeAsArrayFromString(string jsonString, Type objectType) =>
         Task.FromResult(deserializeAsArrayFromString(jsonString, objectType));
     

      private object[]  deserializeAsArrayFromString(string json, Type objectType)
      {
         var schema = validateSnapshot(objectType);
         var deserializedSnapshot = JsonConvert.DeserializeObject(json, _settings);

         switch (deserializedSnapshot)
         {
            case JObject jsonObject:
               return new[] { validatedObject(jsonObject, schema, objectType) };

            case JArray array:
               return array.Select(x => validatedObject(x, schema, objectType)).ToArray();
            default:
               return null;
         }
      }
      public async Task<object> Deserialize(string fileName, Type objectType)
      {
         var deserializedObjects = await DeserializeAsArray(fileName, objectType);
         return deserializedObjects.FirstOrDefault();
      }

      public async Task<T> Deserialize<T>(string fileName) where T : class
      {
         var deserializedObject = await Deserialize(fileName, typeof(T));
         return deserializedObject as T;
      }

      public async Task<object> DeserializeFromString(string jsonString, Type objectType)
      {
         var deserializedObjects = await DeserializeAsArrayFromString(jsonString, objectType);
         return deserializedObjects.FirstOrDefault();
      }

      public async Task<T> DeserializeFromString<T>(string jsonString) where T:class
      {
         var deserializedObject = await DeserializeFromString(jsonString, typeof(T));
         return deserializedObject as T;
      }

      private object validatedObject(JToken jToken, JSchema schema, Type snapshotType)
      {
         if (!requiresSchemaValidation(snapshotType))
            return jToken.ToObject(snapshotType);

         if (jToken.IsValid(schema, out IList<string> errorMessages))
            return jToken.ToObject(snapshotType);

         throw new SnapshotFileMismatchException(snapshotType.Name, errorMessages);
      }

      private bool requiresSchemaValidation(Type snapshotType)
      {
         return snapshotType.IsAnImplementationOf<Compound>();
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