using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class FormulationMappingXmlSerializer : BaseXmlSerializer<FormulationMapping>
   {
      public override void PerformMapping()
      {
         Map(x => x.TemplateFormulationId);
         Map(x => x.FormulationKey);
      }
   }
}