using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.Infrastructure.Services
{
   public class ObservedDataTask : CoreObservedDataTask
   {
      private readonly IApplicationController _applicationController;

      public ObservedDataTask(
         IPKSimProjectRetriever projectRetriever,
         IExecutionContext executionContext,
         IDialogCreator dialogCreator,
         IApplicationController applicationController,
         IDataRepositoryExportTask dataRepositoryTask,
         ITemplateTask templateTask,
         IContainerTask containerTask,
         IParameterChangeUpdater parameterChangeUpdater,
         IPKMLPersistor pkmlPersistor,
         IObjectTypeResolver objectTypeResolver,
         IOutputMappingMatchingTask outputMappingMatchingTask,
         IConfirmationManager confirmationManager)
         : base(projectRetriever, executionContext, dialogCreator, dataRepositoryTask, templateTask,
            containerTask, parameterChangeUpdater, pkmlPersistor, objectTypeResolver,
            outputMappingMatchingTask, confirmationManager)
      {
         _applicationController = applicationController;
      }

      public override void Rename(DataRepository observedData)
      {
         using (var renamePresenter = _applicationController.Start<IRenameObservedDataPresenter>())
         {
            if (!renamePresenter.Edit(observedData))
               return;

            _executionContext.AddToHistory(new RenameObservedDataCommand(observedData, renamePresenter.Name).Run(_executionContext));
         }
      }

      public override void LoadFromSnapshot()
      {
         using (var presenter = _applicationController.Start<ILoadFromSnapshotPresenter<DataRepository>>())
         {
            var observedData = presenter.LoadModelFromSnapshot();
            observedData?.Each(AddObservedDataToProject);
         }
      }
   }
}
