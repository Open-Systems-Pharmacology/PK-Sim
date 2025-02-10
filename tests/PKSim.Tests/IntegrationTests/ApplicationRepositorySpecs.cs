using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ApplicationRepository : ContextForIntegration<IApplicationRepository>
   {
   }

   public class When_retrieving_all_applications_from_the_repository : concern_for_ApplicationRepository
   {
      private IEnumerable<ApplicationBuilder> _result;

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
         _result.Each(x => x.ProtocolSchemaItemContainer().ShouldNotBeNull());
      }

      [Observation]
      public void every_application_except_oral_and_bolus_should_have_application_rate_parameter()
      {
         foreach (var applicationBuilder in _result)
         {
            if (applicationBuilder.Name.StartsWith("Oral") || applicationBuilder.IsNamed(CoreConstants.Application.Name.IntravenousBolus))
               continue;

            applicationBuilder.Parameter(CoreConstants.Parameters.APPLICATION_RATE).ShouldNotBeNull();
         }
      }

      [Observation]
      public void application_transport_should_have_nonempty_source_criteria()
      {
         _result.Each(app => app.Transports.Each(t => t.SourceCriteria.Count.ShouldBeGreaterThan(0)));
      }

      [Observation]
      public void application_transport_that_is_not_user_defined_should_have_nonempty_target_criteria()
      {
         _result.Where(x => !x.Name.StartsWith(ApplicationTypes.UserDefined.Name))
            .Each(app => app.Transports.Each(t => t.TargetCriteria.Count.ShouldBeGreaterThan(0)));
      }

      [Observation]
      public void application_transport_should_have_application_tag_as_part_of_source_criteria()
      {
         _result.Each(app => app.Transports.Each(
            t => t.SourceCriteria.Contains(
               new MatchTagCondition(CoreConstants.Tags.APPLICATION)).ShouldBeTrue()));
      }

      [Observation]
      public void application_should_have_at_least_one_event()
      {
         _result.Each(app => app.Events.Count().ShouldBeGreaterThan(0));
      }
   }
}