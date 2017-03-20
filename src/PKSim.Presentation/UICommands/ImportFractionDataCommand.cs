using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class ImportFractionDataCommand : IUICommand
   {
      private readonly IImportObservedDataTask _observedDataTask;

      public ImportFractionDataCommand(IImportObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      public void Execute()
      {
         _observedDataTask.AddFractionDataToProject();
      }
   }
}