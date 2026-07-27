using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure;
using PKSim.Infrastructure.ProjectConverter.v13;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v13
{
   public class When_creating_a_simulation_using_the_building_blocks_of_the_simple_project_730_project : ContextWithLoadedProject<Converter12To13>
   {
      private Individual _individual;
      private Compound _compound;
      private Protocol _protocol;
      private Simulation _simulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimpleProject_730");
         _individual = FindByName<Individual>("Ind");
         _compound = FindByName<Compound>("Caffeine");
         _protocol = FindByName<Protocol>("IV");
      }

      protected override void Context()
      {
         base.Context();
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol);
      }

      [Observation]
      public void should_be_able_to_create_a_simulation_with_the_converted_building_blocks()
      {
         _simulation.Model.ShouldNotBeNull();
      }
   }
}
