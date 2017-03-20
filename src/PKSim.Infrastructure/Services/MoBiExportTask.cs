using System;
using System.Linq;
using System.Threading.Tasks;
using PKSim.Assets;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization.Exchange;
using OSPSuite.Core.Services;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using Process = System.Diagnostics.Process;

namespace PKSim.Infrastructure.Services
{
   public class MoBiExportTask : IMoBiExportTask, IVisitor<IObjectBase>
   {
      private readonly IBuildConfigurationTask _buildConfigurationTask;
      private readonly ISimulationToModelCoreSimulationMapper _simulationMapper;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IPKSimConfiguration _configuration;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IDialogCreator _dialogCreator;
      private readonly ISimulationPersistor _simulationPersistor;
      private readonly IProjectRetriever _projectRetriever;
      private readonly IObjectIdResetter _objectIdResetter;
      private readonly IJournalRetriever _journalRetriever;

      public MoBiExportTask(IBuildConfigurationTask buildConfigurationTask, ISimulationToModelCoreSimulationMapper simulationMapper,
         IRepresentationInfoRepository representationInfoRepository, IPKSimConfiguration configuration,
         ILazyLoadTask lazyLoadTask, IDialogCreator dialogCreator, ISimulationPersistor simulationPersistor, IProjectRetriever projectRetriever,
         IObjectIdResetter objectIdResetter, IJournalRetriever journalRetriever)
      {
         _buildConfigurationTask = buildConfigurationTask;
         _simulationMapper = simulationMapper;
         _representationInfoRepository = representationInfoRepository;
         _configuration = configuration;
         _lazyLoadTask = lazyLoadTask;
         _dialogCreator = dialogCreator;
         _simulationPersistor = simulationPersistor;
         _projectRetriever = projectRetriever;
         _objectIdResetter = objectIdResetter;
         _journalRetriever = journalRetriever;
      }

      public void StartWith(Simulation simulation)
      {
         startMoBiWithFile(() =>
         {
            var moBiFile = FileHelper.GenerateTemporaryFileName();
            exportSimulationToFile(simulation, moBiFile);
            return moBiFile;
         }, "s");
      }

      public void StartWithContentFile(string contentFile)
      {
         startMoBiWithFile(() => contentFile, "b");
      }

      private void startMoBiWithFile(Func<string> contentFileFunc, string option)
      {
         if (!FileHelper.FileExists(_configuration.MoBiPath))
            throw new PKSimException(PKSimConstants.Error.MoBiNotFound);

         var contentFile = contentFileFunc();

         //now start MoBi
         this.DoWithinExceptionHandler(() => Process.Start(_configuration.MoBiPath, $"/{option} \"{contentFile}\""));
      }

      private void exportSimulationToFile(Simulation simulation, string moBiFile)
      {
         _lazyLoadTask.Load(simulation);
         if (simulation.IsImported)
            throw new PKSimException(PKSimConstants.Error.CannotExportAnImportedSimulation);

         var configuration = _buildConfigurationTask.CreateFor(simulation, shouldValidate: true, createAgingDataInSimulation: false);
         var moBiSimulation = _simulationMapper.MapFrom(simulation, configuration, shouldCloneModel: false);
         updateObserverForAllFlag(moBiSimulation);
         updateRepresentationInfo(moBiSimulation);
         updateFormulaIdIn(moBiSimulation);

         var currentProject = _projectRetriever.CurrentProject;
         var simulationTransfer = new SimulationTransfer
         {
            Simulation = moBiSimulation,
            AllObservedData = simulation.UsedObservedData.Select(o => currentProject.ObservedDataBy(o.Id)).ToList(),
            Favorites = currentProject.Favorites,
            JournalPath = _journalRetriever.JournalFullPath
         };

         _simulationPersistor.Save(simulationTransfer, moBiFile);
      }

      private void updateFormulaIdIn(IModelCoreSimulation modelCoreSimulation)
      {
         _objectIdResetter.ResetIdFor(modelCoreSimulation);
      }

      public Task SaveSimulationToFileAsync(Simulation simulation, string fileName)
      {
         return Task.Run(() => SaveSimulationToFile(simulation, fileName));
      }

      public void UpdateObserverForAllFlag(IObserverBuildingBlock observerBuildingBlock)
      {
         var allObserversForAll = observerBuildingBlock.Where(x => x.NameIsOneOf(CoreConstants.Observer.MoBiForAll));
         allObserversForAll.Each(x => x.ForAll = true);
      }

      private void updateObserverForAllFlag(IModelCoreSimulation moBiSimulation)
      {
         UpdateObserverForAllFlag(moBiSimulation.BuildConfiguration.Observers);
      }

      public void SaveSimulationToFile(Simulation simulation)
      {
         var moBiFile = _dialogCreator.AskForFileToSave(PKSimConstants.UI.ExportSimulationToMoBiTitle, Constants.Filter.PKML_FILE_FILTER, Constants.DirectoryKey.MODEL_PART, simulation.Name);
         SaveSimulationToFile(simulation, moBiFile);
      }

      public void SaveSimulationToFile(Simulation simulation, string fileName)
      {
         if (string.IsNullOrEmpty(fileName)) return;
         exportSimulationToFile(simulation, fileName);
      }

      private void updateRepresentationInfo(IModelCoreSimulation moBiSimulation)
      {
         moBiSimulation.BuildConfiguration.SpatialStructure.AcceptVisitor(this);
         moBiSimulation.BuildConfiguration.Reactions.AcceptVisitor(this);
         moBiSimulation.BuildConfiguration.Molecules.AcceptVisitor(this);
         moBiSimulation.BuildConfiguration.PassiveTransports.AcceptVisitor(this);
         moBiSimulation.BuildConfiguration.Observers.AcceptVisitor(this);
         moBiSimulation.BuildConfiguration.EventGroups.AcceptVisitor(this);
         moBiSimulation.Model.AcceptVisitor(this);
      }

      public void Visit(IObjectBase objToVisit)
      {
         if (!_representationInfoRepository.ContainsInfoFor(objToVisit))
            return;

         var repInfo = _representationInfoRepository.InfoFor(objToVisit);
         objToVisit.Icon = repInfo.IconName;

         if (string.IsNullOrEmpty(objToVisit.Description))
            objToVisit.Description = repInfo.Description;
      }
   }
}