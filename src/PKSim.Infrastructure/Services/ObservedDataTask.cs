using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Assets;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Presenters.Snapshots;
using IObservedDataTask = PKSim.Core.Services.IObservedDataTask;

namespace PKSim.Infrastructure.Services
{
   public class ObservedDataTask : OSPSuite.Core.Domain.Services.ObservedDataTask, IObservedDataTask
   {
      private readonly IPKSimProjectRetriever _projectRetriever;
      private readonly IExecutionContext _executionContext;
      private readonly IApplicationController _applicationController;
      private readonly ITemplateTask _templateTask;
      private readonly IParameterChangeUpdater _parameterChangeUpdater;
      private readonly IPKMLPersistor _pkmlPersistor;
      private readonly IEntitiesInSimulationRetriever _entitiesInSimulationRetriever;

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
         IEntitiesInSimulationRetriever entitiesInSimulationRetriever) : base(dialogCreator, executionContext, dataRepositoryTask, containerTask, objectTypeResolver)
      {
         _projectRetriever = projectRetriever;
         _executionContext = executionContext;
         _applicationController = applicationController;
         _templateTask = templateTask;
         _parameterChangeUpdater = parameterChangeUpdater;
         _pkmlPersistor = pkmlPersistor;
         _entitiesInSimulationRetriever = entitiesInSimulationRetriever;
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
         var file = _dialogCreator.AskForFileToSave(PKSimConstants.UI.ExportObservedDataToPkml, Constants.Filter.PKML_FILE_FILTER, Constants.DirectoryKey.MODEL_PART, observedData.Name);
         if (string.IsNullOrEmpty(file)) return;

         _pkmlPersistor.SaveToPKML(observedData, file);
      }

      public void LoadFromSnapshot()
      {
         using (var presenter = _applicationController.Start<ILoadFromSnapshotPresenter<DataRepository>>())
         {
            var observedData = presenter.LoadModelFromSnapshot();
            observedData?.Each(AddObservedDataToProject);
         }
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

         foreach (var observedData in observedDataList)
         {
            var newOutputMapping = mapMatchingOutput(observedData, simulation);

            if (newOutputMapping.Output != null)
               simulation.OutputMappings.Add(newOutputMapping);
         }

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

      private OutputMapping mapMatchingOutput(DataRepository observedData, ISimulation simulation)
      {
         var newOutputMapping = new OutputMapping();
         var pathCache = _entitiesInSimulationRetriever.OutputsFrom(simulation);
         var matchingOutputPath = pathCache.Keys.FirstOrDefault(x => observedDataMatchesOutput(observedData, x));

         if (matchingOutputPath == null)
         {
            newOutputMapping.WeightedObservedData = new WeightedObservedData(observedData);
            return newOutputMapping;
         }

         var matchingOutput = pathCache[matchingOutputPath];

         newOutputMapping.OutputSelection =
            new SimulationQuantitySelection(simulation, new QuantitySelection(matchingOutputPath, matchingOutput.QuantityType));
         newOutputMapping.WeightedObservedData = new WeightedObservedData(observedData);
         newOutputMapping.Scaling = defaultScalingFor(matchingOutput);
         return newOutputMapping;
      }

      private Scalings defaultScalingFor(IQuantity output)
      {
         return output.IsFraction() ? Scalings.Linear : Scalings.Log;
      }

      private bool observedDataMatchesOutput(DataRepository observedData, string outputPath)
      {
         var organ = observedData.ExtendedPropertyValueFor(Constants.ObservedData.ORGAN);
         var compartment = observedData.ExtendedPropertyValueFor(Constants.ObservedData.COMPARTMENT);
         var molecule = observedData.ExtendedPropertyValueFor(Constants.ObservedData.MOLECULE);

         if (organ == null || compartment == null || molecule == null)
            return false;

         return outputPath.Contains(organ) && outputPath.Contains(compartment) && outputPath.Contains(molecule);
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

      private DataRepository observedDataFrom(UsedObservedData usedObservedDatas)
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