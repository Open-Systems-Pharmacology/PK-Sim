using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Assets;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using PKSim.Assets;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationSelectionForComparisonView : BaseModalView, ISimulationSelectionForComparisonView
   {
      private ISimulationSelectionForComparisonPresenter _presenter;
      private readonly GridViewBinder<PopulationSimulationSelectionDTO> _gridViewBinder;
      private readonly ScreenBinder<SimulationComparisonSelectionDTO> _screenBinder;
      private readonly ScreenBinder<GroupingItemDTO> _groupingItemBinder;
      private readonly UxRepositoryItemCheckEdit _selectEditor;

      public SimulationSelectionForComparisonView()
      {
         InitializeComponent();
         _gridViewBinder = new GridViewBinder<PopulationSimulationSelectionDTO>(gridView);
         _screenBinder = new ScreenBinder<SimulationComparisonSelectionDTO>();
         _groupingItemBinder = new ScreenBinder<GroupingItemDTO>();
         gridView.AllowsFiltering = false;
         _selectEditor = new UxRepositoryItemCheckEdit(gridView);
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.Bind(x => x.Selected)
            .WithRepository(selectRepository)
            .WithEditorConfiguration(editSelectionRepository)
            .WithFixedWidth(OSPSuite.UI.UIConstants.Size.EMBEDDED_CHECK_BOX_WIDTH);

         _gridViewBinder.Bind(x => x.Name)
            .AsReadOnly();

         _screenBinder.Bind(x => x.Reference)
            .To(cbReferenceSimulation)
            .WithValues(dto => _presenter.AvailableSimulations())
            .Changed += () => OnEvent(_presenter.ReferenceSimulationChanged);

         _groupingItemBinder.Bind(x => x.Color)
            .To(colorSelection);

         _groupingItemBinder.Bind(x => x.Label)
            .To(tbLabel);

         _groupingItemBinder.Bind(x => x.Symbol)
            .To(cbSymbol)
            .WithValues(dto => _presenter.AllSymbols());

         _gridViewBinder.Changed += NotifyViewChanged;

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
         RegisterValidationFor(_groupingItemBinder, NotifyViewChanged);
      }

      private void editSelectionRepository(BaseEdit baseEdit, PopulationSimulationSelectionDTO dto)
      {
         baseEdit.Enabled = !_presenter.ReferenceSimulationIs(dto.Simulation);
      }

      private RepositoryItem selectRepository(PopulationSimulationSelectionDTO dto)
      {
         return _selectEditor;
      }

      public void AttachPresenter(ISimulationSelectionForComparisonPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(SimulationComparisonSelectionDTO simulationComparisonSelectionDTO)
      {
         _gridViewBinder.BindToSource(simulationComparisonSelectionDTO.AllSimulations);
         _screenBinder.BindToSource(simulationComparisonSelectionDTO);
         _groupingItemBinder.BindToSource(simulationComparisonSelectionDTO.GroupingItem);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.UI.SelectSimulationForComparison;
         Icon = ApplicationIcons.PopulationSimulationComparison.WithSize(IconSizes.Size16x16);
         lblDescription.AsDescription();
         lblDescription.Text = PKSimConstants.UI.SelectSimulationForComparisonDescription.FormatForDescription();
         lblReferenceDescription.AsDescription();
         lblReferenceDescription.Text = PKSimConstants.UI.ReferenceSimulationDescription.FormatForDescription();
         layoutItemLabel.Text = PKSimConstants.UI.Label.FormatForLabel();
         layoutItemColorSelection.Text = PKSimConstants.UI.Color.FormatForLabel();
         layoutItemSymbol.Text = PKSimConstants.UI.Symbol.FormatForLabel();
         layoutItemReferenceSimulation.Text = PKSimConstants.UI.Reference.FormatForLabel();
         layoutGroupReferenceSimulation.Text = PKSimConstants.UI.ReferenceSimulation;
      }

      public override bool HasError
      {
         get { return _gridViewBinder.HasError || _screenBinder.HasError || _groupingItemBinder.HasError; }
      }
   }
}