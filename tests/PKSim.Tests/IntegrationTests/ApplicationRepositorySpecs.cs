using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ApplicationRepository : ContextForIntegration<IApplicationRepository>
   {
   }

   
   public class When_retrieving_all_applications_from_the_repository : concern_for_ApplicationRepository
   {
      private IEnumerable<IApplicationBuilder> _result;

      protected override void Because()
      {
         _result = sut.All();
         _result = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_element()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void every_application_should_have_protocol_schema_item_container()
      {
         foreach (IApplicationBuilder applicBuilder in _result)
            applicBuilder.ProtocolSchemaItemContainer().ShouldNotBeNull();
      }

      [Observation]
      public void every_application_except_oral_and_bolus_should_have_application_rate_parameter()
      {
         foreach (IApplicationBuilder applicBuilder in _result)
         {
            if (applicBuilder.Name.StartsWith("Oral")||applicBuilder.Name.Equals(CoreConstants.Application.Name.IntravenousBolus))
               continue;
            applicBuilder.GetSingleChildByName<IParameter>(CoreConstants.Parameters.APPLICATION_RATE).ShouldNotBeNull();
         }
      }

      [Observation]
      public void application_transport_should_have_nonempty_source_criteria()
      {
         _result.Each(app => app.Transports.Each(t => t.SourceCriteria.Count().ShouldBeGreaterThan(0)));
      }

      [Observation]
      public void application_transport_that_is_not_user_defiend_should_have_nonempty_target_criteria()
      {
         _result.Where(x=>!x.Name.StartsWith(ApplicationTypes.UserDefined.Name))
                .Each(app => app.Transports.Each(t => t.TargetCriteria.Count().ShouldBeGreaterThan(0)));
      }

      [Observation]
      public void application_transport_should_have_application_tag_as_part_of_source_criteria()
      {
         _result.Each(app => app.Transports.Each(
            t => t.SourceCriteria.Contains(
               new MatchTagCondition(CoreConstants.Tags.APPLICATION)).ShouldBeTrue()));
      }

      [Observation]
      public void application_source_criteria_should_not_be_empty()
      {
         _result.Each(app => app.SourceCriteria.Count().ShouldBeGreaterThan(0));
      }

      [Observation]
      public void application_should_have_at_least_one_event()
      {
         _result.Each(app => app.Events.Count().ShouldBeGreaterThan(0));
      }
   }

}	