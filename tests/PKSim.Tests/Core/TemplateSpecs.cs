using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_TemplateSpecs : ContextSpecification<Template>
   {
      protected RemoteTemplate _remoteTemplate;

      protected override void Context()
      {
         base.Context();
         _remoteTemplate = new RemoteTemplate
         {
            Type = TemplateType.Individual,
         };
      }
   }

   public class When_checking_if_a_remote_template_is_valid_for_a_given_version : concern_for_TemplateSpecs
   {
      [Observation]
      public void should_return_true_if_no_version_is_defined_for_the_template()
      {
         _remoteTemplate.IsSupportedByCurrentVersion("10.0").ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_if_there_is_a_min_version_defined_for_the_template_smaller_than_current()
      {
         _remoteTemplate.MinVersion = "9.2";
         _remoteTemplate.IsSupportedByCurrentVersion("10.0").ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_there_is_a_min_version_defined_for_the_template_bigger_than_current()
      {
         _remoteTemplate.MinVersion = "11.2";
         _remoteTemplate.IsSupportedByCurrentVersion("10.0").ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_there_is_a_max_version_defined_for_the_template_bigger_than_current()
      {
         _remoteTemplate.MaxVersion = "11.0";
         _remoteTemplate.IsSupportedByCurrentVersion("10.0").ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_there_is_a_max_version_defined_for_the_template_smaller_than_current()
      {
         _remoteTemplate.MaxVersion = "9.0";
         _remoteTemplate.IsSupportedByCurrentVersion("10.0").ShouldBeFalse();
      }
   }
}