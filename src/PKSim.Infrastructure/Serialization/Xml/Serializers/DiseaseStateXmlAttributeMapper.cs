using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Serializer.Attributes;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class DiseaseStateXmlAttributeMapper : AttributeMapper<DiseaseState, SerializationContext>
   {
      public override object ConvertFrom(string attributeValue, SerializationContext context)
      {
         var diseaseStateRepository = context.Resolve<IDiseaseStateRepository>();
         return diseaseStateRepository.FindByName(attributeValue);
      }

      public override string Convert(DiseaseState diseaseState, SerializationContext context)
      {
         if (diseaseState == null || diseaseState.IsHealthy)
            return null;

         return diseaseState.Name;
      }
   }
}