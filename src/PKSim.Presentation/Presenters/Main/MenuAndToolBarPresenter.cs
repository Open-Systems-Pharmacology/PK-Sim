using System;
using System.Drawing;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Core.Events;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Events;
using OSPSuite.Presentation.Presenters.Main;
using OSPSuite.Presentation.Repositories;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;
using OSPSuite.Assets;
using IWorkspace = PKSim.Presentation.Core.IWorkspace;

namespace PKSim.Presentation.Presenters.Main
{
   public interface IMenuAndToolBarPresenter : IMainViewItemPresenter,
      IListener<ProjectCreatedEvent>,
      IListener<ProjectClosedEvent>,
      IListener<ProjectChangedEvent>,
      IListener<SimulationRunStartedEvent>,
      IListener<SimulationRunFinishedEvent>,
      IListener<EnableUIEvent>,
      IListener<DisableUIEvent>,
      IListener<ScreenActivatedEvent>,
      IListener<NoActiveScreenEvent>,
      IListener<BuildingBlockAddedEvent>,
      IListener<BuildingBlockRemovedEvent>,
      IListener<SimulationResultsUpdatedEvent>,
      IListener<EditJournalPageStartedEvent>,
      IListener<JournalLoadedEvent>,
      IListener<JournalClosedEvent>,
      IListener<ParameterIdentificationStartedEvent>,
      IListener<ParameterIdentificationTerminatedEvent>,
      IListener<SensitivityAnalysisStartedEvent>,
      IListener<SensitivityAnalysisTerminatedEvent>
   {
   }

   public class MenuAndToolBarPresenter : AbstractMenuAndToolBarPresenter, IMenuAndToolBarPresenter,
      IVisitor<ParameterIdentification>,
      IVisitor<Simulation>,
      IVisitor<PopulationSimulationComparison>,
      IVisitor<SensitivityAnalysis>

   {
      private readonly IMenuBarItemRepository _menuBarItemRepository;
      private readonly IButtonGroupRepository _buttonGroupRepository;
      private readonly ISkinManager _skinManager;
      private readonly IStartOptions _startOptions;
      private readonly IWorkspace _workspace;
      private readonly IActiveSubjectRetriever _activeSubjectRetriever;
      private bool _enabled;
      private SimulationState _simulationState;

      //cache containing the name of the ribbon category corresponding to a given type.Returns an empty string if not found
      private readonly ICache<Type, string> _dynamicRibbonPageCache = new Cache<Type, string>(t => string.Empty);
      private bool _parameterIdentificationRunning;
      private bool _sensitivityRunning;

      public MenuAndToolBarPresenter(IMenuAndToolBarView view, IMenuBarItemRepository menuBarItemRepository,
         IButtonGroupRepository buttonGroupRepository, IMRUProvider mruProvider,
         ISkinManager skinManager, IStartOptions startOptions, IWorkspace workspace, IActiveSubjectRetriever activeSubjectRetriever) : base(view, menuBarItemRepository, mruProvider)
      {
         _menuBarItemRepository = menuBarItemRepository;
         _buttonGroupRepository = buttonGroupRepository;
         _skinManager = skinManager;
         _startOptions = startOptions;
         _workspace = workspace;
         _activeSubjectRetriever = activeSubjectRetriever;
         _enabled = true;
      }

      protected override void AddRibbonPages()
      {
         _view.AddApplicationMenu(_buttonGroupRepository.Find(ButtonGroupIds.Project));

         _view.AddPageGroupToPage(_buttonGroupRepository.Find(ButtonGroupIds.Create), PKSimConstants.RibbonPages.Modeling);
         _view.AddPageGroupToPage(_buttonGroupRepository.Find(ButtonGroupIds.Compare), PKSimConstants.RibbonPages.Modeling);

         _view.AddPageGroupToPage(_buttonGroupRepository.Find(ButtonGroupIds.ParameterIdentification), RibbonPages.ParameterIdentificationAndSensitivity);
         _view.AddPageGroupToPage(_buttonGroupRepository.Find(ButtonGroupIds.SensitivityAnalysis), RibbonPages.ParameterIdentificationAndSensitivity);

         _view.AddPageGroupToPage(_buttonGroupRepository.Find(ButtonGroupIds.Journal), PKSimConstants.RibbonPages.WorkingJournal);

         _view.AddPageGroupToPage(_buttonGroupRepository.Find(ButtonGroupIds.Import), PKSimConstants.RibbonPages.ImportExport);
         _view.AddPageGroupToPage(_buttonGroupRepository.Find(ButtonGroupIds.LoadBuildingBlocks), PKSimConstants.RibbonPages.ImportExport);
         _view.AddPageGroupToPage(_buttonGroupRepository.Find(ButtonGroupIds.ExportProject), PKSimConstants.RibbonPages.ImportExport);

         _view.InitializeSkinGallery(_skinManager, PKSimConstants.Ribbons.Skins, PKSimConstants.RibbonPages.Utilities);
         _view.AddPageGroupToPage(_buttonGroupRepository.Find(ButtonGroupIds.Tools), PKSimConstants.RibbonPages.Utilities);
         _view.AddPageGroupToPage(_buttonGroupRepository.Find(ButtonGroupIds.DisplayUnits), PKSimConstants.RibbonPages.Utilities);
         _view.AddPageGroupToPage(_buttonGroupRepository.Find(ButtonGroupIds.Favorites), PKSimConstants.RibbonPages.Utilities);


         if (_startOptions.IsDeveloperMode)
            _view.AddPageGroupToPage(_buttonGroupRepository.Find(ButtonGroupIds.Admin), PKSimConstants.RibbonPages.Utilities);

         _view.AddPageGroupToPage(_buttonGroupRepository.Find(ButtonGroupIds.View), PKSimConstants.RibbonPages.Views);

         _view.AddQuickAcccessButton(_menuBarItemRepository[MenuBarItemIds.OpenProject]);
         _view.AddQuickAcccessButton(_menuBarItemRepository[MenuBarItemIds.SaveProject]);
         _view.AddPageHeaderItemLinks(_menuBarItemRepository[MenuBarItemIds.Help]);

         initializeDynamicPages();
      }

      private void initializeDynamicPages()
      {
         _view.CreateDynamicPageCategory(PKSimConstants.RibbonCategories.IndividualSimulation, Color.LightGreen);
         _view.CreateDynamicPageCategory(PKSimConstants.RibbonCategories.PopulationSimulation, Color.LightGreen);
         _view.CreateDynamicPageCategory(PKSimConstants.RibbonCategories.PopulationSimulationComparison, Color.LightGreen);
         _view.CreateDynamicPageCategory(RibbonCategories.ParameterIdentification, Color.LightGreen);
         _view.CreateDynamicPageCategory(RibbonCategories.SensitivityAnalysis, Color.LightGreen);

         _dynamicRibbonPageCache.Add(typeof (IndividualSimulation), PKSimConstants.RibbonCategories.IndividualSimulation);
         _dynamicRibbonPageCache.Add(typeof (PopulationSimulation), PKSimConstants.RibbonCategories.PopulationSimulation);
         _dynamicRibbonPageCache.Add(typeof (PopulationSimulationComparison), PKSimConstants.RibbonCategories.PopulationSimulationComparison);
         _dynamicRibbonPageCache.Add(typeof (ParameterIdentification), RibbonCategories.ParameterIdentification);
         _dynamicRibbonPageCache.Add(typeof (SensitivityAnalysis), RibbonCategories.SensitivityAnalysis);

         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.RunSimulation), PKSimConstants.RibbonPages.RunSimulation, PKSimConstants.RibbonCategories.IndividualSimulation);
         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.IndividualSimulationAnalyses), PKSimConstants.RibbonPages.RunSimulation, PKSimConstants.RibbonCategories.IndividualSimulation);
         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.ExportIndividualSimulation), PKSimConstants.RibbonPages.Export, PKSimConstants.RibbonCategories.IndividualSimulation);

         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.RunSimulation), PKSimConstants.RibbonPages.RunSimulation, PKSimConstants.RibbonCategories.PopulationSimulation);
         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.PopulationSimulationAnalyses), PKSimConstants.RibbonPages.RunSimulation, PKSimConstants.RibbonCategories.PopulationSimulation);
         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.PopulationSimulationWorkflow), PKSimConstants.RibbonPages.RunSimulation, PKSimConstants.RibbonCategories.PopulationSimulation);
         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.ImportPopulationSimulation), PKSimConstants.RibbonPages.Import, PKSimConstants.RibbonCategories.PopulationSimulation);
         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.ExportPopulationSimulation), PKSimConstants.RibbonPages.Export, PKSimConstants.RibbonCategories.PopulationSimulation);

         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.PopulationSimulationAnalyses), PKSimConstants.RibbonPages.Analyze, PKSimConstants.RibbonCategories.PopulationSimulationComparison);

         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.RunParameterIdentification), RibbonPages.RunParameterIdentification, RibbonCategories.ParameterIdentification);
         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.ParameterIdentificationAnalyses), RibbonPages.RunParameterIdentification, RibbonCategories.ParameterIdentification);
         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.ParameterIdentificationConfidenceInterval), RibbonPages.RunParameterIdentification, RibbonCategories.ParameterIdentification);

         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.RunSensitivityAnalysis), RibbonPages.RunSensitivityAnalysis, RibbonCategories.SensitivityAnalysis);
         _view.AddDynamicPageGroupToPageCategory(_buttonGroupRepository.Find(ButtonGroupIds.SensitivityAnalysisPKParameterAnalyses), RibbonPages.RunSensitivityAnalysis, RibbonCategories.SensitivityAnalysis);
      }

      protected override void DisableMenuBarItemsForPogramStart()
      {
         DisableAll();
         enableDefaultItems();
         updateResultsVisibility(shouldShowIndividualResults: true);
      }

      private void enableDefaultItems()
      {
         _menuBarItemRepository[MenuBarItemIds.NewProject].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.OpenProject].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.Options].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.HistoryView].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.ComparisonView].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.BuildingBlockExplorerView].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.SimulationExplorerView].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.GarbageCollection].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.GenerateCalculationMethods].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.GeneratePKMLTemplates].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.GenerateGroupsTemplate].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.Exit].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.Help].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.About].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.JournalView].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.JournalDiagramView].Enabled = true;
         _menuBarItemRepository[MenuBarItemIds.LoadProjectFromSnahpshot].Enabled = true;
      }

      public void Handle(ProjectCreatedEvent eventToHandle)
      {
         updateLoadedProjectItem(eventToHandle);
      }

      public override void Handle(ProjectLoadedEvent eventToHandle)
      {
         base.Handle(eventToHandle);
         updateLoadedProjectItem(eventToHandle);
      }

      private void updateLoadedProjectItem(ProjectEvent projectEvent)
      {
         updateProjectItems(isEnabled: true, observedDataEnabled: compoundsAvailableIn(projectEvent.Project));
      }

      public void Handle(ProjectClosedEvent eventToHandle)
      {
         updateProjectItems(isEnabled: false, observedDataEnabled: false);
         _menuBarItemRepository[MenuBarItemIds.JournalEditorView].Enabled = false;
         _simulationState.Reset();
      }

      public void Handle(NoActiveScreenEvent eventToHandle)
      {
         disableSimulationItems();
         hideAllDyamicCategories();
      }

      public void Handle(ScreenActivatedEvent eventToHandle)
      {
         hideAllDyamicCategories();
         var subject = eventToHandle.Presenter.Subject;
         eventToHandle.Presenter.Activated();
         if (subject == null) return;

         this.Visit(subject);

         var matchingPage = _dynamicRibbonPageCache.Keys.FirstOrDefault(x => subject.IsAnImplementationOf(x));
         if (matchingPage == null) return;

         _view.SetPageCategoryVisibility(_dynamicRibbonPageCache[matchingPage], true);
      }

      private void hideAllDyamicCategories()
      {
         PKSimConstants.RibbonCategories.AllDynamicCategories().Each(x => _view.SetPageCategoryVisibility(x, visible: false));
      }

      private void updateSimulationStateFrom(Simulation simulation, bool isActiveSimulation)
      {
         var isIndivividualSimulation = simulation.IsAnImplementationOf<IndividualSimulation>();
         _simulationState.IsActivated = isActiveSimulation;
         _simulationState.HasResult = simulationHasResults(simulation);
         _simulationState.IsIndividual = isIndivividualSimulation;
         _simulationState.IsImported = simulation.IsImported;
      }

      private bool simulationHasResults(Simulation simulation)
      {
         if (simulation.HasResults)
            return true;

         //for population simulation, it is enough to have analyses 
         var populationSimulation = simulation as PopulationSimulation;
         if (populationSimulation == null)
            return false;

         return populationSimulation.HasPKAnalyses;
      }

      private void updateResultsVisibility(bool shouldShowIndividualResults)
      {
         _menuBarItemRepository[MenuBarItemIds.ShowIndividualResults].Visible = shouldShowIndividualResults;
      }

      private void disableSimulationItems()
      {
         _simulationState.Reset();
         updateSimulationItemsAccordingToSimulationState();
      }

      private void updateSimulationItemsAccordingToSimulationState()
      {
         bool enabled = _simulationState.IsActivated && _enabled;
         bool resultsEnabled = enabled && _simulationState.HasResult;
         bool enablePopSimulationItems = enabled && !_simulationState.IsIndividual;
         bool enableIndividualSimulationItems = enabled && _simulationState.IsIndividual;
         bool enabledPKSimSimulationOnlyItems = enabled && !_simulationState.IsImported;

         //All simulation type items
         _menuBarItemRepository[MenuBarItemIds.Run].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.RunWithSettings].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.ExportActiveSimulationToMoBi].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.ExportActiveSimulationToPkml].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.ExportActiveSimulationResultsToCSV].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.ExportActiveSimulationToPDF].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.CloneActiveSimulation].Enabled = enabledPKSimSimulationOnlyItems;
         _menuBarItemRepository[MenuBarItemIds.ConfigureActiveSimulation].Enabled = enabledPKSimSimulationOnlyItems;

         //Individual Simulation Items
         _menuBarItemRepository[MenuBarItemIds.ExportActiveSimulationResultsToExcel].Enabled = enableIndividualSimulationItems;

         //Population Simulation Items
         _menuBarItemRepository[MenuBarItemIds.ExportActiveSimulationForClusterComputations].Enabled = enablePopSimulationItems;
         _menuBarItemRepository[MenuBarItemIds.ExportActiveSimulationPopulationToExcel].Enabled = enablePopSimulationItems;
         _menuBarItemRepository[MenuBarItemIds.ExportActiveSimulationPKAnalysesToCSV].Enabled = enablePopSimulationItems;
         _menuBarItemRepository[MenuBarItemIds.ImportActiveSimulationPKParameters].Enabled = enablePopSimulationItems;
         _menuBarItemRepository[MenuBarItemIds.ImportActiveSimulationResults].Enabled = enablePopSimulationItems;
         _menuBarItemRepository[MenuBarItemIds.SavePopulationSimulationWorkflow].Enabled = enablePopSimulationItems;
         _menuBarItemRepository[MenuBarItemIds.LoadPopulationSimulationWorkflow].Enabled = enablePopSimulationItems;

         _menuBarItemRepository[MenuBarItemIds.ShowIndividualResults].Enabled = resultsEnabled;
         enablePopulationAnalysesItems = resultsEnabled;
      }

      private bool enablePopulationAnalysesItems
      {
         set
         {
            _menuBarItemRepository[MenuBarItemIds.BoxWhiskerAnalysis].Enabled = value;
            _menuBarItemRepository[MenuBarItemIds.TimeProfileAnalysis].Enabled = value;
            _menuBarItemRepository[MenuBarItemIds.ScatterAnalysis].Enabled = value;
            _menuBarItemRepository[MenuBarItemIds.RangeAnalysis].Enabled = value;
         }
      }

      public void Handle(SimulationRunFinishedEvent eventToHandle)
      {
         enableDefaultItems();
         updateProjectItems(isEnabled: true, observedDataEnabled: true);

         updateSimulationItemsFor(eventToHandle.Simulation);
      }

      public void Handle(SimulationResultsUpdatedEvent eventToHandle)
      {
         updateSimulationItemsFor(eventToHandle.Simulation as Simulation);
      }

      private void updateSimulationItemsFor(Simulation simulation)
      {
         var activeSimulation = _activeSubjectRetriever.Active<Simulation>();
         _menuBarItemRepository[MenuBarItemIds.Stop].Enabled = false;
         bool simIsActive = (activeSimulation != null) && (activeSimulation == simulation);
         updateSimulationStateFrom(simulation, isActiveSimulation: simIsActive);
         updateSimulationItemsAccordingToSimulationState();
      }

      private void updateProjectItems(bool isEnabled, bool observedDataEnabled)
      {
         bool enabled = isEnabled && _enabled;
         updateSaveProjectButtons(enabled);

         _menuBarItemRepository[MenuBarItemIds.ProjectDescription].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.ExportProjectToSnapshot].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.CloseProject].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.NewIndividual].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.LoadIndividual].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.NewPopulation].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.LoadPopulation].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.ImportPopulation].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.NewCompound].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.LoadCompound].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.NewFormulation].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.LoadFormulationFromTemplate].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.NewSimulation].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.NewImportIndividualSimulation].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.NewImportPopulationSimulation].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.NewEvent].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.LoadEvent].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.NewProtocol].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.LoadProtocol].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.ImportObservedData].Enabled = enabled && observedDataEnabled;
         _menuBarItemRepository[MenuBarItemIds.ImportFractionData].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.ProjectReport].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.IndividualSimulationComparison].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.IndividualSimulationComparisonInAnalyze].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.PopulationSimulationComparison].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.PopulationSimulationComparisonInAnalyze].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.HistoryReportGroup].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.HistoryReportExcel].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.HistoryReportPDF].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.ManageUserDisplayUnits].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.ManageProjectDisplayUnits].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.UpdateAllToDisplayUnits].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.CreateJournalPage].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.SelectJournal].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.LoadFavorites].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.SaveFavorites].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.CreateParameterIdentification].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.ParameterIdentificationFeedbackView].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.CreateSensitivityAnalysis].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.SensitivityAnalysisFeedbackView].Enabled = enabled;
      }

      public void Handle(SimulationRunStartedEvent eventToHandle)
      {
         DisableAll();
         _menuBarItemRepository[MenuBarItemIds.Stop].Enabled = true;
      }

      public void Handle(DisableUIEvent eventToHandle)
      {
         _enabled = false;
         DisableAll();
      }

      public void Handle(EnableUIEvent eventToHandle)
      {
         _enabled = true;
         enableDefaultItems();

         updateProjectItems(isEnabled: eventToHandle.ProjectLoaded, observedDataEnabled: compoundsAvailableIn(eventToHandle.Project));

         updateSimulationItemsAccordingToSimulationState();
      }

      private void updateSaveProjectButtons(bool enabled)
      {
         var canSave = enabled && !_workspace.ProjectIsReadOnly;
         _menuBarItemRepository[MenuBarItemIds.SaveGroup].Enabled = enabled;
         _menuBarItemRepository[MenuBarItemIds.SaveProject].Enabled = canSave;
         _menuBarItemRepository[MenuBarItemIds.SaveProjectAs].Enabled = enabled;
      }

      public void Handle(ProjectChangedEvent eventToHandle)
      {
         updateSaveProjectButtons(true);
      }

      public void Handle(BuildingBlockAddedEvent eventToHandle)
      {
         _menuBarItemRepository[MenuBarItemIds.ImportObservedData].Enabled = compoundsAvailableIn(eventToHandle.Project);
      }

      public void Handle(BuildingBlockRemovedEvent eventToHandle)
      {
         _menuBarItemRepository[MenuBarItemIds.ImportObservedData].Enabled = compoundsAvailableIn(eventToHandle.Project);
      }

      private bool compoundsAvailableIn(IProject project)
      {
         return project != null && project.DowncastTo<PKSimProject>().All<Compound>().Any();
      }

      private struct SimulationState
      {
         /// <summary>
         ///    Returns  <c>true</c> if a <see cref="Simulation" /> is activated otherwise  <c>false</c>
         /// </summary>
         public bool IsActivated { get; set; }

         /// <summary>
         ///    Returns <c>true</c> if activated <see cref="Simulation" /> has results otherwise <c>false</c>
         /// </summary>
         public bool HasResult { get; set; }

         /// <summary>
         ///    Returns <c>true</c> if activated <see cref="Simulation" /> is an imported simulation otherwise <c>false</c>
         /// </summary>
         public bool IsImported { get; set; }

         /// <summary>
         ///    Returns <c>true</c> if activated <see cref="Simulation" /> represents an individual simulation otherwise
         ///    <c>false</c>
         /// </summary>
         public bool IsIndividual { get; set; }

         public void Reset()
         {
            IsActivated = false;
            HasResult = false;
            IsImported = false;
            IsIndividual = true;
         }
      }

      public void Handle(EditJournalPageStartedEvent eventToHandle)
      {
         _menuBarItemRepository[MenuBarItemIds.JournalEditorView].Enabled = true;
      }

      public void Handle(JournalLoadedEvent eventToHandle)
      {
         enableJournalItems = true;
      }

      public void Handle(JournalClosedEvent eventToHandle)
      {
         enableJournalItems = false;
      }

      private bool enableJournalItems
      {
         set
         {
            _menuBarItemRepository[MenuBarItemIds.SearchJournal].Enabled = value;
            _menuBarItemRepository[MenuBarItemIds.ExportJournal].Enabled = value;
            _menuBarItemRepository[MenuBarItemIds.RefreshJournal].Enabled = value;
         }
      }

      public void Visit(Simulation simulation)
      {
         updateSimulationStateFrom(simulation, isActiveSimulation: true);
         updateSimulationItemsAccordingToSimulationState();
         updateResultsVisibility(shouldShowIndividualResults: _simulationState.IsIndividual);
      }

      public void Visit(PopulationSimulationComparison populationSimulationComparison)
      {
         enablePopulationAnalysesItems = true;
         updateResultsVisibility(shouldShowIndividualResults: false);
      }

      public void Visit(SensitivityAnalysis sensitivityAnalysis)
      {
         updateSensitivityAnalysisItems(sensitivityAnalysis);
      }

      public void Visit(ParameterIdentification parameterIdentification)
      {
         updateParameterIdentifcationItems(parameterIdentification);
      }

      public void Handle(ParameterIdentificationStartedEvent parameterIdentificationEvent)
      {
         _parameterIdentificationRunning = true;
         updateParameterIdentifcationItems(parameterIdentificationEvent.ParameterIdentification);
      }

      public void Handle(ParameterIdentificationTerminatedEvent parameterIdentificationEvent)
      {
         _parameterIdentificationRunning = false;
         updateParameterIdentifcationItems(parameterIdentificationEvent.ParameterIdentification);
      }

      private void updateParameterIdentifcationItems(ParameterIdentification parameterIdentification)
      {
         var hasResult = !_parameterIdentificationRunning && parameterIdentification.HasResults;
         _menuBarItemRepository[MenuBarItemIds.RunParameterIdentification].Enabled = !_parameterIdentificationRunning;
         _menuBarItemRepository[MenuBarItemIds.StopParameterIdentification].Enabled = _parameterIdentificationRunning;
         _menuBarItemRepository[MenuBarItemIds.TimeProfileParameterIdentification].Enabled = hasResult;
         _menuBarItemRepository[MenuBarItemIds.PredictedVsObservedParameterIdentification].Enabled = hasResult;
         _menuBarItemRepository[MenuBarItemIds.CorrelationMatrixParameterIdentification].Enabled = hasResult;
         _menuBarItemRepository[MenuBarItemIds.CovarianceMatrixParameterIdentification].Enabled = hasResult;
         _menuBarItemRepository[MenuBarItemIds.ResidualsVsTimeParameterIdentifcation].Enabled = hasResult;
         _menuBarItemRepository[MenuBarItemIds.ResidualHistogramParameterIdentification].Enabled = hasResult;
         _menuBarItemRepository[MenuBarItemIds.TimeProfileConfidenceInterval].Enabled = hasResult;
         _menuBarItemRepository[MenuBarItemIds.TimeProfilePredictionInterval].Enabled = hasResult;
         _menuBarItemRepository[MenuBarItemIds.TimeProfileVPCInterval].Enabled = hasResult;
      }

      public void Handle(SensitivityAnalysisStartedEvent sensitivityAnalysisEvent)
      {
         _sensitivityRunning = true;
         updateSensitivityAnalysisItems(sensitivityAnalysisEvent.SensitivityAnalysis);
      }

      public void Handle(SensitivityAnalysisTerminatedEvent sensitivityAnalysisEvent)
      {
         _sensitivityRunning = false;
         updateSensitivityAnalysisItems(sensitivityAnalysisEvent.SensitivityAnalysis);
      }

      private void updateSensitivityAnalysisItems(SensitivityAnalysis sensitivityAnalysis)
      {
         var hasResult = !_sensitivityRunning && sensitivityAnalysis.HasResults;
         _menuBarItemRepository[MenuBarItemIds.RunSensitivityAnalysis].Enabled = !_sensitivityRunning;
         _menuBarItemRepository[MenuBarItemIds.StopSensitivityAnalysis].Enabled = _sensitivityRunning;
         _menuBarItemRepository[MenuBarItemIds.SensitivityAnalysisPKParameterAnalysis].Enabled = hasResult;
      }
   }
}