using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class RunSimulationCommand : ActiveObjectUICommand<Simulation>
   {
      private readonly IInteractiveSimulationRunner _simulationRunner;
      private readonly bool _selectOutput;

      public RunSimulationCommand(IInteractiveSimulationRunner simulationRunner, IActiveSubjectRetriever activeSubjectRetriever, bool selectOutput = false) : base(activeSubjectRetriever)
      {
         _simulationRunner = simulationRunner;
         _selectOutput = selectOutput;
      }

      protected override async void PerformExecute()
      {
         await _simulationRunner.SecureAwait(x => x.RunSimulation(Subject, _selectOutput));
      }
   }

   public class RunSimulationWithSettingsCommand : RunSimulationCommand
   {
      public RunSimulationWithSettingsCommand(IInteractiveSimulationRunner simulationRunner, IActiveSubjectRetriever activeSubjectRetriever) : base(simulationRunner, activeSubjectRetriever, true)
      {
      }
   }
}