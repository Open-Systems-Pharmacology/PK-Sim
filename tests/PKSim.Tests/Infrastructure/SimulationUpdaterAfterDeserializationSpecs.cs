using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SimulationUpdaterAfterDeserialization : ContextSpecification<ISimulationUpdaterAfterDeserialization>
   {
      protected IParameterIdUpdater _parameterIdUpdater;
      private IReferencesResolver _referenceResolver;

      protected override void Context()
      {
         _parameterIdUpdater = A.Fake<IParameterIdUpdater>();
         _referenceResolver= A.Fake<IReferencesResolver>();
         sut = new SimulationUpdaterAfterDeserialization(_parameterIdUpdater,_referenceResolver);
      }
   }

   public class When_updating_a_simulation : concern_for_SimulationUpdaterAfterDeserialization
   {
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<IndividualSimulation>();
      }

      protected override void Because()
      {
         sut.UpdateSimulation(_simulation);
      }

      [Observation]
      public void should_have_set_the_simulation_id_in_all_parameters()
      {
         A.CallTo(() => _parameterIdUpdater.UpdateSimulationId(_simulation)).MustHaveHappened();
      }
   }

   public class When_updating_an_undefined_simulation : concern_for_SimulationUpdaterAfterDeserialization
   {
      protected override void Because()
      {
         sut.UpdateSimulation(null);
      }

      [Observation]
      public void should_not_do_anything()
      {
         A.CallTo(() => _parameterIdUpdater.UpdateSimulationId(A<Simulation>._)).MustNotHaveHappened();
      }
   }
}