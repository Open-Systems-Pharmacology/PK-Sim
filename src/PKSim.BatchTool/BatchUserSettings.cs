using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Comparison;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters.ParameterIdentifications;
using OSPSuite.Presentation.Presenters.SensitivityAnalyses;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Settings;
using PKSim.CLI.Core.MinimalImplementations;
using PKSim.Presentation;

namespace PKSim.BatchTool
{
   public class BatchUserSettings : CLIUserSettings, IUserSettings
   {
      public IList<string> ProjectFiles { get; set; } = new List<string>();
      public string DefaultChartEditorLayout { get; set; }
      public string ActiveSkin { get; set; }
      public Scalings DefaultChartYScaling { get; set; }
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
      public DisplayUnitsManager DisplayUnits { get; set; } = new DisplayUnitsManager();
      public Color DisabledColor { get; set; }
      public ParameterGroupingModeId DefaultParameterGroupingMode { get; set; }
      public string LastIgnoredVersion { get; set; }

      public IDiagramOptions DiagramOptions { get; set; }

      public string ChartEditorLayout { get; set; }

      public DirectoryMapSettings DirectoryMapSettings { get; } = new DirectoryMapSettings();
      public IEnumerable<DirectoryMap> UsedDirectories { get; } = Enumerable.Empty<DirectoryMap>();
      public bool ShowUpdateNotification { get; set; }
      public bool ShouldRestoreWorkspaceLayout { get; set; }

      public JournalPageEditorSettings JournalPageEditorSettings { get; set; } = new JournalPageEditorSettings();
      public ParameterIdentificationFeedbackEditorSettings ParameterIdentificationFeedbackEditorSettings { get; set; } = new ParameterIdentificationFeedbackEditorSettings();
      public SensitivityAnalysisFeedbackEditorSettings SensitivityAnalysisFeedbackEditorSettings { get; set; } = new SensitivityAnalysisFeedbackEditorSettings();

      public void RestoreLayout()
      {
         /*nothing to do*/
      }

      public void SaveLayout()
      {
         /*nothing to do*/
      }

      public void ResetLayout()
      {
         /*nothing to do*/
      }

      public ViewLayout PreferredViewLayout { get; set; } = ViewLayouts.TabbedView;
   }
}