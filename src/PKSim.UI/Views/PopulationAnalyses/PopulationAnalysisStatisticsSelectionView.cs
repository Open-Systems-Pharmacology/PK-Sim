using System.Collections.Generic;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.Assets;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using PKSim.Assets;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class PopulationAnalysisStatisticsSelectionView : BaseUserControl, IPopulationAnalysisStatisticsSelectionView
   {
      private IPopulationAnalysisStatisticsSelectionPresenter _presenter;
      private readonly GridViewBinder<StatisticalAggregation> _gridViewBinder;
      private readonly RepositoryItem _lineStyleRepository;
      private readonly UxRepositoryItemCheckEdit _selectionRepository;

      public PopulationAnalysisStatisticsSelectionView()
      {
         InitializeComponent();
         gridView.AllowsFiltering = false;
         _gridViewBinder = new GridViewBinder<StatisticalAggregation>(gridView);
         _lineStyleRepository = new UxLineStylesComboBoxRepository(gridView, removeLineStyleNone: true);
         _selectionRepository = new UxRepositoryItemCheckEdit(gridView);
      }

      public void AttachPresenter(IPopulationAnalysisStatisticsSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.Bind(x => x.Selected)
            .WithRepository(x => _selectionRepository);

         _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.Name)
            .WithRepository(editRepository)
            .AsReadOnly();

         _gridViewBinder.Bind(x => x.LineStyle)
            .WithCaption(PKSimConstants.UI.LineStyle)
            .WithRepository(x => _lineStyleRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);

         _gridViewBinder.Changed += NotifyViewChanged;
      }

      private RepositoryItem editRepository(StatisticalAggregation statisticalAggregation) => new RepositoryItemTextEdit
      {
         NullText = _presenter.DisplayNameFor(statisticalAggregation)
      };

      public void BindTo(IEnumerable<StatisticalAggregation> selection)
      {
         _gridViewBinder.BindToSource(selection);
         gridView.BestFitColumns();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         lblDescription.AsDescription();
         lblDescription.Text = PKSimConstants.UI.StatisticalOutputSelectionDescription;
         ApplicationIcon = ApplicationIcons.Simulation;
      }
   }
}