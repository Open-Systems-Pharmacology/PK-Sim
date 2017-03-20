using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class SimulationPropertiesXmlSerializer : BaseXmlSerializer<SimulationProperties>
   {
      public override void PerformMapping()
      {
         Map(x => x.AllowAging);
         Map(x => x.Origin);
         Map(x => x.ModelProperties);
         MapEnumerable(x => x.CompoundPropertiesList,x=>x.AddCompoundProperties);
         Map(x => x.EventProperties);
         Map(x => x.InteractionProperties);
      }
   }
}