using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v5_3;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_3
{
   public class When_converting_the_Simple_Population_531_project : ContextWithLoadedProject<Converter531To532>
   {
      private Individual _individual;
      private PopulationSimulation _populationSimulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Simple_Population_531");
         _individual = First<Individual>();
         _populationSimulation = First<PopulationSimulation>();
      }

      [Observation]
      public void should_not_have_updated_the_can_be_varied_flag_in_the_individual()
      {
         var allVariableInPopulation = _individual.GetAllChildren<IParameter>(x => x.CanBeVariedInPopulation);
         var allVariable = _individual.GetAllChildren<IParameter>(x => x.CanBeVaried);
         allVariableInPopulation.Any().ShouldBeTrue();
         //a 5.3.1 project created from scratch should have different can be varied and can be varied in population count
         allVariable.Count.ShouldNotBeEqualTo(allVariableInPopulation.Count);
      }

      [Observation]
      public void should_not_have_updated_the_can_be_varied_flag_in_the_simulation()
      {
         var allVariableInPopulation = _populationSimulation.AllPotentialAdvancedParameters.Where(x => x.CanBeVariedInPopulation).ToList();
         var allVariable = _populationSimulation.AllPotentialAdvancedParameters.Where(x => x.CanBeVaried).ToList();
         allVariableInPopulation.Any().ShouldBeTrue();
         //a 5.3.1 project created from scratch should have different can be varied and can be varied in population count
         allVariable.Count.ShouldNotBeEqualTo(allVariableInPopulation.Count);
      }
   }
}