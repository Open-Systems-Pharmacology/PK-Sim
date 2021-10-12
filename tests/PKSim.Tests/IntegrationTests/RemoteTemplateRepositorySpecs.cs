using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_RemoteTemplateRepository : ContextForIntegration<IRemoteTemplateRepository>
   {
     
   }

   public class When_loading_all_remote_templates_defined_in_pksim : concern_for_RemoteTemplateRepository
   {
      [Observation]
      public void should_return_a_list_of_available_templates()
      {
         sut.All().Any().ShouldBeTrue();
      }
   }
}