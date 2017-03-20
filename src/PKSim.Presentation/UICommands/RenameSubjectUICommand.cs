using PKSim.Core.Model;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.UICommands
{
   public class RenameSubjectUICommand : ObjectUICommand<IMdiChildView>
   {
      private readonly RenameBuildingBlockUICommand _renameBuildingBlockCommand;
      private readonly RenameObjectUICommand _renameSimulationComparisonCommand;
      private readonly RenameObservedDataUICommand _renameObservedDataUICommand;

      public RenameSubjectUICommand(RenameBuildingBlockUICommand renameBuildingBlockCommand, RenameObjectUICommand renameSimulationComparisonCommand, RenameObservedDataUICommand renameObservedDataUICommand)
      {
         _renameBuildingBlockCommand = renameBuildingBlockCommand;
         _renameSimulationComparisonCommand = renameSimulationComparisonCommand;
         _renameObservedDataUICommand = renameObservedDataUICommand;
      }

      protected override void PerformExecute()
      {
         var subject = Subject.Presenter.Subject;
         if (subject == null) return;
         var buildingBlock = subject as IPKSimBuildingBlock;
         if (buildingBlock != null)
         {
            execute(_renameBuildingBlockCommand, buildingBlock);
            return;
         }

         var simulationComparison = subject as ISimulationComparison;
         if (simulationComparison != null)
         {
            execute(_renameSimulationComparisonCommand, simulationComparison);
            return;
         }

         var observedData = subject as DataRepository;
         if (observedData != null)
            execute(_renameObservedDataUICommand, observedData);
      }

      private void execute<T>(ObjectUICommand<T> command, T subject) where T : class
      {
         command.Subject = subject;
         command.Execute();
      }
   }
}