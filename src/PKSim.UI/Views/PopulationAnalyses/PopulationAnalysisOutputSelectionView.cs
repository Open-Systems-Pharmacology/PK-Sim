using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using PKSim.Assets;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using PKSim.UI.Extensions;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class PopulationAnalysisOutputSelectionView : BaseUserControl, IPopulationAnalysisOutputSelectionView
   {
      private IPopulationAnalysisOutputSelectionPresenter _presenter;
      private readonly ScreenBinder<PopulationStatisticalAnalysis> _screenBinder;

      public PopulationAnalysisOutputSelectionView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<PopulationStatisticalAnalysis>();
      }

      public void AttachPresenter(IPopulationAnalysisOutputSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public override ApplicationIcon ApplicationIcon
      {
         get { return ApplicationIcons.Simulation; }
      }

      public void AddPopulationOutputsView(IView view)
      {
         panelOutputs.FillWith(view);
      }

      public void AddSelectedOutputsView(IView view)
      {
         panelSelectedOutputs.FillWith(view);
      }

      public void AddStatisticsSelectionView(IView view)
      {
         panelStatistics.FillWith(view);
      }

      public void BindTo(PopulationStatisticalAnalysis populationAnalysis)
      {
         _screenBinder.BindToSource(populationAnalysis);
      }

      public override void InitializeBinding()
      {
         btnAdd.Click += (o, e) => OnEvent(_presenter.AddOutput);
         btnRemove.Click += (o, e) => OnEvent(_presenter.RemoveOutput);

         _screenBinder.Bind(x => x.TimeUnit)
            .To(cbTimeUnit)
            .WithValues(x => _presenter.AllTimeUnits);

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutControl.DoInBatch(() =>
         {
            layoutItemButtonRemove.AsRemoveButton();
            layoutItemButtonAdd.AsAddButton();
         });
         Caption = PKSimConstants.UI.Output;
         layoutItemTimeUnit.Text = PKSimConstants.UI.TimeUnit.FormatForLabel();
      }
   }
}