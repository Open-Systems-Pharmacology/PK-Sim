using System.Collections.Generic;
using System.Drawing;
using OSPSuite.Assets;
using OSPSuite.Core.Comparison;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters.ParameterIdentifications;
using OSPSuite.Presentation.Presenters.SensitivityAnalyses;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Settings;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIUserSettings : Notifier, IUserSettings
   {
      public IBusinessRuleSet Rules { get; private set; }
      public string DefaultChartEditorLayout { get; set; }
      public string ActiveSkin { get; set; }
      public Scalings DefaultChartYScaling { get; set; }
      public int NumberOfBins { get; set; }
      public int NumberOfIndividualsPerBin { get; set; }
      public string DefaultSpecies { get; set; }
      public string DefaultPopulation { get; set; }
      public double AbsTol { get; set; }
      public double RelTol { get; set; }
      public string DefaultLipophilicityName { get; set; }
      public string DefaultFractionUnboundName { get; set; }
      public string DefaultSolubilityName { get; set; }
      public OutputSelections OutputSelections { get; set; }
      public int MaximumNumberOfCoresToUse { get; set; }
      public PopulationAnalysisType DefaultPopulationAnalysis { get; set; }
      public string TemplateDatabasePath { get; set; }
      public IconSize IconSizeTreeView { get; set; }
      public IconSize IconSizeTab { get; set; }
      public IconSize IconSizeContextMenu { get; set; }
      public uint DecimalPlace { get; set; }
      public bool AllowsScientifcNotation { get; set; }
      public uint MRUListItemCount { get; set; }
      public ComparerSettings ComparerSettings { get; set; }
      public string MainViewLayout { get; set; }
      public string RibbonLayout { get; set; }
      public int LayoutVersion { get; set; }
      public Color ChangedColor { get; set; }
      public Color FormulaColor { get; set; }
      public Color ChartBackColor { get; set; }
      public Color ChartDiagramBackColor { get; set; }
      public DisplayUnitsManager DisplayUnits { get; set; }
      public Color DisabledColor { get; set; }
      public bool ShouldRestoreWorkspaceLayout { get; set; }
      public ParameterGroupingModeId DefaultParameterGroupingMode { get; set; }
      public DirectoryMapSettings DirectoryMapSettings { get; set; }
      public IEnumerable<DirectoryMap> UsedDirectories { get; set; }
      public bool ShowUpdateNotification { get; set; }
      public string LastIgnoredVersion { get; set; }
      public ViewLayout PreferredViewLayout { get; set; }
      public IList<string> ProjectFiles { get; set; }

      public IDiagramOptions DiagramOptions { get; set; }

      public string ChartEditorLayout { get; set; }

      public JournalPageEditorSettings JournalPageEditorSettings { get; set; }

      public void ResetToDefault()
      {
         AbsTol = CoreConstants.DEFAULT_ABS_TOL;
         RelTol = CoreConstants.DEFAULT_REL_TOL;
         NumberOfBins = CoreConstants.DEFAULT_NUMBER_OF_BINS;
         DefaultLipophilicityName = PKSimConstants.UI.DefaultAlternative;
         DefaultFractionUnboundName = PKSimConstants.UI.DefaultAlternative;
         DefaultSolubilityName = PKSimConstants.UI.DefaultAlternative;
         JournalPageEditorSettings = new JournalPageEditorSettings();
         ParameterIdentificationFeedbackEditorSettings = new ParameterIdentificationFeedbackEditorSettings();
         SensitivityAnalysisFeedbackEditorSettings = new SensitivityAnalysisFeedbackEditorSettings();
      }

      public void ResetLayout()
      {
      }

      public void RestoreLayout()
      {
      }

      public void SaveLayout()
      {
      }

      public CLIUserSettings()
      {
         DisplayUnits = new DisplayUnitsManager();
         ProjectFiles = new List<string>();
         ResetToDefault();
      }

      public ParameterIdentificationFeedbackEditorSettings ParameterIdentificationFeedbackEditorSettings { get; set; }
      public SensitivityAnalysisFeedbackEditorSettings SensitivityAnalysisFeedbackEditorSettings { get; set; }
   }
}