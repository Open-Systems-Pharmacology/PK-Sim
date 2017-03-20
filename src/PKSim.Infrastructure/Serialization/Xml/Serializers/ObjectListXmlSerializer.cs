using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class GroupingItemListXmlSerializer : ObjectListXmlSerializer<GroupingItem>, IPKSimXmlSerializer
   {
   }

   public class CurveSelectionListXmlSerializer : ObjectListXmlSerializer<StatisticalAggregation>,IPKSimXmlSerializer
   {
   }
}