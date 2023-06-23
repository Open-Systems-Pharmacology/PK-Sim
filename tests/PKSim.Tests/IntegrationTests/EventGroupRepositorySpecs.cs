using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_EventGroupRepository : ContextForIntegration<IEventGroupRepository>
   {
    }

   
   public class When_retrieving_all_event_groups_from_the_repository : concern_for_EventGroupRepository
   {
      private IEnumerable<EventGroupBuilder> _result;

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_element()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void eventgroup_should_have_main_eventgroup_subcontainer()
      {
         _result.Each(eg => eg.MainSubContainer().ShouldNotBeNull());
      }

      [Observation]
      public void eventgroup_source_criteria_should_not_be_empty()
      {
         _result.Each(eg => eg.MainSubContainer().SourceCriteria.Count().ShouldBeGreaterThan(0));
      }

      [Observation]
      public void eventgroup_should_have_at_least_one_event()
      {
         _result.Each(eg => eg.MainSubContainer().Events.Count().ShouldBeGreaterThan(0));
      }

      [Observation]
      public void eventgroup_should_have_start_time_parameter()
      {
         _result.Each(eg => eg.MainSubContainer().GetSingleChildByName<IParameter>(Constants.Parameters.START_TIME).ShouldNotBeNull());
      }
   }

}
