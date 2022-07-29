using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using OSPSuite.Assets;
using OSPSuite.Core.Extensions;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Services;
using OSPSuite.Utility;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;

namespace PKSim.UI.Views
{
   public partial class UserSettingsView : BaseUserControl, IUserSettingsView
   {
      private readonly IImageListRetriever _imageListRetriever;
      private readonly IToolTipCreator _toolTipCreator;
      private IUserSettingsPresenter _presenter;
      private ScreenBinder<IUserSettings> _screenBinder;

      public UserSettingsView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator)
      {
         InitializeComponent();
         _imageListRetriever = imageListRetriever;
         _toolTipCreator = toolTipCreator;
         cbDefaultSpecies.Properties.SmallImages = _imageListRetriever.AllImages16x16;
         cbDefaultSpecies.Properties.LargeImages = _imageListRetriever.AllImages32x32;
         cbDefaultPopulationAnalysis.Properties.SmallImages = _imageListRetriever.AllImages16x16;
         cbDefaultPopulationAnalysis.Properties.LargeImages = _imageListRetriever.AllImages32x32;
      }

      public void AttachPresenter(IUserSettingsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _screenBinder = new ScreenBinder<IUserSettings>();

         _screenBinder.Bind(x => x.AllowsScientificNotation)
            .To(chkAllowsScientificNotation)
            .WithCaption(PKSimConstants.UI.AllowsScientificNotation);

         _screenBinder.Bind(x => x.ShouldRestoreWorkspaceLayout)
            .To(chkShouldRestoreWorkspaceLayout)
            .WithCaption(PKSimConstants.UI.ShouldRestoreWorkspaceLayout);

         _screenBinder.Bind(x => x.ShowUpdateNotification)
            .To(chkShowUpdateNotification)
            .WithCaption(PKSimConstants.UI.ShowUpdateNotification);

         _screenBinder.Bind(x => x.ColorGroupObservedDataFromSameFolder)
            .To(chckColorGroupObservedData)
            .WithCaption(Captions.ShouldColorGroupObservedData);

         _screenBinder.Bind(x => x.ActiveSkin)
            .To(cbActiveSkin)
            .WithValues(x => _presenter.AvailableSkins);

         _screenBinder.Bind(x => x.IconSizeTreeView)
            .To(cbIconSizeTreeView)
            .WithValues(x => _presenter.AvailableIconSizes);

         _screenBinder.Bind(x => x.IconSizeTab)
            .To(cbIconSizeTab)
            .WithValues(x => _presenter.AvailableIconSizes);

         _screenBinder.Bind(x => x.IconSizeContextMenu)
            .To(cbIconSizeContextMenu)
            .WithValues(x => _presenter.AvailableIconSizes);

         _screenBinder.Bind(x => x.DefaultSpecies)
            .To(cbDefaultSpecies)
            .WithImages(species => _imageListRetriever.ImageIndex(species))
            .WithValues(x => _presenter.AllSpecies())
            .AndDisplays(x => _presenter.AllSpeciesDisplayName())
            .Changed += () => _presenter.SpeciesChanged();

         _screenBinder.Bind(x => x.DefaultPopulation)
            .To(cbDefaultPopulation)
            .WithValues(x => _presenter.AllPopulationsFor(x.DefaultSpecies))
            .AndDisplays(x => _presenter.AllPopulationsDisplayName(x.DefaultSpecies));

         _screenBinder.Bind(x => x.DefaultParameterGroupingMode)
            .To(cbDefaultParameterGroupingMode)
            .WithValues(x => _presenter.AllParameterGroupingMode())
            .AndDisplays(x => _presenter.AllParameterGroupingModeDisplay());

         _screenBinder.Bind(x => x.AbsTol).To(tbAbsTol);
         _screenBinder.Bind(x => x.RelTol).To(tbRelTol);
         _screenBinder.Bind(x => x.NumberOfBins).To(tbNumberOfBins);
         _screenBinder.Bind(x => x.NumberOfIndividualsPerBin).To(tbNumberOfIndividualsPerBin);
         _screenBinder.Bind(x => x.DecimalPlace).To(tbDecimalPlace);
         _screenBinder.Bind(x => x.MaximumNumberOfCoresToUse).To(tbNumberOfProcessors);

         _screenBinder.Bind(x => x.MRUListItemCount).To(tbMRUListItemCount);
         _screenBinder.Bind(x => x.TemplateDatabasePath).To(tbTemplateDatabase);
         _screenBinder.Bind(x => x.ChangedColor).To(colorChanged);
         _screenBinder.Bind(x => x.FormulaColor).To(colorFormula);
         _screenBinder.Bind(x => x.DisabledColor).To(colorDisabled);
         _screenBinder.Bind(x => x.ChartDiagramBackColor).To(colorChartDiagramBack);
         _screenBinder.Bind(x => x.ChartBackColor).To(colorChartBack);
         _screenBinder.Bind(x => x.DefaultFractionUnboundName).To(cbDefaultFuName);
         _screenBinder.Bind(x => x.DefaultLipophilicityName).To(cbDefaultLipoName);
         _screenBinder.Bind(x => x.DefaultSolubilityName).To(cbDefaultSolName);

         var allBehaviors = EnumHelper.AllValuesFor<LoadTemplateWithReference>().ToList();
         _screenBinder.Bind(x => x.LoadTemplateWithReference)
            .To(cbTemplateReferenceBehavior)
            .WithValues(allBehaviors)
            .AndDisplays(allBehaviors.Select(v => v.ToString().SplitToUpperCase()));

         _screenBinder.Bind(x => x.DefaultPopulationAnalysis)
            .To(cbDefaultPopulationAnalysis)
            .WithImages(populationAnalysisType => _imageListRetriever.ImageIndex(_presenter.PopulationIconNameFor(populationAnalysisType)))
            .WithValues(x => _presenter.AllPopulationAnalyses())
            .AndDisplays(_presenter.PopulationAnalysesDisplayFor);

         _screenBinder.Bind(x => x.DefaultChartYScaling)
            .To(cbPreferredChartYScaling)
            .WithValues(x => _presenter.AllScalings());

         _screenBinder.Bind(x => x.PreferredViewLayout)
            .To(cbPreferredVewLayout)
            .WithValues(x => _presenter.AllViewLayouts());

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
         _screenBinder.Changed += NotifyViewChanged;

         tbTemplateDatabase.ButtonClick += (o, e) => OnEvent(() => templateDatabaseButtonClick(o, e));
      }

      private void templateDatabaseButtonClick(object sender, ButtonPressedEventArgs e)
      {
         var editor = (ButtonEdit) sender;
         var buttonIndex = editor.Properties.Buttons.IndexOf(e.Button);
         if (buttonIndex == 0)
            _presenter.SelectTemplateDatabase();
         else
            _presenter.CreateTemplateDatabase();
      }

      public void BindTo(IUserSettings userSettings)
      {
         _screenBinder.BindToSource(userSettings);
      }

      public void RefreshAllIndividualList()
      {
         _screenBinder.RefreshListElements();
      }

      public override bool HasError => _screenBinder.HasError;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemDefaultChartYScale.Text = PKSimConstants.UI.ChartYScale.FormatForLabel();
         layoutItemActiveSkin.Text = PKSimConstants.UI.ActiveSkin.FormatForLabel();
         layoutItemDecimalPlace.Text = PKSimConstants.UI.DecimalPlace.FormatForLabel();
         layoutItemIconSizeTreeView.Text = PKSimConstants.UI.IconSizeTreeView.FormatForLabel();
         layoutItemIconSizeTab.Text = PKSimConstants.UI.IconSizeTab.FormatForLabel();
         layoutItemIconSizeContextMenu.Text = PKSimConstants.UI.IconSizeContextMenu.FormatForLabel();
         layoutItemMRUListItemCount.Text = PKSimConstants.UI.MRUListItemCount.FormatForLabel();
         layoutItemRelTol.Text = PKSimConstants.UI.RelTol.FormatForLabel();
         layoutItemAbsTol.Text = PKSimConstants.UI.AbsTol.FormatForLabel();
         layoutItemNumberOfBins.Text = PKSimConstants.UI.NumberOfBins.FormatForLabel();
         layoutItemNumberOfIndividualsPerBin.Text = PKSimConstants.UI.NumberOfIndividualsPerBin.FormatForLabel();
         layoutItemNumberOfProcessors.Text = Captions.NumberOfProcessors.FormatForLabel();
         layoutGroupNumericalProperties.Text = PKSimConstants.UI.NumericalProperties;
         layoutGroupUIProperties.Text = PKSimConstants.UI.UIProperties;
         layoutGroupIconSizes.Text = PKSimConstants.UI.IconSizes;
         layoutGroupTemplateDatabase.Text = PKSimConstants.UI.TemplateDatabase;
         layoutGroupColors.Text = PKSimConstants.UI.Colors;
         layoutItemTemplateDatabase.Text = PKSimConstants.UI.TemplateDatabasePath.FormatForLabel();
         layoutItemChartBackColor.Text = PKSimConstants.UI.ChartBackColor.FormatForLabel();
         layoutItemFormulaColor.Text = PKSimConstants.UI.FormulaColor.FormatForLabel();
         layoutItemChartDiagramBackColor.Text = PKSimConstants.UI.ChartDiagramBackColor.FormatForLabel();
         layoutItemDisabledColor.Text = PKSimConstants.UI.DisabledColor.FormatForLabel();
         layoutItemChangedColor.Text = PKSimConstants.UI.ChangedColor.FormatForLabel();
         layoutItemDefaultSpecies.Text = PKSimConstants.UI.DefaultSpecies.FormatForLabel();
         layoutItemDefaultPopulation.Text = PKSimConstants.UI.DefaultPopulation.FormatForLabel();
         layoutItemParameterGroupingMode.Text = PKSimConstants.UI.DefaultParameterGroupLayout.FormatForLabel();
         layoutItemDefaultLipoName.Text = PKSimConstants.UI.DefaultLipophilicityName.FormatForLabel();
         layoutItemDefaultFuName.Text = PKSimConstants.UI.DefaultFractionUnboundName.FormatForLabel();
         layoutItemDefaultSolName.Text = PKSimConstants.UI.DefaultSolubilityName.FormatForLabel();
         layoutItemDefaultPopulationAnalysis.Text = PKSimConstants.UI.DefaultPopulationAnalysisType.FormatForLabel();
         layoutItemPreferredViewLayout.Text = PKSimConstants.UI.PreferredViewLayout.FormatForLabel();
         layoutItemTemplateRefBehavior.Text = PKSimConstants.UI.TemplateReferenceBehavior.FormatForLabel();
         layoutGroupDefaults.Text = PKSimConstants.UI.Defaults;
         Caption = PKSimConstants.UI.UserGeneral;
         tbTemplateDatabase.Properties.Buttons[0].SuperTip = _toolTipCreator.CreateToolTip(PKSimConstants.UI.SelectTemplateDatabasePath, PKSimConstants.UI.TemplateDatabasePath, ApplicationIcons.ProjectOpen);
         tbTemplateDatabase.Properties.Buttons[1].SuperTip = _toolTipCreator.CreateToolTip(PKSimConstants.UI.CreateTemplateDatabasePath, PKSimConstants.UI.TemplateDatabasePath, ApplicationIcons.Create);
         tbTemplateDatabase.Properties.Buttons[1].ToolTip = PKSimConstants.UI.CreateTemplateDatabasePath;
         tbTemplateDatabase.Properties.Buttons[1].Kind = ButtonPredefines.Glyph;
         tbTemplateDatabase.Properties.Buttons[1].Image = ApplicationIcons.Create.ToImage(IconSizes.Size16x16);
         cbDefaultSolName.FillWith(PKSimConstants.UI.PredefinedSolubilityAlternatives());
         cbDefaultFuName.FillWith(PKSimConstants.UI.PredefinedFractionUnboundAlternatives());
         cbDefaultLipoName.FillWith(PKSimConstants.UI.PredefinedLipophilicityAlternatives());
         ApplicationIcon = ApplicationIcons.UserSettings;
      }
   }
}