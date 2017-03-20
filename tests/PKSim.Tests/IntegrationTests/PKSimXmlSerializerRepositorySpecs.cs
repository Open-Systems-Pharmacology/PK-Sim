using OSPSuite.BDDHelper;
using PKSim.Infrastructure.Serialization.Xml.Serializers;

namespace PKSim.IntegrationTests
{
   
   public class When_performing_the_mapping_for_all_available_serializers : ContextForIntegration<IPKSimXmlSerializerRepository>
   {
      [Observation]
      public void should_not_crash()
      {
         sut = new PKSimXmlSerializerRepository();
         sut.PerformMapping();
      }
   }
}