using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.UICommands;
using PKSim.Core;
using PKSim.Core.Services;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using OSPSuite.Utility.Extensions;
using System.Collections.Generic;
using OSPSuite.Infrastructure.Import.Services;

namespace PKSim.Presentation.UICommands
{
   public class ReloadAllObservedDataCommand : ObjectUICommand<DataRepository>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IImportObservedDataTask _importObservedDataTask;
      private readonly IParameterIdentificationTask _parameterIdentificationTask;

      public ReloadAllObservedDataCommand(
         IExecutionContext executionContext,
         IImportObservedDataTask importObservedDataTask,
         IParameterIdentificationTask parameterIdentificationTask)
      {
         _executionContext = executionContext;
         _importObservedDataTask = importObservedDataTask;
         _parameterIdentificationTask = parameterIdentificationTask;
      }

      protected override void PerformExecute()
      {
         if (string.IsNullOrEmpty(Subject.ConfigurationId))
            return;

         var project = _executionContext.Project;
         var configurationId = Subject.ConfigurationId;

         //we should check this
         var observedDataFromSameFile =
            project.AllObservedData.Where(r => !string.IsNullOrEmpty(r.ConfigurationId) && r.ConfigurationId == configurationId ).ToList(); //actually the question here is: configID means they come from the same file right?

         var configuration = project.ImporterConfigurationBy(configurationId);
         _importObservedDataTask.AddAndReplaceObservedDataFromConfigurationToProject(configuration, observedDataFromSameFile);
         
      }
   }
}