using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class DistributionMetaDataXmlSerializer : BaseXmlSerializer<DistributionMetaData>
   {
      public override void PerformMapping()
      {
         Map(x => x.Mean);
         Map(x => x.Deviation);
         Map(x => x.Distribution);
      }
   }
}