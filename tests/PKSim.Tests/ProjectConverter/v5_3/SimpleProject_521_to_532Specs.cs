using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v5_3;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_3
{
   public class When_converting_the_simple_project_5_2_1_to_5_3_2 : ContextWithLoadedProject<Converter531To532>
   {
      private Individual _individual;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimpleProject_521");
         _individual = First<Individual>();
      }

      [Observation]
      public void should_have_updated_the_can_be_varied_in_population_flag_in_all_can_be_varied_parameter()
      {
         var allVariableInPopulation = _individual.GetAllChildren<IParameter>(x => x.CanBeVariedInPopulation);
         allVariableInPopulation.Any().ShouldBeTrue();
         allVariableInPopulation.Count.ShouldBeGreaterThan(50);
      }
   }
}