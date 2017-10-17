using System;
using Newtonsoft.Json;
using OSPSuite.Utility.Format;

namespace PKSim.Infrastructure.Serialization.Json
{
   public class NullabeDoubleJsonConverter : JsonConverter
   {
      private const int DOUBLE_PRECISION = 10;

      private readonly NumericFormatter<double> _doubleFormatter = new NumericFormatter<double>(new NumericFormatterOptions
      {
         AllowsScientificNotation = true,
         DecimalPlace = DOUBLE_PRECISION
      });

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
         var d = (double?) value;
         if (!d.HasValue)
            return;

         var formatted = _doubleFormatter.Format(d.Value);

         double.TryParse(formatted, out double roundedFromString);

         writer.WriteValue(roundedFromString);
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