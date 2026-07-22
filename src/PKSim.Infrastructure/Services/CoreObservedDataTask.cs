using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using IObservedDataTask = PKSim.Core.Services.IObservedDataTask;

namespace PKSim.Infrastructure.Services
{
   public class CoreObservedDataTask : ObservedDataTask, IObservedDataTask
   {
      protected readonly IPKSimProjectRetriever _projectRetriever;
      protected readonly IExecutionContext _executionContext;
      private readonly ITemplateTask _templateTask;
      private readonly IParameterChangeUpdater _parameterChangeUpdater;
      private readonly IPKMLPersistor _pkmlPersistor;
      private readonly IOutputMappingMatchingTask _outputMappingMatchingTask;

      public CoreObservedDataTask(
         IPKSimProjectRetriever projectRetriever,
         IExecutionContext executionContext,
         IDialogCreator dialogCreator,
         IDataRepositoryExportTask dataRepositoryTask,
         ITemplateTask templateTask,
         IContainerTask containerTask,
         IParameterChangeUpdater parameterChangeUpdater,
         IPKMLPersistor pkmlPersistor,
         IObjectTypeResolver objectTypeResolver,
         IOutputMappingMatchingTask outputMappingMatchingTask,
         IConfirmationManager confirmationManager)
         : base(dialogCreator, executionContext, dataRepositoryTask, containerTask,
            objectTypeResolver, confirmationManager)
      {
         _projectRetriever = projectRetriever;
         _executionContext = executionContext;
         _templateTask = templateTask;
         _parameterChangeUpdater = parameterChangeUpdater;
         _pkmlPersistor = pkmlPersistor;
         _outputMappingMatchingTask = outputMappingMatchingTask;
      }

      // Rename requires IApplicationController + IRenameObservedDataPresenter (Presentation-only).
      // The Presentation-aware derived ObservedDataTask overrides this. Headless variant: no-op.
      public override void Rename(DataRepository observedData)
      {
      }

      public override void UpdateMolWeight(DataRepository observedData)
      {
         _parameterChangeUpdater.UpdateMolWeightIn(observedData);
      }

      public void SaveToTemplate(DataRepository observedData)
      {
         _templateTask.SaveToTemplate(observedData, TemplateType.ObservedData);
      }

      public void ExportToPkml(DataRepository observedData)
      {
         var file = _dialogCreator.AskForFileToSave(PKSimConstants.UI.ExportObservedDataToPkml, Constants.Filter.PKML_FILE_FILTER,
            Constants.DirectoryKey.MODEL_PART, observedData.Name);
         if (string.IsNullOrEmpty(file)) return;

         _pkmlPersistor.SaveToPKML(observedData, file);
      }

      // LoadFromSnapshot lives in the Presentation-aware derived class (needs IApplicationController).
      public virtual void LoadFromSnapshot()
      {
      }

      public void AddObservedDataToAnalysable(IReadOnlyList<DataRepository> observedDataList, IAnalysable analysable)
      {
         AddObservedDataToAnalysable(observedDataList, analysable, showData: false);
      }

      public void AddObservedDataToAnalysable(IReadOnlyList<DataRepository> observedDataList, IAnalysable analysable, bool showData)
      {
         var simulation = analysable as Simulation;
         if (simulation == null)
            return;

         var observedDataToAdd = observedDataList.Where(x => !simulation.UsesObservedData(x)).ToList();
         if (!observedDataToAdd.Any())
            return;

         observedDataToAdd.Each(simulation.AddUsedObservedData);
         observedDataList.Each(observedData => _outputMappingMatchingTask.AddMatchingOutputMapping(observedData, simulation));

         _executionContext.PublishEvent(new ObservedDataAddedToAnalysableEvent(simulation, observedDataToAdd, showData));
         _executionContext.PublishEvent(new SimulationStatusChangedEvent(simulation));
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
         observedDataList.Each(simulation.RemoveOutputMappings);

         _executionContext.PublishEvent(new ObservedDataRemovedFromAnalysableEvent(simulation, observedDataList));
         _executionContext.PublishEvent(new SimulationStatusChangedEvent(simulation));
      }

      private IReadOnlyList<DataRepository> observedDataListFrom(IEnumerable<UsedObservedData> usedObservedDatas)
      {
         return usedObservedDatas.Select(observedDataFrom).ToList();
      }

      protected DataRepository observedDataFrom(UsedObservedData usedObservedDatas)
      {
         return _projectRetriever.CurrentProject.ObservedDataBy(usedObservedDatas.Id);
      }

      public async Task LoadObservedDataFromTemplateAsync()
      {
         var observedDataList = await _templateTask.LoadFromTemplateAsync<DataRepository>(TemplateType.ObservedData);
         observedDataList.Each(AddObservedDataToProject);
      }
   }
}