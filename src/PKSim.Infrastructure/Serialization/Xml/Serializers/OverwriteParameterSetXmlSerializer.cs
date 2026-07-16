using OSPSuite.Core.Serialization.Xml;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class OverwriteParameterSetXmlSerializer : ObjectBaseXmlSerializer<OverwriteParameterSet>, IPKSimXmlSerializer
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.IsDefault);
         MapEnumerable(x => x.ExtendedProperties, x => x.ExtendedProperties.Add);
         MapEnumerable(x => x.ParameterValues, x => x.Add);
      }
   }
}
