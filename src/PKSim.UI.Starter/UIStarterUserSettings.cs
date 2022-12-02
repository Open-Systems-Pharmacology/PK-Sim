using System.Collections.Generic;
using System.Drawing;
using OSPSuite.Assets;
using OSPSuite.Core.Comparison;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Diagram.Elements;
using OSPSuite.Presentation.Presenters.ParameterIdentifications;
using OSPSuite.Presentation.Presenters.SensitivityAnalyses;
using OSPSuite.Presentation.Settings;
using PKSim.CLI.Core.MinimalImplementations;

namespace PKSim.UI.Starter
{
   public class UIStarterUserSettings : CLIUserSettings, IPresentationUserSettings
   {
      public UIStarterUserSettings()
      {
         DisplayUnits = new DisplayUnitsManager();
         ComparerSettings = new ComparerSettings { CompareHiddenEntities = false };
         ProjectFiles = new List<string>();
         DiagramOptions = new DiagramOptions();
         JournalPageEditorSettings = new JournalPageEditorSettings();
         ParameterIdentificationFeedbackEditorSettings = new ParameterIdentificationFeedbackEditorSettings();
         SensitivityAnalysisFeedbackEditorSettings = new SensitivityAnalysisFeedbackEditorSettings();

         ResetToDefault();
      }

      public string DefaultChartEditorLayout { get; set; }
      public string ActiveSkin { get; set; }
      public Scalings DefaultChartYScaling { get; set; }
      public IconSize IconSizeTreeView { get; set; }
      public IconSize IconSizeTab { get; set; }
      public IconSize IconSizeContextMenu { get; set; }
      public Color ChartBackColor { get; set; }
      public Color ChartDiagramBackColor { get; set; }
      public bool ColorGroupObservedDataFromSameFolder { get; set; }
      public DisplayUnitsManager DisplayUnits { get; set; }
      public IList<string> ProjectFiles { get; set; }
      public uint MRUListItemCount { get; set; }
      public ComparerSettings ComparerSettings { get; set; }
      public IDiagramOptions DiagramOptions { get; set; }
      public string ChartEditorLayout { get; set; }
      public JournalPageEditorSettings JournalPageEditorSettings { get; set; }
      public ParameterIdentificationFeedbackEditorSettings ParameterIdentificationFeedbackEditorSettings { get; set; }
      public SensitivityAnalysisFeedbackEditorSettings SensitivityAnalysisFeedbackEditorSettings { get; set; }
   }
}