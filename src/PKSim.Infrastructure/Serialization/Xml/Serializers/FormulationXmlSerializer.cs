using OSPSuite.Serializer;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class FormulationXmlSerializer : BuildingBlockXmlSerializer<Formulation>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.FormulationType);
         MapEnumerable(x => x.Routes, x => x.AddRoute).WithChildMappingName(CoreConstants.Serialization.Route);
      }
   }
}