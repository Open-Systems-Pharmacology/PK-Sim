using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Repositories;
using PKSim.Assets;
using PKSim.Presentation.Core;

namespace PKSim.Presentation.Repositories
{
   public class ButtonGroupRepository : OSPSuite.Presentation.Repositories.ButtonGroupRepository
   {
      public ButtonGroupRepository(IMenuBarItemRepository menuBarItemRepository) : base(menuBarItemRepository)
      {
      }

      protected override IEnumerable<IButtonGroup> AllButtonGroups()
      {
         yield return projectButtonGroup;
         yield return createButtonGroup;
         yield return compareButtonGroup;
         yield return toolsButtonGroup;
         yield return runSimulationButtonGroup;
         yield return individualSimulationAnalysesButtonGroup;
         yield return viewButtonGroup;
         yield return importButtonGroup;
         yield return importPopulationSimulationButtonGroup;
         yield return exportIndividualSimulationGroup;
         yield return exportPopulationSimulationGroup;
         yield return adminButtonGroup;
         yield return exportProjectGroup;
         yield return loadBuildingBlocksButtonGroup;
         yield return displayUnitsButtonGroup;
         yield return populationSimulationAnalysesButtonGroup;
         yield return populationSimulationWorkflowButtonGroup;
         yield return journalButtonGroup;
         yield return favoritesButtonGroup;
         yield return parameterIdentificationButtonGroup;
         yield return parameterSensitivityButtonGroup;
         yield return runParameterIdentificationButtonGroup;
         yield return runSensitivityAnalysisButtonGroup;
         yield return parameterIdentificationAnalysisButtonGroup;
         yield return parameterIdentificationConfidenceIntervalButtonGroup;
         yield return senstivityAnalysisButtonGroup;
      }

      private IButtonGroup projectButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.File)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.NewProject)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.OpenProject)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.CloseProject)))
         .WithButton(
            CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.SaveGroup))
               .WithSubItem(_menuBarItemRepository.Find(MenuBarItemIds.SaveProject))
               .WithSubItem(_menuBarItemRepository.Find(MenuBarItemIds.SaveProjectAs))
               .WithStyle(ItemStyle.Large)
               .AsGroupStarter()
         )
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ProjectDescription)))
         .WithButton(
            CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportProjectToSnapshot))
               .WithCaption(PKSimConstants.MenuNames.ExportProjectToSnapshotMenu)
         )
         .WithButton(
            CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.LoadProjectFromSnahpshot))
               .WithCaption(PKSimConstants.MenuNames.LoadProjectFromSnapshotMenu)
         )
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.SelectJournal)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.About)).AsGroupStarter())
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.Exit)).AsGroupStarter())
         .WithId(ButtonGroupIds.Project);

      private IButtonGroup importButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.Import)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ImportObservedData)).WithCaption(PKSimConstants.Ribbons.ObservedData))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ImportFractionData)).WithCaption(PKSimConstants.Ribbons.FractionData))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.NewImportIndividualSimulation)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.NewImportPopulationSimulation)))
         .WithId(ButtonGroupIds.Import);

      private IButtonGroup importPopulationSimulationButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.Import)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ImportActiveSimulationResults)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ImportActiveSimulationPKParameters)))
         .WithId(ButtonGroupIds.ImportPopulationSimulation);

      private IButtonGroup exportIndividualSimulationGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.Export)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportActiveSimulationToMoBi)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportActiveSimulationToPkml)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportActiveSimulationResultsToExcel)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportActiveSimulationResultsToCSV)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportActiveSimulationToPDF)))
         .WithId(ButtonGroupIds.ExportIndividualSimulation);

      private IButtonGroup exportPopulationSimulationGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.Export)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportActiveSimulationToMoBi)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportActiveSimulationToPkml)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportActiveSimulationPopulationToExcel)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportActiveSimulationResultsToCSV)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportActiveSimulationPKAnalysesToCSV)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportActiveSimulationForClusterComputations)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportActiveSimulationToPDF)))
         .WithId(ButtonGroupIds.ExportPopulationSimulation);

      private IButtonGroup exportProjectGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.ExportProject)
         .WithButton(
            CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.HistoryReportGroup))
               .WithSubItem(_menuBarItemRepository.Find(MenuBarItemIds.HistoryReportExcel))
               .WithSubItem(_menuBarItemRepository.Find(MenuBarItemIds.HistoryReportPDF))
               .WithStyle(ItemStyle.Large)
         )
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ProjectReport)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportProjectToSnapshot)))
         .WithId(ButtonGroupIds.ExportProject);

      private IButtonGroup createButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.Create)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.NewIndividual)).WithCaption(PKSimConstants.Ribbons.Individual))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.NewPopulation)).WithCaption(PKSimConstants.Ribbons.Population))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.NewCompound)).WithCaption(PKSimConstants.Ribbons.Compound))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.NewFormulation)).WithCaption(PKSimConstants.Ribbons.Formulation))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.NewProtocol)).WithCaption(PKSimConstants.Ribbons.Protocol))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.NewEvent)).WithCaption(PKSimConstants.Ribbons.Event))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.NewSimulation)).WithCaption(PKSimConstants.Ribbons.Simulation))
         .WithId(ButtonGroupIds.Create);

      private IButtonGroup compareButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.CompareResults)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.IndividualSimulationComparison)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.PopulationSimulationComparison)))
         .WithId(ButtonGroupIds.Compare);

      private IButtonGroup displayUnitsButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.DisplayUnits)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ManageUserDisplayUnits)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ManageProjectDisplayUnits)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.UpdateAllToDisplayUnits)))
         .WithId(ButtonGroupIds.DisplayUnits);

      private IButtonGroup parameterIdentificationButtonGroup => CreateButtonGroup.WithCaption(Ribbons.ParameterIdentification)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.CreateParameterIdentification)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ParameterIdentificationFeedbackView)))
         .WithId(ButtonGroupIds.ParameterIdentification);

      private IButtonGroup parameterSensitivityButtonGroup => CreateButtonGroup.WithCaption(Ribbons.SensitivityAnalysis)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.CreateSensitivityAnalysis)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.SensitivityAnalysisFeedbackView)))
         .WithId(ButtonGroupIds.SensitivityAnalysis);

      private IButtonGroup loadBuildingBlocksButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.LoadBuildingBlocks)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.LoadIndividual))
            .WithCaption(PKSimConstants.ObjectTypes.Individual)
            .WithStyle(ItemStyle.Small)
            .WithIcon(ApplicationIcons.Individual))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.LoadPopulation))
            .WithCaption(PKSimConstants.ObjectTypes.Population)
            .WithStyle(ItemStyle.Small)
            .WithIcon(ApplicationIcons.Population))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.LoadCompound))
            .WithCaption(PKSimConstants.ObjectTypes.Compound)
            .WithStyle(ItemStyle.Small)
            .WithIcon(ApplicationIcons.Compound))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.LoadFormulationFromTemplate))
            .WithCaption(PKSimConstants.ObjectTypes.Formulation)
            .WithStyle(ItemStyle.Small)
            .WithIcon(ApplicationIcons.Formulation))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.LoadProtocol))
            .WithCaption(PKSimConstants.ObjectTypes.AdministrationProtocol)
            .WithStyle(ItemStyle.Small)
            .WithIcon(ApplicationIcons.Protocol))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.LoadEvent))
            .WithCaption(PKSimConstants.ObjectTypes.Event)
            .WithStyle(ItemStyle.Small)
            .WithIcon(ApplicationIcons.Event))
         .WithId(ButtonGroupIds.LoadBuildingBlocks);

      private IButtonGroup toolsButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.Tools)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.Options)))
         .WithId(ButtonGroupIds.Tools);

      private IButtonGroup adminButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.Admin)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.GarbageCollection)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.GenerateCalculationMethods)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.GeneratePKMLTemplates)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.GenerateGroupsTemplate)))
         .WithId(ButtonGroupIds.Admin);

      private IButtonGroup viewButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.PanelView)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.BuildingBlockExplorerView)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.SimulationExplorerView)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.HistoryView)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ComparisonView)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.JournalView)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.JournalDiagramView)))
         .WithId(ButtonGroupIds.View);

      private IButtonGroup runSimulationButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.Simulation)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.CloneActiveSimulation)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ConfigureActiveSimulation)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.Run)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.RunWithSettings)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.Stop)))
         .WithId(ButtonGroupIds.RunSimulation);

      private IButtonGroup individualSimulationAnalysesButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.Analyses)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ShowIndividualResults)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.IndividualSimulationComparisonInAnalyze)))
         .WithId(ButtonGroupIds.IndividualSimulationAnalyses);

      private IButtonGroup populationSimulationAnalysesButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.Analyses)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.TimeProfileAnalysis)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.BoxWhiskerAnalysis)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ScatterAnalysis)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.RangeAnalysis)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.PopulationSimulationComparisonInAnalyze)))
         .WithId(ButtonGroupIds.PopulationSimulationAnalyses);

      private IButtonGroup populationSimulationWorkflowButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.Workflow)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.LoadPopulationSimulationWorkflow)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.SavePopulationSimulationWorkflow)))
         .WithId(ButtonGroupIds.PopulationSimulationWorkflow);

      private IButtonGroup journalButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.WorkingJournal)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.CreateJournalPage)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.JournalEditorView)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.SearchJournal)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ExportJournal)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.RefreshJournal)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.SelectJournal)))
         .WithId(ButtonGroupIds.Journal);

      private IButtonGroup favoritesButtonGroup => CreateButtonGroup.WithCaption(PKSimConstants.Ribbons.Favorites)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.LoadFavorites)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.SaveFavorites)))
         .WithId(ButtonGroupIds.Favorites);

      private IButtonGroup runParameterIdentificationButtonGroup => CreateButtonGroup.WithCaption(Ribbons.ParameterIdentification)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.RunParameterIdentification)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.StopParameterIdentification)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ParameterIdentificationFeedbackView)))
         .WithId(ButtonGroupIds.RunParameterIdentification);

      private IButtonGroup runSensitivityAnalysisButtonGroup => CreateButtonGroup.WithCaption(Ribbons.SensitivityAnalysis)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.RunSensitivityAnalysis)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.StopSensitivityAnalysis)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.SensitivityAnalysisFeedbackView)))
         .WithId(ButtonGroupIds.RunSensitivityAnalysis);

      private IButtonGroup senstivityAnalysisButtonGroup => CreateButtonGroup.WithCaption(Ribbons.ParameterSensitivityAnalyses)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.SensitivityAnalysisPKParameterAnalysis)))
         .WithId(ButtonGroupIds.SensitivityAnalysisPKParameterAnalyses);

      private IButtonGroup parameterIdentificationAnalysisButtonGroup => CreateButtonGroup.WithCaption(Ribbons.ParameterIdentificationAnalyses)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.TimeProfileParameterIdentification)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.PredictedVsObservedParameterIdentification)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ResidualsVsTimeParameterIdentifcation)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.ResidualHistogramParameterIdentification)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.CovarianceMatrixParameterIdentification)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.CorrelationMatrixParameterIdentification)))
         .WithId(ButtonGroupIds.ParameterIdentificationAnalyses);

      private IButtonGroup parameterIdentificationConfidenceIntervalButtonGroup => CreateButtonGroup.WithCaption(Ribbons.ParameterIdentificationConfidenceInterval)
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.TimeProfileConfidenceInterval)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.TimeProfileVPCInterval)))
         .WithButton(CreateRibbonButton.From(_menuBarItemRepository.Find(MenuBarItemIds.TimeProfilePredictionInterval)))
         .WithId(ButtonGroupIds.ParameterIdentificationConfidenceInterval);
   }
}