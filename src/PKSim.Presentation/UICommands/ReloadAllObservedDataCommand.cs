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
         if (string.IsNullOrEmpty(Subject.ConfigurationId))
            return;

         var project = _executionContext.Project;
         var configurationId = Subject.ConfigurationId;
         _observedDataTask.Delete(project.AllObservedData.Where(r => !string.IsNullOrEmpty(r.ConfigurationId) && r.ConfigurationId == configurationId));

         var configuration = project.ImporterConfigurationBy(configurationId);
         _importObservedDataTask.AddObservedDataFromConfigurationToProject(configuration);
      }
   }
}