using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Presentation.Views;
using PKSim.Core.Model;

namespace PKSim.Presentation.UICommands
{
   public class RenameSubjectUICommand : ObjectUICommand<IMdiChildView>
   {
      private readonly RenameBuildingBlockUICommand _renameBuildingBlockCommand;
      private readonly RenameObjectUICommand _renameObjectCommand;
      private readonly RenameObservedDataUICommand _renameObservedDataCommand;

      public RenameSubjectUICommand(
         RenameBuildingBlockUICommand renameBuildingBlockCommand, 
         RenameObservedDataUICommand renameObservedDataCommand, 
         RenameObjectUICommand renameObjectCommand)
      {
         _renameBuildingBlockCommand = renameBuildingBlockCommand;
         _renameObjectCommand = renameObjectCommand;
         _renameObservedDataCommand = renameObservedDataCommand;
      }

      protected override void PerformExecute()
      {
         var subject = Subject.Presenter.Subject;
         if (subject == null) return;

         switch (subject)
         {
            case IPKSimBuildingBlock buildingBlock:
               execute(_renameBuildingBlockCommand, buildingBlock);
               break;
            case DataRepository observedData:
               execute(_renameObservedDataCommand, observedData);
               break;
            case IWithName withName:
               execute(_renameObjectCommand, withName);
               break;
         }
      }

      private void execute<T>(ObjectUICommand<T> command, T subject) where T : class
      {
         command.Subject = subject;
         command.Execute();
      }
   }
}