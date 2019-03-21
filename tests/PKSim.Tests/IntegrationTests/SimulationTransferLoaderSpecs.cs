using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Services;
using OSPSuite.Core.Serialization.Exchange;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SimulationTransferLoader : ContextForIntegration<ICoreLoader>
   {
      protected SimulationTransfer _simulationTransfer;

      public void LoadSimulation(string simulationfileName)
      {
         var simulationFile = DomainHelperForSpecs.DataFilePathFor($"{simulationfileName}.pkml");
         _simulationTransfer = sut.LoadSimulationTransfer(simulationFile);
      }
   }

   public class When_loading_a_simulation_transfer_file_with_chart_templates : concern_for_SimulationTransferLoader
   {
      protected override void Because()
      {
         LoadSimulation("S1_concentrBased");
      }

      [Observation]
      public void should_be_able_to_load_the_simulation()
      {
         _simulationTransfer.Simulation.ShouldNotBeNull();
      }
   }
}