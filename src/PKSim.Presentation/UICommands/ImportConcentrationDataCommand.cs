using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class ImportConcentrationDataCommand : IUICommand
   {
      private readonly IImportObservedDataTask _observedDataTask;

      public ImportConcentrationDataCommand(IImportObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      public void Execute()
      {
         _observedDataTask.AddObservedDataToProject();
      }
   }
}