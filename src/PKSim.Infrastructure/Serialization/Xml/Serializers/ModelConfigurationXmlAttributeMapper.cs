using OSPSuite.Serializer.Attributes;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ModelConfigurationXmlAttributeMapper : AttributeMapper<ModelConfiguration, SerializationContext>
   {
      private readonly IModelConfigurationRepository _modelConfigurationRepository;

      public ModelConfigurationXmlAttributeMapper() : this(IoC.Resolve<IModelConfigurationRepository>())
      {
      }

      public ModelConfigurationXmlAttributeMapper(IModelConfigurationRepository modelConfigurationRepository)
      {
         _modelConfigurationRepository = modelConfigurationRepository;
      }

      public override string Convert(ModelConfiguration modelConfiguration, SerializationContext context)
      {
         if (modelConfiguration == null)
            return string.Empty;

         return modelConfiguration.Id;
      }

      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         return _modelConfigurationRepository.FindById(attributeValue);
      }
   }
}