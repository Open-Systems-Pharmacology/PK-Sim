using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class QuantityPKParameterXmlSerializer : BaseXmlSerializer<QuantityPKParameter>
   {
      public override void PerformMapping()
      {
         Map(x => x.Dimension);
         Map(x => x.Name);
         Map(x => x.QuantityPath);
         Map(x => x.Values);
      }
   }
}