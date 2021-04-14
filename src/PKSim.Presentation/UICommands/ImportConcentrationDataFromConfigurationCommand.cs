using OSPSuite.Core.Import;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class ImportConcentrationDataFromConfigurationCommand : ObjectUICommand<Compound>
   {
      private readonly IImportObservedDataTask _observedDataTask;

      public ImportConcentrationDataFromConfigurationCommand(IImportObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      protected override void PerformExecute()
      {
         var configuration = _observedDataTask.OpenXmlConfiguration();
         if (configuration == null) return;
         _observedDataTask.AddObservedDataFromConfigurationToProjectForCompound(Subject, configuration);
      }
   }
}