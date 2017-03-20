using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class AdvancedParameterXmlSerializer : PKSimContainerXmlSerializer<AdvancedParameter> 
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.ParameterPath);
         Map(x => x.Seed);
      }
   }
}