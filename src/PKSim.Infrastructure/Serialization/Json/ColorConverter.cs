﻿using System;
using System.Drawing;
using Newtonsoft.Json;
using OSPSuite.Core.Extensions;

namespace PKSim.Infrastructure.Serialization.Json
{
   class ColorConverter : JsonConverter
   {
      public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
      {
         if (value == null)
            return;

         var c = (Color) value;
         var hexString = $"{c.R:X2}{c.G:X2}{c.B:X2}";
         // Only add hex transparency if required as it does not seem to be widely supported by color readers.
         var preFix = c.A == 255 ? "#" : $"#{c.A:X2}";
         hexString = $"{preFix}{hexString}";
         writer.WriteValue(hexString);
      }

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
      {
         //this will never be called as default color reader will take precedence
         return existingValue;
      }

      public override bool CanRead => true;

      public override bool CanConvert(Type objectType)
      {
         return objectType.IsOneOf(typeof(Color), typeof(Color?));
      }
   }
}