using System.Collections.Generic;
using System.Windows.Forms;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Container;
using PKSim.Presentation.Core;
using PKSim.Presentation.UICommands;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Assets;
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
            .WithCommand<NewProjectCommand>()
            .WithIcon(ApplicationIcons.ProjectNew)
            .WithDescription(PKSimConstants.UI.NewProjectDescription)
            .WithShortcut(Keys.Control | Keys.N);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.OpenProject)
            .WithId(MenuBarItemIds.OpenProject)
            .WithCommand<OpenProjectCommand>()
            .WithDescription(PKSimConstants.UI.OpenProjectDescription)
            .WithIcon(ApplicationIcons.ProjectOpen)
            .WithShortcut(Keys.Control | Keys.O);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ProjectDescription)
            .WithId(MenuBarItemIds.ProjectDescription)
            .WithCommand<EditProjectDescriptionCommand>()
            .WithDescription(PKSimConstants.UI.ProjectDescriptionDescription)
            .WithIcon(ApplicationIcons.ProjectOpen)
            .WithShortcut(Keys.Control | Keys.D);

         //TODO UPDATE ICON
         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportProjectToSnapshot)
            .WithId(MenuBarItemIds.ExportProjectToSnapshot)
            .WithCommand<ExportProjectToSnapshotCommand>()
            .WithDescription(PKSimConstants.UI.ExportProjectToSnapshotDescription)
            .WithIcon(ApplicationIcons.Save);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadProjectFromSnapshot)
            .WithId(MenuBarItemIds.LoadProjectFromSnahpshot)
            .WithCommand< LoadProjectFromSnapshotCommand>()
            .WithDescription(PKSimConstants.UI.LoadProjectFromSnapshotDescription)
            .WithIcon(ApplicationIcons.ProjectOpen);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.CloseProject)
            .WithId(MenuBarItemIds.CloseProject)
            .WithDescription(PKSimConstants.UI.CloseProjectDescription)
            .WithCommand<CloseProjectCommand>()
            .WithIcon(ApplicationIcons.Close);

         yield return CreateSubMenu.WithCaption(PKSimConstants.MenuNames.SaveProject)
            .WithId(MenuBarItemIds.SaveGroup)
            .WithDescription(PKSimConstants.UI.SaveProjectDescription)
            .WithIcon(ApplicationIcons.Save);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SaveProject)
            .WithId(MenuBarItemIds.SaveProject)
            .WithDescription(PKSimConstants.UI.SaveProjectDescription)
            .WithCommand<SaveProjectCommand>()
            .WithIcon(ApplicationIcons.Save)
            .WithShortcut(Keys.Control | Keys.S);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SaveProjectAs)
            .WithId(MenuBarItemIds.SaveProjectAs)
            .WithDescription(PKSimConstants.UI.SaveProjectAsDescription)
            .WithIcon(ApplicationIcons.SaveAs)
            .WithCommand<SaveProjectAsCommand>();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewSimulation)
            .WithId(MenuBarItemIds.NewSimulation)
            .WithCommand<NewSimulationCommand>()
            .WithDescription(PKSimConstants.UI.NewSimulationDescription)
            .WithIcon(ApplicationIcons.Simulation)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.S);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportIndividualSimulation)
            .WithId(MenuBarItemIds.NewImportIndividualSimulation)
            .WithCommand<NewImportIndividualSimulationCommand>()
            .WithDescription(PKSimConstants.UI.ImportIndividualSimulationDescription)
            .WithIcon(ApplicationIcons.IndividualSimulationLoad);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportPopulationSimulation)
            .WithId(MenuBarItemIds.NewImportPopulationSimulation)
            .WithCommand<NewImportPopulationSimulationCommand>()
            .WithDescription(PKSimConstants.UI.ImportPopulationSimulationDescription)
            .WithIcon(ApplicationIcons.PopulationSimulationLoad);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewIndividual)
            .WithId(MenuBarItemIds.NewIndividual)
            .WithCommand<NewIndividualCommand>()
            .WithDescription(PKSimConstants.UI.NewIndividualDescription)
            .WithIcon(ApplicationIcons.Individual)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.I);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadIndividual)
            .WithCommand<LoadIndividualCommand>()
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewPopulation)
            .WithId(MenuBarItemIds.NewPopulation)
            .WithCommand<NewRandomPopulationCommand>()
            .WithDescription(PKSimConstants.UI.NewPopulationDescription)
            .WithIcon(ApplicationIcons.Population)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.P);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportPopulation)
            .WithId(MenuBarItemIds.ImportPopulation)
            .WithCommand<ImportPopulationCommand>()
            .WithDescription(PKSimConstants.UI.ImportPopulationDescription)
            .WithIcon(ApplicationIcons.MergePopulation);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadPopulation)
            .WithCommand<LoadPopulationCommand>()
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewAdministrationProtocol)
            .WithId(MenuBarItemIds.NewProtocol)
            .WithCommand<NewProtocolCommand>()
            .WithDescription(PKSimConstants.UI.NewProtocolDescription)
            .WithIcon(ApplicationIcons.Protocol)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.A);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadProtocol)
            .WithCommand<LoadProtocolCommand>()
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewFormulation)
            .WithId(MenuBarItemIds.NewFormulation)
            .WithCommand<NewFormulationCommand>()
            .WithDescription(PKSimConstants.UI.NewFormulationDescription)
            .WithIcon(ApplicationIcons.Formulation)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.F);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadFormulationFromTemplate)
            .WithCommand<LoadFormulationCommand>()
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewEvent)
            .WithId(MenuBarItemIds.NewEvent)
            .WithCommand<NewEventCommand>()
            .WithDescription(PKSimConstants.UI.NewEventDescription)
            .WithIcon(ApplicationIcons.Event)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.E);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadEvent)
            .WithCommand<LoadEventCommand>()
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.About)
            .WithId(MenuBarItemIds.About)
            .WithCommand<ShowAboutUICommand>()
            .WithIcon(ApplicationIcons.Info)
            .WithDescription(PKSimConstants.UI.AboutThisApplication);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.NewCompound)
            .WithId(MenuBarItemIds.NewCompound)
            .WithCommand<AddCompoundUICommand>()
            .WithDescription(PKSimConstants.UI.NewCompoundDescription)
            .WithIcon(ApplicationIcons.Compound)
            .WithShortcut(Keys.Control | Keys.Alt | Keys.C);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromTemplate)
            .WithId(MenuBarItemIds.LoadCompound)
            .WithCommand<LoadCompoundCommand>()
            .WithIcon(ApplicationIcons.LoadFromTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.GarbageCollection)
            .WithId(MenuBarItemIds.GarbageCollection)
            .WithCommand<GarbageCollectionCommand>()
            .ForDeveloper();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.GenerateCalculationMethods)
            .WithId(MenuBarItemIds.GenerateCalculationMethods)
            .WithCommand<GenerateCalculationMethodsCommand>()
            .WithIcon(ApplicationIcons.DistributionCalculation)
            .ForDeveloper();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.GeneratePKMLTemplates)
            .WithId(MenuBarItemIds.GeneratePKMLTemplates)
            .WithCommand<GeneratePKMLTemplatesCommand>()
            .WithIcon(ApplicationIcons.PKML)
            .ForDeveloper();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.GenerateGroupsTemplate)
            .WithId(MenuBarItemIds.GenerateGroupsTemplate)
            .WithCommand<GenerateGroupsTemplateCommand>()
            .WithIcon(ApplicationIcons.PKML)
            .ForDeveloper();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Options)
            .WithId(MenuBarItemIds.Options)
            .WithCommand<SettingsCommand>()
            .WithDescription(PKSimConstants.UI.OptionsDescription)
            .WithIcon(ApplicationIcons.Settings)
            .WithShortcut(Keys.Control | Keys.Shift | Keys.O);
            
         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Exit)
            .WithId(MenuBarItemIds.Exit)
            .WithCommand<IExitCommand>()
            .WithDescription(PKSimConstants.UI.ExitDescription)
            .WithIcon(ApplicationIcons.Exit)
            .WithShortcut(Keys.Alt | Keys.F4);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Run)
            .WithId(MenuBarItemIds.Run)
            .WithDescription(PKSimConstants.UI.RunDescription)
            .WithCommand<RunSimulationCommand>()
            .WithIcon(ApplicationIcons.Run)
            .WithShortcut(Keys.F5);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.RunWithSettings)
            .WithId(MenuBarItemIds.RunWithSettings)
            .WithDescription(PKSimConstants.UI.RunWithSettingsDescription)
            .WithCommand<RunSimulationWithSettingsCommand>()
            .WithIcon(ApplicationIcons.ConfigureAndRun);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Stop)
            .WithId(MenuBarItemIds.Stop)
            .WithDescription(PKSimConstants.UI.StopDescription)
            .WithCommand<StopSimulationCommand>()
            .WithIcon(ApplicationIcons.Stop);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.TimeProfileAnalysis)
            .WithId(MenuBarItemIds.ShowIndividualResults)
            .WithDescription(PKSimConstants.UI.ShowIndividualResultsDescription)
            .WithCommand<ShowSimulationResultsCommand>()
            .WithIcon(ApplicationIcons.TimeProfileAnalysis);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.BoxWhiskerAnalysis)
            .WithId(MenuBarItemIds.BoxWhiskerAnalysis)
            .WithDescription(PKSimConstants.UI.BoxWhiskerAnalysisDescription)
            .WithCommand<StartBoxWhiskerAnalysisCommand>()
            .WithIcon(ApplicationIcons.BoxWhiskerAnalysis);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ScatterAnalysis)
            .WithId(MenuBarItemIds.ScatterAnalysis)
            .WithCommand<StartScatterAnalysisCommand>()
            .WithDescription(PKSimConstants.UI.ScatterAnalysisDescription)
            .WithIcon(ApplicationIcons.ScatterAnalysis);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.RangeAnalysis)
            .WithId(MenuBarItemIds.RangeAnalysis)
            .WithCommand<StartRangeAnalysisCommand>()
            .WithDescription(PKSimConstants.UI.RangeAnalysisDescription)
            .WithIcon(ApplicationIcons.RangeAnalysis);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.TimeProfileAnalysis)
            .WithId(MenuBarItemIds.TimeProfileAnalysis)
            .WithCommand<StartTimeProfileAnalysisCommand>()
            .WithDescription(PKSimConstants.UI.TimeProfileAnalysisDescription)
            .WithIcon(ApplicationIcons.TimeProfileAnalysis);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.IndividualSimulationComparison)
            .WithId(MenuBarItemIds.IndividualSimulationComparison)
            .WithDescription(PKSimConstants.UI.IndividualSimulationComparisonDescription)
            .WithCommand<CreateIndividualSimulationComparisonUICommand>()
            .WithIcon(ApplicationIcons.IndividualSimulationComparison);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Comparison)
           .WithId(MenuBarItemIds.IndividualSimulationComparisonInAnalyze)
           .WithDescription(PKSimConstants.UI.IndividualSimulationComparisonDescription)
           .WithCommand<CreateIndividualSimulationComparisonUICommand>()
           .WithIcon(ApplicationIcons.IndividualSimulationComparison);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.PopulationPopulationComparison)
            .WithId(MenuBarItemIds.PopulationSimulationComparison)
            .WithDescription(PKSimConstants.UI.PopulationSimulationComparisonDescription)
            .WithCommand<CreatePopulationSimulationComparisonUICommand>()
            .WithIcon(ApplicationIcons.PopulationSimulationComparison);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Comparison)
            .WithId(MenuBarItemIds.PopulationSimulationComparisonInAnalyze)
            .WithDescription(PKSimConstants.UI.PopulationSimulationComparisonDescription)
            .WithCommand<CreatePopulationSimulationComparisonUICommand>()
            .WithIcon(ApplicationIcons.PopulationSimulationComparison);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.HistoryView)
            .WithId(MenuBarItemIds.HistoryView)
            .WithDescription(PKSimConstants.UI.HistoryViewDescription)
            .WithCommand<HistoryVisibilityCommand>()
            .WithIcon(ApplicationIcons.History)
            .WithShortcut(Keys.Control | Keys.Shift | Keys.H);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ComparisonView)
            .WithId(MenuBarItemIds.ComparisonView)
            .WithDescription(PKSimConstants.UI.ComparisonViewDescription)
            .WithCommand<ComparisonVisibilityUICommand>()
            .WithIcon(ApplicationIcons.Comparison)
            .WithShortcut(Keys.Control | Keys.Shift | Keys.N);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SimulationExplorerView)
            .WithId(MenuBarItemIds.SimulationExplorerView)
            .WithDescription(PKSimConstants.UI.SimulationExplorerViewDescription)
            .WithCommand<SimulationExplorerVisibilityCommand>()
            .WithIcon(ApplicationIcons.SimulationExplorer)
            .WithShortcut(Keys.Control | Keys.Shift | Keys.S);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.BuildingBlockExplorerView)
            .WithId(MenuBarItemIds.BuildingBlockExplorerView)
            .WithDescription(PKSimConstants.UI.BuildingBlockExplorerViewDescription)
            .WithCommand<BuildingBlockExplorerVisibilityCommand>()
            .WithIcon(ApplicationIcons.BuildingBlockExplorer)
            .WithShortcut(Keys.Control | Keys.Shift | Keys.B);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AddObservedData)
            .WithId(MenuBarItemIds.ImportObservedData)
            .WithDescription(PKSimConstants.UI.AddObservedDataDescription)
            .WithCommand<ImportConcentrationDataCommmand>()
            .WithIcon(ApplicationIcons.ObservedData);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AddFractionData)
          .WithId(MenuBarItemIds.ImportFractionData)
          .WithDescription(PKSimConstants.UI.AddFractionDataDescription)
          .WithCommand<ImportFractionDataCommand>()
          .WithIcon(ApplicationIcons.FractionData);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ProjectReport)
            .WithId(MenuBarItemIds.ProjectReport)
            .WithDescription(PKSimConstants.UI.ProjectReportDescription)
            .WithCommand<ExportProjectToPDFCommand>()
            .WithIcon(ApplicationIcons.PDF);

         yield return CreateSubMenu.WithCaption(PKSimConstants.MenuNames.ExportHistory)
            .WithId(MenuBarItemIds.HistoryReportGroup)
            .WithIcon(ApplicationIcons.HistoryExport);

         yield return CreateMenuButton.WithCaption(MenuNames.ExportToExcel)
            .WithId(MenuBarItemIds.HistoryReportExcel)
            .WithDescription(PKSimConstants.UI.ExportHistoryToExcelDescription)
            .WithCommand<ExportHistoryToExcelCommand>()
            .WithIcon(ApplicationIcons.Excel);

         yield return CreateMenuButton.WithCaption(MenuNames.ExportToPDF)
            .WithId(MenuBarItemIds.HistoryReportPDF)
            .WithDescription(PKSimConstants.UI.ExportHistoryToPDFDescription)
            .WithIcon(ApplicationIcons.PDF)
            .WithCommand<ExportHistoryToPDFCommand>();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Help)
            .WithId(MenuBarItemIds.Help)
            .WithIcon(ApplicationIcons.Help)
            .WithCommand<ShowHelpCommand>()
            .WithShortcut(Keys.F1);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.CloneMenu)
            .WithId(MenuBarItemIds.CloneActiveSimulation)
            .WithDescription(PKSimConstants.UI.CloneSimulation)
            .WithCommand<CloneSimulationCommand>()
            .WithIcon(ApplicationIcons.SimulationClone);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ConfigureShortMenu)
            .WithId(MenuBarItemIds.ConfigureActiveSimulation)
            .WithDescription(PKSimConstants.UI.ConfigureSimulationDescription)
            .WithCommand<ConfigureSimulationCommand>()
            .WithIcon(ApplicationIcons.SimulationConfigure);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportToMoBiShortMEnu)
            .WithId(MenuBarItemIds.ExportActiveSimulationToMoBi)
            .WithDescription(PKSimConstants.UI.ExportActiveSimulationToMoBiDescription)
            .WithCommand<ExportSimulationToMoBiCommand>()
            .WithIcon(ApplicationIcons.MoBi);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportToPKMLShortMenu)
            .WithId(MenuBarItemIds.ExportActiveSimulationToPkml)
            .WithDescription(PKSimConstants.UI.ExportSimulationToMoBiTitle)
            .WithCommand<SaveSimulationToMoBiFileCommand>()
            .WithIcon(ApplicationIcons.PKMLSave);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSimulationResultsToExcelMenu)
            .WithId(MenuBarItemIds.ExportActiveSimulationResultsToExcel)
            .WithDescription(PKSimConstants.UI.ExportSimulationResultsToExcel)
            .WithCommand<ExportSimulationResultsToExcelCommand>()
            .WithIcon(ApplicationIcons.ExportToExcel);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSimulationResultsToCSVMenu)
            .WithId(MenuBarItemIds.ExportActiveSimulationResultsToCSV)
            .WithDescription(PKSimConstants.UI.ExportSimulationResultsToCSV)
            .WithCommand<ExportSimulationResultsToCSVCommand>()
            .WithIcon(ApplicationIcons.ExportToCSV);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportPopulationToCSVMenu)
            .WithId(MenuBarItemIds.ExportActiveSimulationPopulationToExcel)
            .WithDescription(PKSimConstants.UI.ExportPopulationToCSVTitle)
            .WithCommand<ExportPopulationSimulationToCSVCommand>()
            .WithIcon(ApplicationIcons.PopulationExportToCSV);


         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportPKAnalysesToCSVMenu)
          .WithId(MenuBarItemIds.ExportActiveSimulationPKAnalysesToCSV)
          .WithDescription(PKSimConstants.MenuNames.ExportPKAnalysesToCSV)
          .WithCommand<ExportPopulationSimulationPKAnalysesCommand>()
          .WithIcon(ApplicationIcons.PKAnalysesExportToCSV);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportForClusterComputationsMenu)
            .WithId(MenuBarItemIds.ExportActiveSimulationForClusterComputations)
            .WithCommand<ExportPopulationSimulationForClusterCommand>()
            .WithIcon(ApplicationIcons.ClusterExport);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportSimulationResultsFromCSVMenu)
            .WithId(MenuBarItemIds.ImportActiveSimulationResults)
            .WithCommand<ImportSimulationResultsCommand>()
            .WithIcon(ApplicationIcons.ResultsImportFromCSV);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ImportPKAnalysesFromCSVMenu)
            .WithId(MenuBarItemIds.ImportActiveSimulationPKParameters)
            .WithIcon(ApplicationIcons.PKAnalysesImportFromCSV)
            .WithCommand<ImportPKAnalysesCommand>();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSimulationToPDFMenu)
            .WithId(MenuBarItemIds.ExportActiveSimulationToPDF)
            .WithIcon(ApplicationIcons.ExportToPDF)
            .WithCommand<ExportActiveSimulationToPDFCommand>();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadPopulationAnalysisWorkflowFromTemplateMenu)
            .WithId(MenuBarItemIds.LoadPopulationSimulationWorkflow)
            .WithDescription(PKSimConstants.UI.LoadPopulationAnalysisWorkflowFromTemplateDescription)
            .WithIcon(ApplicationIcons.AnalysesLoad)
            .WithCommand<LoadPopulationAnalysisWorkflowFromTemplateUICommand>();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SavePopulationAnalysisWorkflowToTemplateMenu)
            .WithId(MenuBarItemIds.SavePopulationSimulationWorkflow)
            .WithDescription(PKSimConstants.UI.SavePopulationAnalysisWorkflowToTemplateDescription)
            .WithIcon(ApplicationIcons.AnalysesSave)
            .WithCommand<SavePopulationAnalysisWorkflowToTemplateUICommand>();

         yield return CommonMenuBarButtons.ManageUserDisplayUnits(MenuBarItemIds.ManageUserDisplayUnits);
         yield return CommonMenuBarButtons.ManageProjectDisplayUnits(MenuBarItemIds.ManageProjectDisplayUnits);
         yield return CommonMenuBarButtons.UpdateAllToDisplayUnits(MenuBarItemIds.UpdateAllToDisplayUnits);
         yield return JournalMenuBarButtons.JournalView(MenuBarItemIds.JournalView);
         yield return JournalMenuBarButtons.CreateJournalPage(MenuBarItemIds.CreateJournalPage);
         yield return JournalMenuBarButtons.SelectJournal(MenuBarItemIds.SelectJournal);
         yield return JournalMenuBarButtons.JournalEditorView(MenuBarItemIds.JournalEditorView);
         yield return CommonMenuBarButtons.JournalDiagramView(MenuBarItemIds.JournalDiagramView);
         yield return JournalMenuBarButtons.SearchJournal(MenuBarItemIds.SearchJournal);
         yield return CommonMenuBarButtons.LoadFavoritesFromFile(MenuBarItemIds.LoadFavorites);
         yield return CommonMenuBarButtons.SaveFavoritesToFile(MenuBarItemIds.SaveFavorites);
         yield return JournalMenuBarButtons.ExportJournal(MenuBarItemIds.ExportJournal);
         yield return JournalMenuBarButtons.RefreshJournal(MenuBarItemIds.RefreshJournal);

         yield return ParameterIdentificationMenuBarButtons.CreateParameterIdentification(MenuBarItemIds.CreateParameterIdentification);
         yield return ParameterIdentificationMenuBarButtons.RunParameterIdentification(MenuBarItemIds.RunParameterIdentification);
         yield return ParameterIdentificationMenuBarButtons.StopParameterIdentification(MenuBarItemIds.StopParameterIdentification);
         yield return ParameterIdentificationMenuBarButtons.TimeProfileParameterIdentification(MenuBarItemIds.TimeProfileParameterIdentification);
         yield return ParameterIdentificationMenuBarButtons.PredictedVsObservedParameterIdentification(MenuBarItemIds.PredictedVsObservedParameterIdentification);
         yield return ParameterIdentificationMenuBarButtons.ResidualsVsTimeParameterIdentifcation(MenuBarItemIds.ResidualsVsTimeParameterIdentifcation);
         yield return ParameterIdentificationMenuBarButtons.ResidualHistogramParameterIdentification(MenuBarItemIds.ResidualHistogramParameterIdentification);
         yield return ParameterIdentificationMenuBarButtons.CorrelationMatrixParameterIdentification(MenuBarItemIds.CorrelationMatrixParameterIdentification);
         yield return ParameterIdentificationMenuBarButtons.CovarianceMatrixParameterIdentification(MenuBarItemIds.CovarianceMatrixParameterIdentification);
         yield return ParameterIdentificationMenuBarButtons.ParameterIdentificationFeedbackView(MenuBarItemIds.ParameterIdentificationFeedbackView);
         yield return ParameterIdentificationMenuBarButtons.TimeProfilePredictionInterval(MenuBarItemIds.TimeProfilePredictionInterval);
         yield return ParameterIdentificationMenuBarButtons.TimeProfileVPCInterval(MenuBarItemIds.TimeProfileVPCInterval);
         yield return ParameterIdentificationMenuBarButtons.TimeProfileConfidenceInterval(MenuBarItemIds.TimeProfileConfidenceInterval);

         yield return SensitivityAnalysisMenuBarButtons.SensitivityAnalysisPKParameterAnalysis(MenuBarItemIds.SensitivityAnalysisPKParameterAnalysis);
         yield return SensitivityAnalysisMenuBarButtons.CreateSensitivityAnalysis(MenuBarItemIds.CreateSensitivityAnalysis);
         yield return SensitivityAnalysisMenuBarButtons.SensitivityAnalysisFeedbackView(MenuBarItemIds.SensitivityAnalysisFeedbackView);
         yield return SensitivityAnalysisMenuBarButtons.RunSensitivityAnalysis(MenuBarItemIds.RunSensitivityAnalysis);
         yield return SensitivityAnalysisMenuBarButtons.StopSensitivityAnalysis(MenuBarItemIds.StopSensitivityAnalysis);
      }
   }
}