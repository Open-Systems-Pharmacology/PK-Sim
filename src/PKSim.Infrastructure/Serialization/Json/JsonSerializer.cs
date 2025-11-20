using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Snapshots;

namespace PKSim.Infrastructure.Serialization.Json
{
   public class JsonSerializer : OSPSuite.Infrastructure.Serialization.Json.JsonSerializer
   {
      public JsonSerializer()
      {
         
      }
      
      protected override object ValidatedObject(JToken jToken, JSchema schema, Type snapshotType)
      {
         if (!requiresSchemaValidation(snapshotType))
            return jToken.ToObject(snapshotType);

         return base.ValidatedObject(jToken, schema, snapshotType);
      }

      private bool requiresSchemaValidation(Type snapshotType)
      {
         return snapshotType.IsAnImplementationOf<Compound>();
      }
   }
}