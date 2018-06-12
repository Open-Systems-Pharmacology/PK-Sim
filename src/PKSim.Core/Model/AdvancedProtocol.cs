using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Model
{
   public class AdvancedProtocol : Protocol
   {
      public override Unit TimeUnit { get; set; }

      public virtual IEnumerable<Schema> AllSchemas => GetChildren<Schema>();

      public virtual void AddSchema(Schema schema) => Add(schema);

      public virtual void RemoveAllSchemas()
      {
         foreach (var schema in AllSchemas.ToList())
         {
            RemoveSchema(schema);
         }
      }

      public virtual bool Contains(Schema schema) => AllSchemas.Contains(schema);

      public virtual void RemoveSchema(Schema schema) => RemoveChild(schema);

      public override IEnumerable<string> UsedFormulationKeys => 
         AllSchemas.SelectMany(x => x.SchemaItems, (schema, item) => item.FormulationKey).Distinct();

      public override ApplicationType ApplicationTypeUsing(string formulationKey)
      {
         return (from allSchema in AllSchemas
                 from item in allSchema.SchemaItems
                 where Equals(item.FormulationKey, formulationKey)
                 select item.ApplicationType).First();
      }

      public override double EndTime => AllSchemas.Max(x => x.EndTime);
   }
}