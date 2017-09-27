using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class RunSimulationCommand : ActiveObjectUICommand<Simulation>
   {
      private readonly ISimulationRunner _simulationRunner;
      private readonly bool _defineSetting;

      public RunSimulationCommand(ISimulationRunner simulationRunner, IActiveSubjectRetriever activeSubjectRetriever, bool defineSetting = false) : base(activeSubjectRetriever)
      {
         _simulationRunner = simulationRunner;
         _defineSetting = defineSetting;
      }

      protected override async void PerformExecute()
      {
         await _simulationRunner.SecureAwait(x => x.RunSimulation(Subject, _defineSetting));
      }
   }

   public class RunSimulationWithSettingsCommand : RunSimulationCommand
   {
      public RunSimulationWithSettingsCommand(ISimulationRunner simulationRunner, IActiveSubjectRetriever activeSubjectRetriever) : base(simulationRunner, activeSubjectRetriever, true)
      {
      }
   }
}