using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class GroupingItemXmlSerializer : BaseXmlSerializer<GroupingItem>
   {
      public override void PerformMapping()
      {
         Map(x => x.Color);
         Map(x => x.Symbol);
         Map(x => x.Label);
      }
   }
}