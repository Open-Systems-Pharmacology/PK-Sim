using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public class concern_for_SimulationSelector : ContextSpecification<SimulationSelector>
   {
      protected override void Context()
      {
         sut = new SimulationSelector();
      }
   }

   public class When_finding_if_a_simulation_can_be_used_for_parameter_identification : concern_for_SimulationSelector
   {
      [Observation]
      public void individual_simulation_should_be_usable()
      {
         sut.SimulationCanBeUsedForIdentification(new IndividualSimulation()).ShouldBeTrue();
      }

      [Observation]
      public void population_simulation_should_not_be_usable()
      {
         sut.SimulationCanBeUsedForIdentification(new PopulationSimulation()).ShouldBeFalse();
      }
   }
}
