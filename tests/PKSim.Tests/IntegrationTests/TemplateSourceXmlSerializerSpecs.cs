using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.IntegrationTests
{
   public class When_serializing_a_template_source : ContextForSerialization<TemplateSource>
   {
      private TemplateSource _templateSource;
      private TemplateSource _deserialized;

      protected override void Context()
      {
         base.Context();
         _templateSource = new TemplateSource("Company Library", "https://templates.company.com/lib/templates.json");
      }

      protected override void Because()
      {
         _deserialized = SerializeAndDeserialize(_templateSource);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_template_source()
      {
         _deserialized.Name.ShouldBeEqualTo(_templateSource.Name);
         _deserialized.Url.ShouldBeEqualTo(_templateSource.Url);
         _deserialized.ShouldBeEqualTo(_templateSource);
      }
   }
}
