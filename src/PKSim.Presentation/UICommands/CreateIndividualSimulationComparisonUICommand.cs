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
      private readonly ISimulationComparisonCreator _simulationComparisonCreator;
      private readonly ISingleStartPresenterTask _singleStartPresenterTask;
      private readonly Func<ISimulationComparisonCreator, Func<ISimulationComparison>> _creator;

      protected CreateSimulationComparisonCommand(ISimulationComparisonCreator simulationComparisonCreator, ISingleStartPresenterTask singleStartPresenterTask,
         Func<ISimulationComparisonCreator, Func<ISimulationComparison>> creator)
      {
         _simulationComparisonCreator = simulationComparisonCreator;
         _singleStartPresenterTask = singleStartPresenterTask;
         _creator = creator;
      }

      public void Execute()
      {
         _singleStartPresenterTask.StartForSubject(_creator(_simulationComparisonCreator).Invoke());
      }
   }

   public class CreateIndividualSimulationComparisonUICommand : CreateSimulationComparisonCommand
   {
      public CreateIndividualSimulationComparisonUICommand(ISimulationComparisonCreator simulationComparisonCreator, ISingleStartPresenterTask singleStartPresenterTask)
         : base(simulationComparisonCreator, singleStartPresenterTask, x => x.CreateIndividualSimulationComparison)
      {
      }
   }

   public class CreatePopulationSimulationComparisonUICommand : CreateSimulationComparisonCommand
   {
      public CreatePopulationSimulationComparisonUICommand(ISimulationComparisonCreator simulationComparisonCreator, ISingleStartPresenterTask singleStartPresenterTask)
         : base(simulationComparisonCreator, singleStartPresenterTask, x => x.CreatePopulationSimulationComparison)
      {
      }
   }
}