using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v5_3;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v5_3
{
   public class When_converting_the_Simple_MoBi_531_project : ContextWithLoadedProject<Converter531To532>
   {
      private PopulationSimulation _populationSimulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Simple_MoBi_531");
         _populationSimulation = First<PopulationSimulation>();
      }

      [Observation]
      public void should_be_able_to_load_the_parameter_that_can_be_varied_in_a_population()
      {
         var allVariableInPopulation = _populationSimulation.AllPotentialAdvancedParameters.Where(x => x.CanBeVariedInPopulation).ToList();
         allVariableInPopulation.Any().ShouldBeTrue();
      }
   }
}