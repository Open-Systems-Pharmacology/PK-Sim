using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class IndividualXmlSerializer : BuildingBlockXmlSerializer<Individual>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.OriginData);
         Map(x => x.Seed);

      }
   }
}