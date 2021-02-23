using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.UICommands;
using PKSim.Core;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class ReloadObservedDataCommand : ObjectUICommand<DataRepository>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IImportObservedDataTask _importObservedDataTask;
      private readonly IObservedDataTask _observedDataTask;

      public ReloadObservedDataCommand(
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
         _observedDataTask.Delete(Subject);

         var configuration = project.ImporterConfigurationBy(Subject.ConfigurationId);
         _importObservedDataTask.AddObservedDataFromConfigurationToProjectForDataRepository(configuration, Subject.Name);
      }
   }
}
