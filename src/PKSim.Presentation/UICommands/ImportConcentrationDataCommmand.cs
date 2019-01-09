using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class ImportConcentrationDataCommmand : IUICommand
   {
      private readonly IImportObservedDataTask _observedDataTask;

      public ImportConcentrationDataCommmand(IImportObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      public void Execute()
      {
         _observedDataTask.AddObservedDataToProject();
      }
   }
}