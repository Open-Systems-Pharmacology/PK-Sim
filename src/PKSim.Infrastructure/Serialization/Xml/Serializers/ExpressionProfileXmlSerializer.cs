using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ExpressionProfileXmlSerializer : BuildingBlockXmlSerializer<ExpressionProfile>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.Species);
         Map(x => x.Category);
         Map(x => x.Individual);
      }
   }
}