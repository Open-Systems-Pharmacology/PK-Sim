using OSPSuite.Utility.Collections;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Core
{
   public static class MenuBarItemIds
   {
      private static readonly Cache<string, MenuBarItemId> _allMenuBarItemIds = new Cache<string, MenuBarItemId>(x => x.Name);

      public static MenuBarItemId NewProject = createMenuBarItemId("NewProject");
      public static MenuBarItemId OpenProject = createMenuBarItemId("OpenProject");
      public static MenuBarItemId ProjectDescription = createMenuBarItemId("ProjectDescription");
      public static MenuBarItemId SaveGroup = createMenuBarItemId("SaveGroup");
      public static MenuBarItemId SaveProject = createMenuBarItemId("SaveProject");
      public static MenuBarItemId SaveProjectAs = createMenuBarItemId("SaveProjectAs");
      public static MenuBarItemId ExportProjectToSnapshot = createMenuBarItemId("ExportProjectAsSnapshot");
      public static MenuBarItemId LoadProjectFromSnahpshot = createMenuBarItemId("LoadProjectFromSnahpshot");
      public static MenuBarItemId CloseProject = createMenuBarItemId("CloseProject");
      public static MenuBarItemId NewIndividual = createMenuBarItemId("NewIndividual");
      public static MenuBarItemId LoadIndividual = createMenuBarItemId("LoadIndividual");
      public static MenuBarItemId NewPopulation = createMenuBarItemId("NewPopulation");
      public static MenuBarItemId ImportPopulation = createMenuBarItemId("ImportPopulation");
      public static MenuBarItemId LoadPopulation = createMenuBarItemId("LoadPopulation");
      public static MenuBarItemId About = createMenuBarItemId("About");
      public static MenuBarItemId HistoryView = createMenuBarItemId("HistoryView");
      public static MenuBarItemId ComparisonView = createMenuBarItemId("ComparisonView");
      public static MenuBarItemId BuildingBlockExplorerView = createMenuBarItemId("BuildingBlockExplorerView");
      public static MenuBarItemId SimulationExplorerView = createMenuBarItemId("SimulationExplorerView");
      public static MenuBarItemId NewCompound = createMenuBarItemId("NewCompound");
      public static MenuBarItemId LoadCompound = createMenuBarItemId("LoadCompound");
      public static MenuBarItemId Options = createMenuBarItemId("Options");
      public static MenuBarItemId GarbageCollection = createMenuBarItemId("GarbageCollection");
      public static MenuBarItemId Exit = createMenuBarItemId("Exit");
      public static MenuBarItemId NewSimulation = createMenuBarItemId("NewSimulation");
      public static MenuBarItemId NewImportIndividualSimulation = createMenuBarItemId("NewImportIndividualSimulation");
      public static MenuBarItemId NewImportPopulationSimulation = createMenuBarItemId("NewImportPopulationSimulation");
      public static MenuBarItemId Run = createMenuBarItemId("Run");
      public static MenuBarItemId RunWithSettings = createMenuBarItemId("RunWithSettings");
      public static MenuBarItemId Stop = createMenuBarItemId("Stop");
      public static MenuBarItemId ShowIndividualResults = createMenuBarItemId("ShowIndividualResults");
      public static MenuBarItemId BoxWhiskerAnalysis = createMenuBarItemId("BoxWhiskerAnalysis");
      public static MenuBarItemId ScatterAnalysis = createMenuBarItemId("ScatterAnalysis");
      public static MenuBarItemId RangeAnalysis = createMenuBarItemId("RangeAnalysis");
      public static MenuBarItemId TimeProfileAnalysis = createMenuBarItemId("TimeProfileAnalysis");
      public static MenuBarItemId NewProtocol = createMenuBarItemId("NewProtocol");
      public static MenuBarItemId LoadProtocol = createMenuBarItemId("LoadProtocol");
      public static MenuBarItemId NewFormulation = createMenuBarItemId("NewFormulation");
      public static MenuBarItemId LoadFormulationFromTemplate = createMenuBarItemId("LoadFormulationFromTemplate");
      public static MenuBarItemId ImportObservedData = createMenuBarItemId("ImportObservedData");
      public static MenuBarItemId ImportFractionData = createMenuBarItemId("ImportFractionData");
      public static MenuBarItemId ProjectReport = createMenuBarItemId("ProjectReport");
      public static MenuBarItemId Help = createMenuBarItemId("Help");
      public static MenuBarItemId NewEvent = createMenuBarItemId("NewEvent");
      public static MenuBarItemId LoadEvent = createMenuBarItemId("LoadEvent");
      public static MenuBarItemId GenerateCalculationMethods = createMenuBarItemId("GenerateCalculationMethods");
      public static MenuBarItemId IndividualSimulationComparison = createMenuBarItemId("IndividualSimulationComparison");
      public static MenuBarItemId IndividualSimulationComparisonInAnalyze = createMenuBarItemId("IndividualSimulationComparisonInAnalyze");
      public static MenuBarItemId PopulationSimulationComparison = createMenuBarItemId("PopulationSimulationComparison");
      public static MenuBarItemId PopulationSimulationComparisonInAnalyze = createMenuBarItemId("PopulationSimulationComparisonInAnalyze");
      public static MenuBarItemId GeneratePKMLTemplates = createMenuBarItemId("GeneratePKMLTemplates");
      public static MenuBarItemId HistoryReportGroup = createMenuBarItemId("HistoryReportGroup");
      public static MenuBarItemId HistoryReportExcel = createMenuBarItemId("HistoryReportExcel");
      public static MenuBarItemId HistoryReportPDF = createMenuBarItemId("HistoryReportPDF");
      public static MenuBarItemId ImportActiveSimulationResults = createMenuBarItemId("ImportActiveSimulationResults");
      public static MenuBarItemId ImportActiveSimulationPKParameters = createMenuBarItemId("ImportActiveSimulationPKParameters");
      public static MenuBarItemId ExportActiveSimulationToMoBi = createMenuBarItemId("ExportActiveSimulationToMoBi");
      public static MenuBarItemId ExportActiveSimulationToPkml = createMenuBarItemId("ExportActiveSimulationToPkml");
      public static MenuBarItemId ExportActiveSimulationResultsToCSV = createMenuBarItemId("ExportActiveSimulationResultsToCSV");
      public static MenuBarItemId ExportActiveSimulationResultsToExcel = createMenuBarItemId("ExportActiveSimulationResultsToExcel");
      public static MenuBarItemId ExportActiveSimulationPKAnalysesToCSV = createMenuBarItemId("ExportActiveSimulationPKAnalysesToCSV");
      public static MenuBarItemId ExportActiveSimulationPopulationToExcel = createMenuBarItemId("ExportActiveSimulationPopulationToExcel");
      public static MenuBarItemId ExportActiveSimulationForClusterComputations = createMenuBarItemId("ExportActiveSimulationForClusterComputations");
      public static MenuBarItemId ExportActiveSimulationToPDF = createMenuBarItemId("ExportActiveSimulationToPDF");
      public static MenuBarItemId CloneActiveSimulation = createMenuBarItemId("CloneActiveSimulation");
      public static MenuBarItemId ConfigureActiveSimulation = createMenuBarItemId("ConfigureActiveSimulation");
      public static MenuBarItemId GenerateGroupsTemplate = createMenuBarItemId("GenerateGroupsTemplate");
      public static MenuBarItemId ManageProjectDisplayUnits = createMenuBarItemId("ManageProjectDisplayUnits");
      public static MenuBarItemId ManageUserDisplayUnits = createMenuBarItemId("ManageUserDisplayUnits");
      public static MenuBarItemId UpdateAllToDisplayUnits = createMenuBarItemId("UpdateAllToDisplayUnits");
      public static MenuBarItemId LoadPopulationSimulationWorkflow = createMenuBarItemId("LoadPopulationSimulationWorkflow");
      public static MenuBarItemId SavePopulationSimulationWorkflow = createMenuBarItemId("SavePopulationSimulationWorkflow");
      public static MenuBarItemId CreateJournalPage = createMenuBarItemId("CreateJournalPage");
      public static MenuBarItemId JournalEditorView = createMenuBarItemId("JournalEditorView");
      public static MenuBarItemId JournalView = createMenuBarItemId("JournalView");
      public static MenuBarItemId SelectJournal = createMenuBarItemId("SelectJournal");
      public static MenuBarItemId JournalDiagramView = createMenuBarItemId("JournalDiagramView");
      public static MenuBarItemId SearchJournal = createMenuBarItemId("SearchJournal");
      public static MenuBarItemId LoadFavorites = createMenuBarItemId("LoadFavorites");
      public static MenuBarItemId SaveFavorites = createMenuBarItemId("SaveFavorites");
      public static MenuBarItemId ExportJournal = createMenuBarItemId("ExportJournal");
      public static MenuBarItemId RefreshJournal = createMenuBarItemId("RefreshJournal");
      public static MenuBarItemId CreateParameterIdentification = createMenuBarItemId("StartPararmeterIdentification");
      public static MenuBarItemId RunParameterIdentification = createMenuBarItemId("RunParameterIdentification");
      public static MenuBarItemId StopParameterIdentification = createMenuBarItemId("StopParameterIdentification");
      public static MenuBarItemId TimeProfileParameterIdentification = createMenuBarItemId("TimeProfileParameterIdentification");
      public static MenuBarItemId PredictedVsObservedParameterIdentification = createMenuBarItemId("PredictedVsObservedParameterIdentification");
      public static MenuBarItemId ResidualsVsTimeParameterIdentifcation = createMenuBarItemId("ResidualsVsTimeParameterIdentifcation");
      public static MenuBarItemId ResidualHistogramParameterIdentification = createMenuBarItemId("ResidualHistogramParameterIdentification");
      public static MenuBarItemId ParameterIdentificationFeedbackView = createMenuBarItemId("ParameterIdentificationFeedbackView");
      public static MenuBarItemId CorrelationMatrixParameterIdentification = createMenuBarItemId("CorrelationMatrixParameterIdentification");
      public static MenuBarItemId CovarianceMatrixParameterIdentification = createMenuBarItemId("CovarianceMatrixParameterIdentification");
      public static MenuBarItemId TimeProfileConfidenceInterval = createMenuBarItemId("TimeProfileConfidenceInterval");
      public static MenuBarItemId TimeProfilePredictionInterval = createMenuBarItemId("TimeProfilePredictionInterval");
      public static MenuBarItemId TimeProfileVPCInterval = createMenuBarItemId("TimeProfileVPCInterval");
      public static MenuBarItemId CreateSensitivityAnalysis = createMenuBarItemId("CreateSensitivityAnalysis");
      public static MenuBarItemId RunSensitivityAnalysis = createMenuBarItemId("RunSensitivityAnalysis");
      public static MenuBarItemId StopSensitivityAnalysis = createMenuBarItemId("StopSensitivityAnalysis");
      public static MenuBarItemId SensitivityAnalysisFeedbackView = createMenuBarItemId("SensitivityAnalysisFeedbackView");
      public static MenuBarItemId SensitivityAnalysisPKParameterAnalysis = createMenuBarItemId("SensitivityAnalysisPKParameterAnalysis");

      private static MenuBarItemId createMenuBarItemId(string name)
      {
         var menuBarItemId = new MenuBarItemId(name, _allMenuBarItemIds.Count);
         _allMenuBarItemIds.Add(menuBarItemId);
         return menuBarItemId;
      }
   }
}