using OSPSuite.Presentation.Serialization.Extensions;
using OSPSuite.Serializer;
using PKSim.Infrastructure.Serialization.Xml.Serializers;

namespace PKSim.Presentation.Infrastructure.Serialization.Xml.Serializers
{
   public class PKSimXmlSerializerRepository : CorePKSimXmlSerializerRepository
   {
      protected override void AddInitialSerializer()
      {
         base.AddInitialSerializer();

         AttributeMapperRepository.AddAttributeMapper(new ViewLayoutXmlAttributeMapper());

         //PKSim Presentation serializers (IPKSimXmlSerializer implementations in PKSim.Presentation)
         this.AddSerializers(x =>
         {
            x.Implementing<IPKSimXmlSerializer>();
            x.InAssemblyContainingType<PKSimXmlSerializerRepository>();
            x.UsingAttributeRepository(AttributeMapperRepository);
         });

         //OSPSuite.Presentation serializer
         this.AddPresentationSerializers();
      }
   }
}