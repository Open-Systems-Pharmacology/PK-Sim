using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class CloneSimulationComparisonCommand : ActiveObjectUICommand<ISimulationComparison>
   {
      private readonly ISimulationComparisonTask _simulationComparisonTask;

      public CloneSimulationComparisonCommand(IActiveSubjectRetriever activeSubjectRetriever, ISimulationComparisonTask simulationComparisonTask) : base(activeSubjectRetriever)
      {
         _simulationComparisonTask = simulationComparisonTask;
      }

      protected override async void PerformExecute()
      {
         await _simulationComparisonTask.SecureAwait(x => x.CloneSimulationComparision(Subject));
      }
   }
}