using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class ImportAmountDataCommmand : IUICommand
   {
      private readonly IImportObservedDataTask _observedDataTask;

      public ImportAmountDataCommmand(IImportObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      public void Execute()
      {
         _observedDataTask.AddAmountDataToProject();
      }
   }
}