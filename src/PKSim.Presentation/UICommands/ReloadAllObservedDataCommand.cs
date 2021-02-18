using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.UICommands;
using PKSim.Core;
using PKSim.Core.Services;
using System.Linq;

namespace PKSim.Presentation.UICommands
{
   public class ReloadAllObservedDataCommand : ObjectUICommand<DataRepository>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IImportObservedDataTask _importObservedDataTask;
      private readonly IObservedDataTask _observedDataTask;

      public ReloadAllObservedDataCommand(
         IExecutionContext executionContext,
         IImportObservedDataTask importObservedDataTask,
         IObservedDataTask observedDataTask
         )
      {
         _executionContext = executionContext;
         _importObservedDataTask = importObservedDataTask;
         _observedDataTask = observedDataTask;
      }

      protected override void PerformExecute()
      {
         if (!Subject.ExtendedProperties.Contains("Configuration"))
            return;

         var project = _executionContext.Project;
         var configurationId = Subject.ExtendedPropertyValueFor("Configuration");
         _observedDataTask.Delete(project.AllObservedData.Where(r => r.ExtendedProperties.Contains("Configuration") && r.ExtendedPropertyValueFor("Configuration") == configurationId));

         var configuration = project.ImporterConfigurationBy(configurationId);
         _importObservedDataTask.AddObservedDataFromConfigurationToProject(configuration);
      }
   }
}