using System.Collections.Generic;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.RepositoryItems;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using PKSim.UI.Views.Core;
using OSPSuite.UI.Controls;

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
         _symbolsRepository = new UxSymbolsComboBoxRepository(gridView);
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