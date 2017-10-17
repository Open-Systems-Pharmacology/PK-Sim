using System;
using Newtonsoft.Json;

namespace PKSim.Infrastructure.Serialization.Json
{
   public class NullabeDoubleJsonConverter : JsonConverter
   {
      private const int DOUBLE_PRECISION = 10;

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         var d = (double?) value;
         if (!d.HasValue)
            return;

         var rounded = Math.Round(d.Value, DOUBLE_PRECISION);
         writer.WriteValue(rounded);
      }

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
         //this will never be called for nullable as double reader will take precedence
         return 0;
      }

      public override bool CanRead => false;

      public override bool CanConvert(Type objectType)
      {
         return objectType == typeof(double?);
      }
   }
}