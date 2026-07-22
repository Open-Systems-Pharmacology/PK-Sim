using System.Collections.Generic;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Controls;
using OSPSuite.UI.RepositoryItems;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class ValueMappingGroupingView : BaseUserControl, IValueMappingGroupingView
   {
      private IValueMappingGroupingPresenter _presenter;
      private readonly GridViewBinder<GroupingLabelDTO> _gridViewBinder;
      private readonly UxRepositoryItemComboBox _symbolsRepository;

      public ValueMappingGroupingView()
      {
         InitializeComponent();
         _gridViewBinder = new GridViewBinder<GroupingLabelDTO>(gridView);
         gridView.AllowsFiltering = false;
         gridView.ShowRowIndicator = false;
         _symbolsRepository = new UxRepositoryItemSymbols(gridView);
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.AutoBind(x => x.Value).AsReadOnly();
         _gridViewBinder.AutoBind(x => x.Label);
         _gridViewBinder.AutoBind(x => x.Sequence);
         _gridViewBinder.AutoBind(x => x.Color);
         _gridViewBinder.AutoBind(x => x.Symbol)
            .WithRepository(x => _symbolsRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);

         _gridViewBinder.Changed += NotifyViewChanged;
      }

      public void AttachPresenter(IValueMappingGroupingPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IEnumerable<GroupingLabelDTO> groupingLabels)
      {
         _gridViewBinder.BindToSource(groupingLabels);
      }
   }
}