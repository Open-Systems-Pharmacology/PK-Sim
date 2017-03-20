using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ModelPropertiesXmlSerializer : BaseXmlSerializer<ModelProperties>
   {
      public override void PerformMapping()
      {
         Map(x => x.ModelConfiguration);
         Map(x => x.CalculationMethodCache);
      }
   }
}