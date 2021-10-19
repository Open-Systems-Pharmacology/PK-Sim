using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Format;
using OSPSuite.Utility.Validation;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Ribbon;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation;
using OSPSuite.Core.Comparison;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Diagram.Elements;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters.ParameterIdentifications;
using OSPSuite.Presentation.Presenters.SensitivityAnalyses;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Settings;
using OSPSuite.UI;

namespace PKSim.UI
{
   public class UserSettings : ValidatableDTO, IUserSettings
   {
      private readonly DockManager _dockManager;
      private readonly RibbonBarManager _ribbonManager;
      private readonly INumericFormatterOptions _numericFormatterOptions;
      private readonly ISkinManager _skinManager;
      public Scalings DefaultChartYScaling { get; set; }
      private readonly DirectoryMapSettings _directoryMapSettings;
      public bool ShouldRestoreWorkspaceLayout { get; set; }
      public ParameterGroupingModeId DefaultParameterGroupingMode { get; set; }
      public string MainViewLayout { get; set; }
      public string RibbonLayout { get; set; }
      public string DefaultChartEditorLayout { get; set; }
      public bool ShowUpdateNotification { get; set; }
      public int MaximumNumberOfCoresToUse { get; set; }
      public PopulationAnalysisType DefaultPopulationAnalysis { get; set; }
      public ViewLayout PreferredViewLayout { get; set; }
      public int NumberOfIndividualsPerBin { get; set; }
      public string DefaultSpecies { get; set; }
      public string DefaultPopulation { get; set; }
      public uint MRUListItemCount { get; set; }
      public ComparerSettings ComparerSettings { get; set; }
      public int LayoutVersion { get; set; }
      public int ChartSettingsVersion { get; set; }
      private string _activeSkin;
      public int NumberOfBins { get; set; }
      public double AbsTol { get; set; }
      public double RelTol { get; set; }
      public string DefaultLipophilicityName { get; set; }
      public string DefaultFractionUnboundName { get; set; }
      public string DefaultSolubilityName { get; set; }
      public IList<string> ProjectFiles { get; set; }
      public IDiagramOptions DiagramOptions { get; set; }
      public string ChartEditorLayout { get; set; }
      public JournalPageEditorSettings JournalPageEditorSettings { get; set; }
      public string LastIgnoredVersion { get; set; }
      public OutputSelections OutputSelections { get; set; }
      public DisplayUnitsManager DisplayUnits { get; set; }
      public ParameterIdentificationFeedbackEditorSettings ParameterIdentificationFeedbackEditorSettings { get; set; }
      public SensitivityAnalysisFeedbackEditorSettings SensitivityAnalysisFeedbackEditorSettings { get; set; }

      private string _templateDatabasePath;
      private bool _layoutWasExplicitlyReset;

      public UserSettings(DockManager dockManager, RibbonBarManager ribbonManager, INumericFormatterOptions numericFormatterOptions,
         ISkinManager skinManager, IPKSimConfiguration configuration, DirectoryMapSettings directoryMapSettings)
      {
         _dockManager = dockManager;
         _ribbonManager = ribbonManager;
         _numericFormatterOptions = numericFormatterOptions;
         _skinManager = skinManager;
         _directoryMapSettings = directoryMapSettings;

         DisplayUnits = new DisplayUnitsManager();
         ComparerSettings = new ComparerSettings { CompareHiddenEntities = false };
         ProjectFiles = new List<string>();
         Rules.AddRange(AllRules.All());
         DiagramOptions = new DiagramOptions();
         TemplateDatabasePath = configuration.DefaultTemplateUserDatabasePath;
         JournalPageEditorSettings = new JournalPageEditorSettings();
         ParameterIdentificationFeedbackEditorSettings = new ParameterIdentificationFeedbackEditorSettings();
         SensitivityAnalysisFeedbackEditorSettings = new SensitivityAnalysisFeedbackEditorSettings();
         ResetToDefault();
         _layoutWasExplicitlyReset = false;
      }

      public void ResetToDefault()
      {
         DecimalPlace = CoreConstants.DEFAULT_DECIMAL_PLACE;
         AllowsScientifcNotation = true;
         ShouldRestoreWorkspaceLayout = false;
         MRUListItemCount = CoreConstants.DEFAULT_MRU_LIST_ITEM_COUNT;
         ActiveSkin = CoreConstants.DEFAULT_SKIN;
         NumberOfBins = CoreConstants.DEFAULT_NUMBER_OF_BINS;
         NumberOfIndividualsPerBin = CoreConstants.DEFAULT_NUMBER_OF_INDIVIDUALS_PER_BIN;
         DefaultSpecies = CoreConstants.Species.HUMAN;
         DefaultPopulation = CoreConstants.Population.ICRP;
         DefaultParameterGroupingMode = ParameterGroupingModeId.Simple;
         LayoutVersion = CoreConstants.LAYOUT_VERSION;
         AbsTol = CoreConstants.DEFAULT_ABS_TOL;
         RelTol = CoreConstants.DEFAULT_REL_TOL;
         DefaultLipophilicityName = PKSimConstants.UI.DefaultAlternative;
         DefaultFractionUnboundName = PKSimConstants.UI.DefaultAlternative;
         DefaultSolubilityName = PKSimConstants.UI.DefaultAlternative;
         ShowUpdateNotification = true;
         MaximumNumberOfCoresToUse = Math.Max(Environment.ProcessorCount - 1, 1);
         DefaultPopulationAnalysis = PopulationAnalysisType.TimeProfile;
         PreferredViewLayout = ViewLayouts.AccordionView;
         DefaultChartYScaling = Scalings.Log;
         DefaultChartEditorLayout = Constants.DEFAULT_CHART_LAYOUT;
      }

      public DirectoryMapSettings DirectoryMapSettings => _directoryMapSettings;

      public IEnumerable<DirectoryMap> UsedDirectories => _directoryMapSettings.UsedDirectories;

      public void RestoreLayout()
      {
         if (LayoutVersion != CoreConstants.LAYOUT_VERSION)
            resetLayout();

         if (!string.IsNullOrEmpty(MainViewLayout))
            _dockManager.RestoreLayoutFromStream(streamFromString(MainViewLayout));

         if (!string.IsNullOrEmpty(RibbonLayout))
            _ribbonManager.Ribbon.Toolbar.RestoreLayoutFromStream(streamFromString(RibbonLayout));
      }

      public void ResetLayout()
      {
         resetLayout();
         _layoutWasExplicitlyReset = true;
      }

      private void resetLayout()
      {
         MainViewLayout = string.Empty;
         RibbonLayout = string.Empty;
         LayoutVersion = CoreConstants.LAYOUT_VERSION;
      }

      private MemoryStream streamFromString(string stringToConvert)
      {
         return new MemoryStream(stringToConvert.ToByteArray());
      }

      private string streamToString(MemoryStream streamToConvert)
      {
         return streamToConvert.ToArray().ToByteString();
      }

      public void SaveLayout()
      {
         LayoutVersion = CoreConstants.LAYOUT_VERSION;
         if (_layoutWasExplicitlyReset)
            return;

         var streamMainView = new MemoryStream();
         _dockManager.SaveLayoutToStream(streamMainView);
         MainViewLayout = streamToString(streamMainView);

         var streamRibbon = new MemoryStream();
         _ribbonManager.Ribbon.Toolbar.SaveLayoutToStream(streamRibbon);
         RibbonLayout = streamToString(streamRibbon);
      }

      public uint DecimalPlace
      {
         get => _numericFormatterOptions.DecimalPlace;
         set => _numericFormatterOptions.DecimalPlace = value;
      }

      public bool AllowsScientifcNotation
      {
         get => _numericFormatterOptions.AllowsScientificNotation;
         set => _numericFormatterOptions.AllowsScientificNotation = value;
      }

      public string ActiveSkin
      {
         get => _activeSkin;
         set
         {
            if (_activeSkin == value)
               return;

            _activeSkin = value;
            _skinManager.ActivateSkin(this, _activeSkin);
         }
      }

      public string TemplateDatabasePath
      {
         get => _templateDatabasePath;
         set
         {
            _templateDatabasePath = value;
            OnPropertyChanged(() => TemplateDatabasePath);
         }
      }

      public IconSize IconSizeTab
      {
         get => UIConstants.ICON_SIZE_TAB;
         set
         {
            UIConstants.ICON_SIZE_TAB = value;
            OnPropertyChanged(() => IconSizeTab);
         }
      }

      public IconSize IconSizeTreeView
      {
         get => UIConstants.ICON_SIZE_TREE_VIEW;
         set
         {
            UIConstants.ICON_SIZE_TREE_VIEW = value;
            OnPropertyChanged(() => IconSizeTreeView);
         }
      }

      public IconSize IconSizeContextMenu
      {
         get => UIConstants.ICON_SIZE_CONTEXT_MENU;
         set
         {
            UIConstants.ICON_SIZE_CONTEXT_MENU = value;
            OnPropertyChanged(() => IconSizeContextMenu);
         }
      }

      public Color ChangedColor
      {
         get => PKSimColors.Changed;
         set
         {
            PKSimColors.Changed = value;
            OnPropertyChanged(() => ChangedColor);
         }
      }

      public Color FormulaColor
      {
         get => PKSimColors.Formula;
         set
         {
            PKSimColors.Formula = value;
            OnPropertyChanged(() => FormulaColor);
         }
      }

      public Color ChartBackColor
      {
         get => PKSimColors.ChartBack;
         set
         {
            PKSimColors.ChartBack = value;
            OnPropertyChanged(() => ChartBackColor);
         }
      }

      public Color ChartDiagramBackColor
      {
         get => PKSimColors.ChartDiagramBack;
         set
         {
            PKSimColors.ChartDiagramBack = value;
            OnPropertyChanged(() => ChartDiagramBackColor);
         }
      }

      public Color DisabledColor
      {
         get => PKSimColors.Disabled;
         set
         {
            PKSimColors.Disabled = value;
            OnPropertyChanged(() => DisabledColor);
         }
      }

      private static class AllRules
      {
         private static IBusinessRule nonEmpty(Expression<Func<ICoreUserSettings, string>> expression)
         {
            return GenericRules.NonEmptyRule(expression);
         }

         private static IBusinessRule numberOfCoreSmallerThanNumberOfProcessor
         {
            get
            {
               return CreateRule.For<ICoreUserSettings>()
                  .Property(x => x.MaximumNumberOfCoresToUse)
                  .WithRule((x, numCore) => numCore > 0 && numCore <= Environment.ProcessorCount)
                  .WithError(OSPSuite.Assets.Error.NumberOfCoreToUseShouldBeInferiorAsTheNumberOfProcessor(Environment.ProcessorCount));
            }
         }

         public static IEnumerable<IBusinessRule> All()
         {
            return new[]
            {
               nonEmpty(x => x.DefaultFractionUnboundName),
               nonEmpty(x => x.DefaultSolubilityName),
               nonEmpty(x => x.DefaultLipophilicityName),
               numberOfCoreSmallerThanNumberOfProcessor
            };
         }
      }

   }
}