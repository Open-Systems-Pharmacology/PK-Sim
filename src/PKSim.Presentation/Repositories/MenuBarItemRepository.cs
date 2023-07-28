using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Utility.Container;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Repositories
{
   public class MenuBarItemRepository : OSPSuite.Presentation.Repositories.MenuBarItemRepository
   {
      public MenuBarItemRepository(IContainer container)
         : base(container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuBarItems()
      {
         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewProject)
            .WithId(MenuBarItemIds.NewProject)
            .WithCommand<NewProjectCommand>(_container)
            .WithIcon(ApplicationIcons.ProjectNew)
            .WithDescription(PKSimConstants.UI.NewProjectDescription)
            .WithShortcut(Keys.Control | Keys.N);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.OpenProject)
            .WithId(MenuBarItemIds.OpenProject)
            .WithCommand<OpenProjectCommand>(_container)
            .WithDescription(PKSimConstants.UI.OpenProjectDescription)
            .WithIcon(ApplicationIcons.ProjectOpen)
            .WithShortcut(Keys.Control | Keys.O);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ProjectDescription)
            .WithId(MenuBarItemIds.ProjectDescription)
            .WithCommand<EditProjectDescriptionCommand>(_container)
            .WithDescription(PKSimConstants.UI.ProjectDescriptionDescription)
            .WithIcon(ApplicationIcons.ProjectDescription)
            .WithShortcut(Keys.Control | Keys.D);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportProjectToSnapshot)
            .WithId(MenuBarItemIds.ExportProjectToSnapshot)
            .WithCommand<ExportProjectToSnapshotCommand>(_container)
            .WithDescription(PKSimConstants.UI.ExportProjectToSnapshotDescription)
            .WithIcon(ApplicationIcons.SnapshotExport);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadProjectFromSnapshot)
            .WithId(MenuBarItemIds.LoadProjectFromSnahpshot)
            .WithCommand<LoadProjectFromSnapshotUICommand>(_container)
            .WithDescription(PKSimConstants.UI.LoadProjectFromSnapshotDescription)
            .WithIcon(ApplicationIcons.SnapshotImport);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.CloseProject)
            .WithId(MenuBarItemIds.CloseProject)
            .WithDescription(PKSimConstants.UI.CloseProjectDescription)
            .WithCommand<CloseProjectCommand>(_container)
            .WithIcon(ApplicationIcons.Close);

         yield return CreateSubMenu.WithCaption(PKSimConstants.MenuNames.SaveProject)
            .WithId(MenuBarItemIds.SaveGroup)
            .WithDescription(PKSimConstants.UI.SaveProjectDescription)
            .WithIcon(ApplicationIcons.Save);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SaveProject)
            .WithId(MenuBarItemIds.SaveProject)
            .WithDescription(PKSimConstants.UI.SaveProjectDescription)
            .WithCommand<SaveProjectCommand>(_container)
            .WithIcon(ApplicationIcons.Save)
            .WithShortcut(Keys.Control | Keys.S);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SaveProjectAs)
            .WithId(MenuBarItemIds.SaveProjectAs)
            .WithDescription(PKSimConstants.UI.SaveProjectAsDescription)
            .WithIcon(ApplicationIcons.SaveAs)
            .WithCommand<SaveProjectAsCommand>(_container);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewSimulation)
            .WithId(MenuBarItemIds.NewSimulation)
            .WithCommand<AddSimulationCommand>(_container)
            .WithDescription(PKSimConstants.UI.NewSimulationDescription)
            .WithIcon(ApplicationIcons.Simulation)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.S);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportIndividualSimulation)
            .WithId(MenuBarItemIds.NewImportIndividualSimulation)
            .WithCommand<NewImportIndividualSimulationCommand>(_container)
            .WithDescription(PKSimConstants.UI.ImportIndividualSimulationDescription)
            .WithIcon(ApplicationIcons.IndividualSimulationLoad);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportPopulationSimulation)
            .WithId(MenuBarItemIds.NewImportPopulationSimulation)
            .WithCommand<NewImportPopulationSimulationCommand>(_container)
            .WithDescription(PKSimConstants.UI.ImportPopulationSimulationDescription)
            .WithIcon(ApplicationIcons.PopulationSimulationLoad);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewIndividual)
            .WithId(MenuBarItemIds.NewIndividual)
            .WithCommand<AddIndividualCommand>(_container)
            .WithDescription(PKSimConstants.UI.NewIndividualDescription)
            .WithIcon(ApplicationIcons.Individual)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.I);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadIndividual)
            .WithCommand<LoadIndividualCommand>(_container)
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewPopulation)
            .WithId(MenuBarItemIds.NewPopulation)
            .WithCommand<AddRandomPopulationCommand>(_container)
            .WithDescription(PKSimConstants.UI.NewPopulationDescription)
            .WithIcon(ApplicationIcons.Population)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.P);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportPopulation)
            .WithId(MenuBarItemIds.ImportPopulation)
            .WithCommand<ImportPopulationCommand>(_container)
            .WithDescription(PKSimConstants.UI.ImportPopulationDescription)
            .WithIcon(ApplicationIcons.MergePopulation);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadPopulation)
            .WithCommand<LoadPopulationCommand>(_container)
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewAdministrationProtocol)
            .WithId(MenuBarItemIds.NewProtocol)
            .WithCommand<AddProtocolCommand>(_container)
            .WithDescription(PKSimConstants.UI.NewProtocolDescription)
            .WithIcon(ApplicationIcons.Protocol)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.A);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadProtocol)
            .WithCommand<LoadProtocolCommand>(_container)
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewFormulation)
            .WithId(MenuBarItemIds.NewFormulation)
            .WithCommand<AddFormulationCommand>(_container)
            .WithDescription(PKSimConstants.UI.NewFormulationDescription)
            .WithIcon(ApplicationIcons.Formulation)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.F);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadFormulationFromTemplate)
            .WithCommand<LoadFormulationCommand>(_container)
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewEvent)
            .WithId(MenuBarItemIds.NewEvent)
            .WithCommand<AddEventCommand>(_container)
            .WithDescription(PKSimConstants.UI.NewEventDescription)
            .WithIcon(ApplicationIcons.Event)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.E);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadEvent)
            .WithCommand<LoadEventCommand>(_container)
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewObservers)
            .WithId(MenuBarItemIds.NewObserverSet)
            .WithCommand<AddObserverSetCommand>(_container)
            .WithDescription(PKSimConstants.UI.NewObserversDescription)
            .WithIcon(ApplicationIcons.Observer);
//            .WithShortcut(Keys.Control | Keys.Alt | Keys.O);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadObserverSet)
            .WithCommand<LoadObserverSetCommand>(_container)
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         var newExpressionProfile = CreateSubMenu.WithCaption(MenuNames.NewExpressionProfile)
            .WithIcon(ApplicationIcons.ExpressionProfile)
            .WithDescription(MenuDescriptions.NewExpressionProfileDescription)
            .WithId(MenuBarItemIds.NewExpressionProfile);

         var newEnzyme = CreateMenuButton.WithCaption(PKSimConstants.UI.AddMetabolizingEnzyme)
            .WithId(MenuBarItemIds.NewExpressionProfileEnzyme)
            .WithCommand<AddExpressionProfileCommand<IndividualEnzyme>>(_container)
            .WithIcon(ApplicationIcons.Enzyme);

         var newTransporter = CreateMenuButton.WithCaption(PKSimConstants.UI.AddTransportProtein)
            .WithId(MenuBarItemIds.NewExpressionProfileTransporter)
            .WithCommand<AddExpressionProfileCommand<IndividualTransporter>>(_container)
            .WithIcon(ApplicationIcons.Transporter);

         var newSpecificBinding = CreateMenuButton.WithCaption(PKSimConstants.UI.AddSpecificBindingPartner)
            .WithId(MenuBarItemIds.NewExpressionProfileSpecificBindingPartner)
            .WithCommand<AddExpressionProfileCommand<IndividualOtherProtein>>(_container)
            .WithIcon(ApplicationIcons.Protein);

         newExpressionProfile.AddItem(newEnzyme);
         newExpressionProfile.AddItem(newTransporter);
         newExpressionProfile.AddItem(newSpecificBinding);

         yield return newExpressionProfile;

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadExpressionProfile)
            .WithCommand<LoadExpressionProfileCommand>(_container)
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.About)
            .WithId(MenuBarItemIds.About)
            .WithCommand<ShowAboutUICommand>(_container)
            .WithIcon(ApplicationIcons.Info)
            .WithDescription(PKSimConstants.UI.AboutThisApplication);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewCompound)
            .WithId(MenuBarItemIds.NewCompound)
            .WithCommand<AddCompoundUICommand>(_container)
            .WithDescription(PKSimConstants.UI.NewCompoundDescription)
            .WithIcon(ApplicationIcons.Compound)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.C);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadCompound)
            .WithCommand<LoadCompoundCommand>(_container)
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.GarbageCollection)
            .WithId(MenuBarItemIds.GarbageCollection)
            .WithCommand<GarbageCollectionCommand>(_container)
            .ForDeveloper();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.GenerateCalculationMethods)
            .WithId(MenuBarItemIds.GenerateCalculationMethods)
            .WithCommand<GenerateCalculationMethodsCommand>(_container)
            .WithIcon(ApplicationIcons.DistributionCalculation)
            .ForDeveloper();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.GeneratePKMLTemplates)
            .WithId(MenuBarItemIds.GeneratePKMLTemplates)
            .WithCommand<GeneratePKMLTemplatesCommand>(_container)
            .WithIcon(ApplicationIcons.PKML)
            .ForDeveloper();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.GenerateGroupsTemplate)
            .WithId(MenuBarItemIds.GenerateGroupsTemplate)
            .WithCommand<GenerateGroupsTemplateCommand>(_container)
            .WithIcon(ApplicationIcons.PKML)
            .ForDeveloper();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Options)
            .WithId(MenuBarItemIds.Options)
            .WithCommand<SettingsCommand>(_container)
            .WithDescription(PKSimConstants.UI.OptionsDescription)
            .WithIcon(ApplicationIcons.Settings)
            .WithShortcut(Keys.Control | Keys.Shift | Keys.O);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Exit)
            .WithId(MenuBarItemIds.Exit)
            .WithCommand<IExitCommand>(_container)
            .WithDescription(PKSimConstants.UI.ExitDescription)
            .WithIcon(ApplicationIcons.Exit)
            .WithShortcut(Keys.Alt | Keys.F4);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Run)
            .WithId(MenuBarItemIds.Run)
            .WithDescription(PKSimConstants.UI.RunDescription)
            .WithCommand<RunSimulationCommand>(_container)
            .WithIcon(ApplicationIcons.Run)
            .WithShortcut(Keys.F5);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.RunWithSettings)
            .WithId(MenuBarItemIds.RunWithSettings)
            .WithDescription(PKSimConstants.UI.RunWithSettingsDescription)
            .WithCommand<RunSimulationWithSettingsCommand>(_container)
            .WithIcon(ApplicationIcons.ConfigureAndRun);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Stop)
            .WithId(MenuBarItemIds.Stop)
            .WithDescription(PKSimConstants.UI.StopDescription)
            .WithCommand<StopSimulationCommand>(_container)
            .WithIcon(ApplicationIcons.Stop);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.TimeProfileAnalysis)
            .WithId(MenuBarItemIds.ShowIndividualResults)
            .WithDescription(PKSimConstants.UI.ShowIndividualResultsDescription)
            .WithCommand<ShowSimulationResultsCommand>(_container)
            .WithIcon(ApplicationIcons.TimeProfileAnalysis);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.BoxWhiskerAnalysis)
            .WithId(MenuBarItemIds.BoxWhiskerAnalysis)
            .WithDescription(PKSimConstants.UI.BoxWhiskerAnalysisDescription)
            .WithCommand<StartBoxWhiskerAnalysisCommand>(_container)
            .WithIcon(ApplicationIcons.BoxWhiskerAnalysis);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ScatterAnalysis)
            .WithId(MenuBarItemIds.ScatterAnalysis)
            .WithCommand<StartScatterAnalysisCommand>(_container)
            .WithDescription(PKSimConstants.UI.ScatterAnalysisDescription)
            .WithIcon(ApplicationIcons.ScatterAnalysis);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.RangeAnalysis)
            .WithId(MenuBarItemIds.RangeAnalysis)
            .WithCommand<StartRangeAnalysisCommand>(_container)
            .WithDescription(PKSimConstants.UI.RangeAnalysisDescription)
            .WithIcon(ApplicationIcons.RangeAnalysis);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.TimeProfileAnalysis)
            .WithId(MenuBarItemIds.TimeProfileAnalysis)
            .WithCommand<StartTimeProfileAnalysisCommand>(_container)
            .WithDescription(PKSimConstants.UI.TimeProfileAnalysisDescription)
            .WithIcon(ApplicationIcons.TimeProfileAnalysis);

         yield return CreateMenuButton.WithCaption(Captions.SimulationUI.PredictedVsObservedSimulation)
            .WithId(MenuBarItemIds.PredictedVsObservedSimulationAnalysis)
            .WithDescription(MenuDescriptions.PredictedVsObservedAnalysisDescription)
            .WithCommand<StartPredictedVsObservedSimulationAnalysisUICommand>(_container)
            .WithIcon(ApplicationIcons.PredictedVsObservedAnalysis);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.IndividualSimulationComparison)
            .WithId(MenuBarItemIds.IndividualSimulationComparison)
            .WithDescription(PKSimConstants.UI.IndividualSimulationComparisonDescription)
            .WithCommand<CreateIndividualSimulationComparisonUICommand>(_container)
            .WithIcon(ApplicationIcons.IndividualSimulationComparison);

         yield return CreateMenuButton.WithCaption(Captions.SimulationUI.ResidualsVsTimeSimulation)
            .WithId(MenuBarItemIds.ResidualsVsTimeSimulationAnalysis)
            .WithDescription(MenuDescriptions.ResidualsVsTimeAnalysisDescription)
            .WithCommand<StartResidualVsTimeSimulationAnalysisUICommand>(_container)
            .WithIcon(ApplicationIcons.ResidualVsTimeAnalysis);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Comparison)
            .WithId(MenuBarItemIds.IndividualSimulationComparisonInAnalyze)
            .WithDescription(PKSimConstants.UI.IndividualSimulationComparisonDescription)
            .WithCommand<CreateIndividualSimulationComparisonUICommand>(_container)
            .WithIcon(ApplicationIcons.IndividualSimulationComparison);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.PopulationSimulationComparison)
            .WithId(MenuBarItemIds.PopulationSimulationComparison)
            .WithDescription(PKSimConstants.UI.PopulationSimulationComparisonDescription)
            .WithCommand<CreatePopulationSimulationComparisonUICommand>(_container)
            .WithIcon(ApplicationIcons.PopulationSimulationComparison);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Comparison)
            .WithId(MenuBarItemIds.PopulationSimulationComparisonInAnalyze)
            .WithDescription(PKSimConstants.UI.PopulationSimulationComparisonDescription)
            .WithCommand<CreatePopulationSimulationComparisonUICommand>(_container)
            .WithIcon(ApplicationIcons.PopulationSimulationComparison);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.HistoryView)
            .WithId(MenuBarItemIds.HistoryView)
            .WithDescription(PKSimConstants.UI.HistoryViewDescription)
            .WithCommand<HistoryVisibilityCommand>(_container)
            .WithIcon(ApplicationIcons.History)
            .WithShortcut(Keys.Control | Keys.Shift | Keys.H);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ComparisonView)
            .WithId(MenuBarItemIds.ComparisonView)
            .WithDescription(PKSimConstants.UI.ComparisonViewDescription)
            .WithCommand<ComparisonVisibilityUICommand>(_container)
            .WithIcon(ApplicationIcons.Comparison)
            .WithShortcut(Keys.Control | Keys.Shift | Keys.N);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SimulationExplorerView)
            .WithId(MenuBarItemIds.SimulationExplorerView)
            .WithDescription(PKSimConstants.UI.SimulationExplorerViewDescription)
            .WithCommand<SimulationExplorerVisibilityCommand>(_container)
            .WithIcon(ApplicationIcons.SimulationExplorer)
            .WithShortcut(Keys.Control | Keys.Shift | Keys.S);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.BuildingBlockExplorerView)
            .WithId(MenuBarItemIds.BuildingBlockExplorerView)
            .WithDescription(PKSimConstants.UI.BuildingBlockExplorerViewDescription)
            .WithCommand<BuildingBlockExplorerVisibilityCommand>(_container)
            .WithIcon(ApplicationIcons.BuildingBlockExplorer)
            .WithShortcut(Keys.Control | Keys.Shift | Keys.B);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AddObservedData)
            .WithId(MenuBarItemIds.AddObservedData)
            .WithDescription(PKSimConstants.UI.AddObservedDataDescription)
            .WithCommand<ImportConcentrationDataCommand>(_container)
            .WithIcon(ApplicationIcons.ObservedData);

         yield return CreateSubMenu.WithCaption(PKSimConstants.MenuNames.ExportHistory)
            .WithId(MenuBarItemIds.HistoryReportGroup)
            .WithIcon(ApplicationIcons.HistoryExport);

         yield return CreateMenuButton.WithCaption(MenuNames.ExportToExcel)
            .WithId(MenuBarItemIds.HistoryReportExcel)
            .WithDescription(PKSimConstants.UI.ExportHistoryToExcelDescription)
            .WithCommand<ExportHistoryToExcelCommand>(_container)
            .WithIcon(ApplicationIcons.Excel);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.CloneMenu)
            .WithId(MenuBarItemIds.CloneActiveSimulation)
            .WithDescription(PKSimConstants.UI.CloneSimulation)
            .WithCommand<CloneSimulationCommand>(_container)
            .WithIcon(ApplicationIcons.SimulationClone);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ConfigureShortMenu)
            .WithId(MenuBarItemIds.ConfigureActiveSimulation)
            .WithDescription(PKSimConstants.UI.ConfigureSimulationDescription)
            .WithCommand<ConfigureSimulationCommand>(_container)
            .WithIcon(ApplicationIcons.SimulationConfigure);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportToMoBiShortMEnu)
            .WithId(MenuBarItemIds.ExportActiveSimulationToMoBi)
            .WithDescription(PKSimConstants.UI.ExportActiveSimulationToMoBiDescription)
            .WithCommand<ExportSimulationToMoBiCommand>(_container)
            .WithIcon(ApplicationIcons.MoBi);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportToPKMLShortMenu)
            .WithId(MenuBarItemIds.ExportActiveSimulationToPkml)
            .WithDescription(PKSimConstants.UI.ExportSimulationToMoBiTitle)
            .WithCommand<SaveSimulationToMoBiFileCommand>(_container)
            .WithIcon(ApplicationIcons.PKMLSave);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSimulationResultsToExcelMenu)
            .WithId(MenuBarItemIds.ExportActiveSimulationResultsToExcel)
            .WithDescription(PKSimConstants.UI.ExportSimulationResultsToExcel)
            .WithCommand<ExportSimulationResultsToExcelCommand>(_container)
            .WithIcon(ApplicationIcons.ExportToExcel);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSimulationResultsToCSVMenu)
            .WithId(MenuBarItemIds.ExportActiveSimulationResultsToCSV)
            .WithDescription(PKSimConstants.UI.ExportSimulationResultsToCSV)
            .WithCommand<ExportSimulationResultsToCSVCommand>(_container)
            .WithIcon(ApplicationIcons.ExportToCSV);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportPopulationToCSVMenu)
            .WithId(MenuBarItemIds.ExportActiveSimulationPopulationToExcel)
            .WithDescription(PKSimConstants.UI.ExportPopulationToCSVTitle)
            .WithCommand<ExportPopulationSimulationToCSVCommand>(_container)
            .WithIcon(ApplicationIcons.PopulationExportToCSV);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportPKAnalysesToCSVMenu)
            .WithId(MenuBarItemIds.ExportActiveSimulationPKAnalysesToCSV)
            .WithDescription(PKSimConstants.MenuNames.ExportPKAnalysesToCSV)
            .WithCommand<ExportPopulationSimulationPKAnalysesCommand>(_container)
            .WithIcon(ApplicationIcons.PKAnalysesExportToCSV);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportForClusterComputationsMenu)
            .WithId(MenuBarItemIds.ExportActiveSimulationForClusterComputations)
            .WithCommand<ExportPopulationSimulationForClusterCommand>(_container)
            .WithIcon(ApplicationIcons.ClusterExport);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportSimulationResultsFromCSVMenu)
            .WithId(MenuBarItemIds.ImportActiveSimulationResults)
            .WithCommand<ImportSimulationResultsCommand>(_container)
            .WithIcon(ApplicationIcons.ResultsImportFromCSV);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportPKAnalysesFromCSVMenu)
            .WithId(MenuBarItemIds.ImportActiveSimulationPKParameters)
            .WithIcon(ApplicationIcons.PKAnalysesImportFromCSV)
            .WithCommand<ImportPKAnalysesCommand>(_container);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadPopulationAnalysisWorkflowFromTemplateMenu)
            .WithId(MenuBarItemIds.LoadPopulationSimulationWorkflow)
            .WithDescription(PKSimConstants.UI.LoadPopulationAnalysisWorkflowFromTemplateDescription)
            .WithIcon(ApplicationIcons.AnalysesLoad)
            .WithCommand<LoadPopulationAnalysisWorkflowFromTemplateUICommand>(_container);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SavePopulationAnalysisWorkflowToTemplateMenu)
            .WithId(MenuBarItemIds.SavePopulationSimulationWorkflow)
            .WithDescription(PKSimConstants.UI.SavePopulationAnalysisWorkflowToTemplateDescription)
            .WithIcon(ApplicationIcons.AnalysesSave)
            .WithCommand<SavePopulationAnalysisWorkflowToTemplateUICommand>(_container);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.RemoveUnusedContent)
            .WithId(MenuBarItemIds.RemoveUnusedContent)
            .WithCommand<RemoveUnusedContentUICommand>(_container)
            .WithIcon(ApplicationIcons.Delete)
            .ForDeveloper();

         yield return CommonMenuBarButtons.ManageUserDisplayUnits(MenuBarItemIds.ManageUserDisplayUnits, _container);
         yield return CommonMenuBarButtons.ManageProjectDisplayUnits(MenuBarItemIds.ManageProjectDisplayUnits, _container);
         yield return CommonMenuBarButtons.UpdateAllToDisplayUnits(MenuBarItemIds.UpdateAllToDisplayUnits, _container);
         yield return CommonMenuBarButtons.ClearHistory(MenuBarItemIds.ClearHistory, _container);
         yield return CommonMenuBarButtons.Help(MenuBarItemIds.Help, _container);

         yield return JournalMenuBarButtons.JournalView(MenuBarItemIds.JournalView, _container);
         yield return JournalMenuBarButtons.CreateJournalPage(MenuBarItemIds.CreateJournalPage, _container);
         yield return JournalMenuBarButtons.SelectJournal(MenuBarItemIds.SelectJournal, _container);
         yield return JournalMenuBarButtons.JournalEditorView(MenuBarItemIds.JournalEditorView, _container);
         yield return CommonMenuBarButtons.JournalDiagramView(MenuBarItemIds.JournalDiagramView, _container);
         yield return JournalMenuBarButtons.SearchJournal(MenuBarItemIds.SearchJournal, _container);
         yield return CommonMenuBarButtons.LoadFavoritesFromFile(MenuBarItemIds.LoadFavorites, _container);
         yield return CommonMenuBarButtons.SaveFavoritesToFile(MenuBarItemIds.SaveFavorites, _container);
         yield return JournalMenuBarButtons.ExportJournal(MenuBarItemIds.ExportJournal, _container);
         yield return JournalMenuBarButtons.RefreshJournal(MenuBarItemIds.RefreshJournal, _container);

         yield return ParameterIdentificationMenuBarButtons.CreateParameterIdentification(MenuBarItemIds.CreateParameterIdentification, _container);
         yield return ParameterIdentificationMenuBarButtons.RunParameterIdentification(MenuBarItemIds.RunParameterIdentification, _container);
         yield return ParameterIdentificationMenuBarButtons.StopParameterIdentification(MenuBarItemIds.StopParameterIdentification, _container);
         yield return ParameterIdentificationMenuBarButtons.TimeProfileParameterIdentification(MenuBarItemIds.TimeProfileParameterIdentification, _container);
         yield return ParameterIdentificationMenuBarButtons.PredictedVsObservedParameterIdentification(MenuBarItemIds
            .PredictedVsObservedParameterIdentification, _container);
         yield return ParameterIdentificationMenuBarButtons.ResidualsVsTimeParameterIdentification(MenuBarItemIds
            .ResidualsVsTimeParameterIdentifcation, _container);
         yield return ParameterIdentificationMenuBarButtons.ResidualHistogramParameterIdentification(MenuBarItemIds
            .ResidualHistogramParameterIdentification, _container);
         yield return ParameterIdentificationMenuBarButtons.CorrelationMatrixParameterIdentification(MenuBarItemIds
            .CorrelationMatrixParameterIdentification, _container);
         yield return ParameterIdentificationMenuBarButtons.CovarianceMatrixParameterIdentification(MenuBarItemIds
            .CovarianceMatrixParameterIdentification, _container);
         yield return ParameterIdentificationMenuBarButtons.ParameterIdentificationFeedbackView(MenuBarItemIds.ParameterIdentificationFeedbackView, _container);
         yield return ParameterIdentificationMenuBarButtons.TimeProfilePredictionInterval(MenuBarItemIds.TimeProfilePredictionInterval, _container);
         yield return ParameterIdentificationMenuBarButtons.TimeProfileVPCInterval(MenuBarItemIds.TimeProfileVPCInterval, _container);
         yield return ParameterIdentificationMenuBarButtons.TimeProfileConfidenceInterval(MenuBarItemIds.TimeProfileConfidenceInterval, _container);

         yield return SensitivityAnalysisMenuBarButtons.SensitivityAnalysisPKParameterAnalysis(MenuBarItemIds.SensitivityAnalysisPKParameterAnalysis, _container);
         yield return SensitivityAnalysisMenuBarButtons.CreateSensitivityAnalysis(MenuBarItemIds.CreateSensitivityAnalysis, _container);
         yield return SensitivityAnalysisMenuBarButtons.SensitivityAnalysisFeedbackView(MenuBarItemIds.SensitivityAnalysisFeedbackView, _container);
         yield return SensitivityAnalysisMenuBarButtons.RunSensitivityAnalysis(MenuBarItemIds.RunSensitivityAnalysis, _container);
         yield return SensitivityAnalysisMenuBarButtons.StopSensitivityAnalysis(MenuBarItemIds.StopSensitivityAnalysis, _container);
      }
   }
}