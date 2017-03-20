using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class SchemaItemXmlSerializer : PKSimContainerXmlSerializer<SchemaItem>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.ApplicationType);
         Map(x => x.FormulationKey);
         Map(x => x.TargetOrgan);
         Map(x => x.TargetCompartment);
      }
   }
}