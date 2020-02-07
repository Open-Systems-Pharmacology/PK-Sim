using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Serializer.Attributes;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ModelConfigurationXmlAttributeMapper : AttributeMapper<ModelConfiguration, SerializationContext>
   {
      public override string Convert(ModelConfiguration modelConfiguration, SerializationContext context)
      {
         if (modelConfiguration == null)
            return string.Empty;

         return modelConfiguration.Id;
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         var modelConfigurationRepository = context.Resolve<IModelConfigurationRepository>();
         return modelConfigurationRepository.FindById(attributeValue);
      }
   }
}