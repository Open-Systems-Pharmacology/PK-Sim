using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SchemaFactory : ContextForIntegration<ISchemaFactory>
   {
      protected ISimulationActiveProcessRepository _simulationActProcRepo;
      protected IFlatProcessRepository _flatProcessRepository;

      protected override void Context()
      {
         sut = IoC.Resolve<ISchemaFactory>();
      }
   }

   public class When_creating_a_schema_item_from_the_factory : concern_for_SchemaFactory
   {
      private Schema _schema;

      protected override void Because()
      {
         _schema = sut.Create();
      }

      [Observation]
      public void should_ensure_that_all_parameters_are_marked_as_set_by_user()
      {
         _schema.Parameter(Constants.Parameters.START_TIME).IsDefault.ShouldBeFalse();
         _schema.Parameter(CoreConstants.Parameters.NUMBER_OF_REPETITIONS).IsDefault.ShouldBeFalse();
         _schema.Parameter(CoreConstants.Parameters.TIME_BETWEEN_REPETITIONS).IsDefault.ShouldBeFalse();
      }

      [Observation]
      public void should_have_set_the_expected_default_value()
      {
         _schema.Parameter(Constants.Parameters.START_TIME).Value.ShouldBeEqualTo(0);
         _schema.Parameter(CoreConstants.Parameters.NUMBER_OF_REPETITIONS).Value.ShouldBeEqualTo(1);
         _schema.Parameter(CoreConstants.Parameters.TIME_BETWEEN_REPETITIONS).Value.ShouldBeEqualTo(0);
      }
   }
}