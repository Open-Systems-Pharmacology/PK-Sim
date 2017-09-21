using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Assets;
using PKSim.Core.Snapshots.Services;
using IObservedDataTask = PKSim.Core.Services.IObservedDataTask;

namespace PKSim.Infrastructure.Services
{
   public class ObservedDataTask : OSPSuite.Core.Domain.Services.ObservedDataTask, IObservedDataTask
   {
      private readonly IPKSimProjectRetriever _projectRetriever;
      private readonly IExecutionContext _executionContext;
      private readonly IApplicationController _applicationController;
      private readonly ITemplateTask _templateTask;
      private readonly IObservedDataPersistor _observedDataPersistor;
      private readonly ISnapshotTask _snapshotTask;

      public ObservedDataTask(IPKSimProjectRetriever projectRetriever, IExecutionContext executionContext, IDialogCreator dialogCreator, IApplicationController applicationController,IDataRepositoryTask dataRepositoryTask,
         ITemplateTask templateTask, IContainerTask containerTask, IObservedDataPersistor observedDataPersistor, IObjectTypeResolver objectTypeResolver, ISnapshotTask snapshotTask) : base(dialogCreator, executionContext, dataRepositoryTask, containerTask, objectTypeResolver)
      {
         _projectRetriever = projectRetriever;
         _executionContext = executionContext;
         _applicationController = applicationController;
         _templateTask = templateTask;
         _observedDataPersistor = observedDataPersistor;
         _snapshotTask = snapshotTask;
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

      public void SaveToTemplate(DataRepository observedData)
      {
         _templateTask.SaveToTemplate(observedData, TemplateType.ObservedData);
      }

      public void ExportToPkml(DataRepository observedData)
      {
         var file = _dialogCreator.AskForFileToSave(PKSimConstants.UI.ExportObservedDataToPkml, Constants.Filter.PKML_FILE_FILTER, Constants.DirectoryKey.MODEL_PART, observedData.Name);
         if (string.IsNullOrEmpty(file)) return;

         _observedDataPersistor.Save(observedData, file);
      }

      public async Task LoadFromSnapshot()
      {
         var observedData = await _snapshotTask.LoadModelFromSnapshot<DataRepository>();
         observedData.Each(AddObservedDataToProject);
      }

      public void AddObservedDataToAnalysable(IReadOnlyList<DataRepository> observedData, IAnalysable analysable)
      {
         AddObservedDataToAnalysable(observedData, analysable, showData: false);
      }

      public void AddObservedDataToAnalysable(IReadOnlyList<DataRepository> observedData, IAnalysable analysable, bool showData)
      {
         var simulation = analysable as Simulation;
         if (simulation == null)
            return;

         var observedDataToAdd = observedData.Where(x => !simulation.UsesObservedData(x)).ToList();
         if (!observedDataToAdd.Any())
            return;

         observedDataToAdd.Each(simulation.AddUsedObservedData);
         _executionContext.PublishEvent(new ObservedDataAddedToAnalysableEvent(simulation, observedDataToAdd, showData));
         _executionContext.PublishEvent(new SimulationStatusChangedEvent(simulation));
      }

      public void RemoveUsedObservedDataFromSimulation(IReadOnlyList<UsedObservedData> usedObservedDataList)
      {
         if (!usedObservedDataList.Any())
            return;

         foreach (var usedObservedData in usedObservedDataList)
         {
            var parameterIdentifications = findParameterIdentificationsUsing(usedObservedData).ToList();
            if (!parameterIdentifications.Any())
               continue;

            _dialogCreator.MessageBoxInfo(Captions.ParameterIdentification.CannotRemoveObservedDataBeingUsedByParameterIdentification(observedDataFrom(usedObservedData).Name, parameterIdentifications.AllNames().ToList()));
            return;
         }

         var viewResult = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyRemoveObservedDataFromSimulation);
         if (viewResult == ViewResult.No)
            return;

         usedObservedDataList.GroupBy(x => x.Simulation).Each(x => removeUsedObservedDataFromSimulation(x, x.Key.DowncastTo<Simulation>()));
      }

      private IEnumerable<ParameterIdentification> findParameterIdentificationsUsing(UsedObservedData usedObservedData)
      {
         var observedData = observedDataFrom(usedObservedData);
         var simulation = usedObservedData.Simulation;

         return from parameterIdentification in allParameterIdentifications()
                let outputMappings = parameterIdentification.AllOutputMappingsFor(simulation)
                where outputMappings.Any(x => x.UsesObservedData(observedData))
                select parameterIdentification;
      }   

      private IReadOnlyCollection<ParameterIdentification> allParameterIdentifications()
      {
         return _projectRetriever.Current.AllParameterIdentifications;
      }

      private void removeUsedObservedDataFromSimulation(IEnumerable<UsedObservedData> usedObservedDatas, Simulation simulation)
      {
         _executionContext.Load(simulation);

         var observedDataList = observedDataListFrom(usedObservedDatas);
         observedDataList.Each(simulation.RemoveUsedObservedData);
         _executionContext.PublishEvent(new ObservedDataRemovedFromAnalysableEvent(simulation, observedDataList));
         _executionContext.PublishEvent(new SimulationStatusChangedEvent(simulation));
      }

      private IReadOnlyList<DataRepository> observedDataListFrom(IEnumerable<UsedObservedData> usedObservedDatas)
      {
         return usedObservedDatas.Select(observedDataFrom).ToList();
      }

      private DataRepository observedDataFrom(UsedObservedData usedObservedDatas)
      {
         return _projectRetriever.CurrentProject.ObservedDataBy(usedObservedDatas.Id);
      }

      public void LoadObservedDataFromTemplate()
      {
         var observedData = _templateTask.LoadFromTemplate<DataRepository>(TemplateType.ObservedData);
         if (observedData == null) return;

         AddObservedDataToProject(observedData);
      }
   }
}