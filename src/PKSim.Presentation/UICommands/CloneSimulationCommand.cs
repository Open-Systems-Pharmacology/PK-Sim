using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class CloneSimulationCommand : ActiveObjectUICommand<Simulation>
   {
      private readonly ICloneSimulationTask _simulationTask;

      public CloneSimulationCommand(ICloneSimulationTask simulationTask, IActiveSubjectRetriever activeSubjectRetriever) : base(activeSubjectRetriever)
      {
         _simulationTask = simulationTask;
      }

      protected override void PerformExecute()
      {
         _simulationTask.Clone(Subject);
      }
   }
}