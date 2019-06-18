using System;
using System.Threading.Tasks;

namespace PKSim.Core.Services
{
   public interface IJsonSerializer
   {
      Task Serialize(object objectToSerialize, string fileName);
      Task<object[]> DeserializeAsArray(string fileName, Type objectType);
      Task<object[]> DeserializeAsArrayFromString(string jsonString, Type objectType);
      Task<object> Deserialize(string fileName, Type objectType);
      Task<T> Deserialize<T>(string fileName) where T : class;
      Task<object> DeserializeFromString(string jsonString, Type objectType);
      Task<T> DeserializeFromString<T>(string jsonString) where T : class;
   }
}