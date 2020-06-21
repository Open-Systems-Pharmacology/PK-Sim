using System;
using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public abstract class CreateSimulationComparisonCommand : IUICommand
   {
      private readonly ISimulationComparisonTask _simulationComparisonTask;
      private readonly ISingleStartPresenterTask _singleStartPresenterTask;
      private readonly Func<ISimulationComparisonTask, Func<ISimulationComparison>> _creator;

      protected CreateSimulationComparisonCommand(ISimulationComparisonTask simulationComparisonTask, ISingleStartPresenterTask singleStartPresenterTask,
         Func<ISimulationComparisonTask, Func<ISimulationComparison>> creator)
      {
         _simulationComparisonTask = simulationComparisonTask;
         _singleStartPresenterTask = singleStartPresenterTask;
         _creator = creator;
      }

      public void Execute()
      {
         _singleStartPresenterTask.StartForSubject(_creator(_simulationComparisonTask).Invoke());
      }
   }

   public class CreateIndividualSimulationComparisonUICommand : CreateSimulationComparisonCommand
   {
      public CreateIndividualSimulationComparisonUICommand(ISimulationComparisonTask simulationComparisonTask, ISingleStartPresenterTask singleStartPresenterTask)
         : base(simulationComparisonTask, singleStartPresenterTask, x => x.CreateIndividualSimulationComparison)
      {
      }
   }

   public class CreatePopulationSimulationComparisonUICommand : CreateSimulationComparisonCommand
   {
      public CreatePopulationSimulationComparisonUICommand(ISimulationComparisonTask simulationComparisonTask, ISingleStartPresenterTask singleStartPresenterTask)
         : base(simulationComparisonTask, singleStartPresenterTask, x => x.CreatePopulationSimulationComparison)
      {
      }
   }
}