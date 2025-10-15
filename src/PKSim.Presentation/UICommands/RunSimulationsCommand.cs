using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class RunSimulationsCommand : ObjectUICommand<IReadOnlyList<Simulation>>
   {
      private readonly IInteractiveSimulationRunner _simulationRunner;

      public RunSimulationsCommand(IInteractiveSimulationRunner simulationRunner)
      {
         _simulationRunner = simulationRunner;
      }

      protected override async void PerformExecute()
      {
         await _simulationRunner.SecureAwait(x => runAllSimulations(Subject, x));
      }

      private Task runAllSimulations(IReadOnlyList<Simulation> simulations, IInteractiveSimulationRunner simulationRunner)
      {
         return Task.WhenAll(simulations.Select(simulation => simulationRunner.RunSimulation(simulation, selectOutput:false)));
      }

   }
}