using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_RemoteTemplateRepository : ContextForIntegration<IRemoteTemplateRepository>
   {
     
   }

   public class When_loading_all_remote_templates_defined_in_pksim : concern_for_RemoteTemplateRepository
   {
      private List<RemoteTemplate> _allTemplates;

      protected override void Because()
      {
         _allTemplates = sut.All().ToList();
      }
      [Observation]
      public void should_return_a_list_of_available_templates()
      {
         _allTemplates.Any().ShouldBeTrue();
      }

      [Observation]
      public void each_loaded_template_should_have_a_valid_version_and_a_repository_url()
      {
         _allTemplates.Each(x =>
         {
            x.Version.ShouldNotBeEmpty();
            x.RepositoryUrl.ShouldNotBeEmpty();
         });
      }
   }
}