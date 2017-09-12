using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_RenameAbsolutePathVisitor: ContextSpecification<IRenameAbsolutePathVisitor>
   {
      protected override void Context()
      {
         sut = new RenameAbsolutePathVisitor();
      }
   }

   public class When_reanming_a_population_simulation_that_has_advanced_parameters : concern_for_RenameAbsolutePathVisitor
   {
      private PopulationSimulation _populationSimulation;
      private AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();
         _populationSimulation= new PopulationSimulation();
         _populationSimulation.Model = new OSPSuite.Core.Domain.Model();
         _populationSimulation.Model.Root = new Container();
         var randomPopulation = new RandomPopulation();
         _populationSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("id", PKSimBuildingBlockType.Population){BuildingBlock = randomPopulation});
         _populationSimulation.SetAdvancedParameters(new AdvancedParameterCollection());
         _populationSimulation.Add(new Parameter().WithName("Param1"));
         _advancedParameter = new AdvancedParameter {ParameterPath = "OLD|Param1"};
         _populationSimulation.AddAdvancedParameter(_advancedParameter);
         _populationSimulation.Name = "NEW";
      }

      protected override void Because()
      {
         sut.RenameAllAbsolutePathIn(_populationSimulation,"OLD");
      }
      
      [Observation]
      public void should_have_renamed_the_absolute_path_in_the_advanced_parameters()
      {
         _populationSimulation.ParameterValuesCache.Has("NEW|Param1").ShouldBeTrue();
         _populationSimulation.AdvancedParameters.ElementAt(0).ParameterPath.ShouldBeEqualTo("NEW|Param1");
      }
   }
}	