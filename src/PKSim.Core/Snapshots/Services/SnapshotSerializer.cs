using System.IO;
using Newtonsoft.Json;

namespace PKSim.Core.Snapshots.Services
{
   public interface ISnapshotSerializer
   {
      void Serialize(object snapshot, string fileName);
      T Deserialize<T>(string fileName);
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

      public T Deserialize<T>(string fileName)
      {
         return JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName), _settings);
      }
   }
}