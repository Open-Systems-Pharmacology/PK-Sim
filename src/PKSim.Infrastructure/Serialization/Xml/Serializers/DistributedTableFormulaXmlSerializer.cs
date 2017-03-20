using OSPSuite.Core.Serialization.Xml;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class DistributedTableFormulaXmlSerializer : TableFormulaXmlSerializerBase<DistributedTableFormula>,IPKSimXmlSerializer
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.Percentile);
         MapEnumerable(x => x.AllDistributionMetaData(), x => x.AddDistributionMetaData);
      }   
   }
}