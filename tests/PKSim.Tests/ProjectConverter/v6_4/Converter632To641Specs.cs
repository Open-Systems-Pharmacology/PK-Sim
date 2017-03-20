using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure;
using PKSim.Infrastructure.ProjectConverter.v6_4;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v6_4
{
   public class When_converting_632_project_with_GFR_and_tubular_secretion : ContextWithLoadedProject<Converter632To641>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("6.3.2_GFR_TS");
      }

      [Observation]
      public void should_load_individual_and_all_simulations()
      {
         var individual = First<Individual>();

         foreach (var simulation in All<Simulation>())
         {
            _lazyLoadTask.Load(simulation);
         }
      }

      [Observation]
      public void should_create_new_simulation_with_old_building_blocks()
      {
         var individual = First<Individual>();
         var compound = First<Compound>();
         var protocol = First<SimpleProtocol>();

         var simulation = DomainFactoryForSpecs.CreateSimulationWith(individual, compound, protocol) as IndividualSimulation;
      }
   }
}	