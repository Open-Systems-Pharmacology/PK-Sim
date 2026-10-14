using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.UI.Extensions;
using DevExpress.Utils;
using PKSim.Assets;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.UI.Controls;
using OSPSuite.Presentation.Extensions;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class NumberOfBinsGroupingView : BaseUserControl, INumberOfBinsGroupingView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private readonly ScreenBinder<BinSizeGroupingDTO> _screenBinder;
      private INumberOfBinsGroupingPresenter _presenter;
      private readonly GridViewBinder<GroupingItemDTO> _gridViewBinder;

      public NumberOfBinsGroupingView(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
         _screenBinder = new ScreenBinder<BinSizeGroupingDTO>();
         _gridViewBinder = new GridViewBinder<GroupingItemDTO>(gridView);
         gridView.AllowsFiltering = false;
         gridView.ShowRowIndicator = false;

         var toolTipController = new ToolTipController();
         toolTipController.Initialize(imageListRetriever);
         tbNamingPattern.ToolTipController = toolTipController;
         toolTipController.GetActiveObjectInfo += (o, e) => OnEvent(getObjectToolTip, e);
      }

      public void AttachPresenter(INumberOfBinsGroupingPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.NumberOfBins).To(tbNumberOfBins);
         _screenBinder.Bind(x => x.NamingPattern).To(tbNamingPattern);
         _screenBinder.Bind(x => x.StartColor).To(ceStartColor);
         _screenBinder.Bind(x => x.EndColor).To(ceEndColor);
         _screenBinder.Bind(x => x.Symbol).To(cbSymbol)
            .WithValues(x => _presenter.AvailableSymbols);

         _screenBinder.Bind(x => x.Strategy)
            .To(cbGenerationStrategy)
            .WithValues(x => _presenter.AvailableStrategies);

         RegisterValidationFor(_screenBinder, statusChangingNotify: NotifyViewChanged, statusChangedNotify: _presenter.GenerateLabels);

         //use autobind to get automatic notification from errors
         _gridViewBinder.AutoBind(x => x.Label).AsReadOnly();
         _gridViewBinder.AutoBind(x => x.Color);

         _gridViewBinder.Changed += NotifyViewChanged;
      }

      public override bool HasError
      {
         get { return _screenBinder.HasError || _gridViewBinder.HasError; }
      }

      public void BindTo(BinSizeGroupingDTO binSizeGroupingDTO)
      {
         _screenBinder.BindToSource(binSizeGroupingDTO);
         _gridViewBinder.BindToSource(binSizeGroupingDTO.Labels);
         gridView.BestFitColumns();
      }

      public void RefreshLabels()
      {
         gridControl.RefreshDataSource();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutControlGroupPattern.Text = PKSimConstants.UI.LabelGeneration;
         layoutItemNumberOfBins.Text = PKSimConstants.UI.NumberOfBins.FormatForLabel();
         layoutItemNamingPattern.Text = PKSimConstants.UI.NamingPattern.FormatForLabel();
         lblNamingPatternDescription.AsDescription();
         lblNamingPatternDescription.Text = _presenter.NamingPatternDescription.FormatForDescription();
         layoutControlGroupLabels.Text = PKSimConstants.UI.GeneratedLabels;
         layoutItemGenerationStrategie.Text = PKSimConstants.UI.NamingPatternStrategy.FormatForLabel();
         layoutItemStartColor.Text = PKSimConstants.UI.StartColor.FormatForLabel();
         layoutItemEndColor.Text = PKSimConstants.UI.EndColor.FormatForLabel();
         layoutItemSymbol.Text = PKSimConstants.UI.Symbol.FormatForLabel();
      }

      private void getObjectToolTip(ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         if (e.SelectedControl != tbNamingPattern) return;

         var superToolTip = _toolTipCreator.CreateToolTip(_presenter.NamingPatternDescriptionToolTip, PKSimConstants.UI.NamingPattern);
         e.Info = _toolTipCreator.ToolTipControlInfoFor(tbNamingPattern, superToolTip);
      }
   }
}