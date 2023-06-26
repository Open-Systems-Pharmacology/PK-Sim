using OSPSuite.Core.Services;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public abstract class CreateSimulationComparisonCommand<TSimulation> : ActiveObjectUICommand<TSimulation> where TSimulation : Simulation
   {
      protected readonly ISimulationComparisonTask _simulationComparisonTask;
      protected readonly ISingleStartPresenterTask _singleStartPresenterTask;

      protected CreateSimulationComparisonCommand(ISimulationComparisonTask simulationComparisonTask, ISingleStartPresenterTask singleStartPresenterTask,
         IActiveSubjectRetriever activeSubjectRetriever) : base(activeSubjectRetriever)
      {
         _simulationComparisonTask = simulationComparisonTask;
         _singleStartPresenterTask = singleStartPresenterTask;
      }
   }

   public class CreateIndividualSimulationComparisonUICommand : CreateSimulationComparisonCommand<IndividualSimulation>
   {
      public CreateIndividualSimulationComparisonUICommand(ISimulationComparisonTask simulationComparisonTask, ISingleStartPresenterTask singleStartPresenterTask, IActiveSubjectRetriever activeSubjectRetriever)
         : base(simulationComparisonTask, singleStartPresenterTask, activeSubjectRetriever)
      {
      }

      protected override void PerformExecute()
      {
         _singleStartPresenterTask.StartForSubject(_simulationComparisonTask.CreateIndividualSimulationComparison(Subject));
      }
   }

   public class CreatePopulationSimulationComparisonUICommand : CreateSimulationComparisonCommand<PopulationSimulation>
   {
      public CreatePopulationSimulationComparisonUICommand(ISimulationComparisonTask simulationComparisonTask, ISingleStartPresenterTask singleStartPresenterTask, IActiveSubjectRetriever activeSubjectRetriever)
         : base(simulationComparisonTask, singleStartPresenterTask, activeSubjectRetriever)
      {
      }

      protected override void PerformExecute()
      {
         _singleStartPresenterTask.StartForSubject(_simulationComparisonTask.CreatePopulationSimulationComparison());
      }
   }
}