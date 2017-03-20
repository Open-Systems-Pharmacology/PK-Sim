using PKSim.Core.Model;
using PKSim.Presentation.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class ConfigureSimulationCommand : ActiveObjectUICommand<Simulation>
   {
      private readonly IConfigureSimulationTask _simulationTask;

      public ConfigureSimulationCommand(IConfigureSimulationTask simulationTask, IActiveSubjectRetriever activeSubjectRetriever)
         : base(activeSubjectRetriever)
      {
         _simulationTask = simulationTask;
      }

      protected override void PerformExecute()
      {
         _simulationTask.Configure(Subject);
      }
   }
}