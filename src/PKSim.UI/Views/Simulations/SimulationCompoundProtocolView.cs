using System;
using DevExpress.XtraLayout;
using OSPSuite.DataBinding;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Extensions;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundProtocolView : BaseContainerUserControl, ISimulationCompoundProtocolView
   {
      private ISimulationCompoundProtocolPresenter _presenter;
      private readonly ScreenBinder<ProtocolSelectionDTO> _screenBinder;
      private IResizableView _resizableView;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };
      private readonly UxBuildingBlockSelection _uxProtocolSelection = new UxBuildingBlockSelection {AllowEmptySelection = true};
      public SimulationCompoundProtocolView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<ProtocolSelectionDTO> {SavingMode = SavingMode.Always};
         // layoutControl.AutoScroll = false;
         layoutItemProtocol.FillWith(_uxProtocolSelection);
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.BuildingBlock)
            .To(_uxProtocolSelection)
            .OnValueUpdating += (o, e) => OnEvent(() => _presenter.ProtocolSelectionChanged(e.NewValue));

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      public override bool HasError => _screenBinder.HasError;

      public void AttachPresenter(ISimulationCompoundProtocolPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(ProtocolSelectionDTO protocolSelectionDTO)
      {
         _screenBinder.BindToSource(protocolSelectionDTO);
         layoutItemProtocol.Text = _uxProtocolSelection.BuildingBlockType.FormatForLabel();
      }

      public void AddFormulationMappingView(IView view)
      {
         _resizableView = view as IResizableView;
         AddViewTo(layoutItemFormulation,layoutControl,  view);
      }

      public bool AllowEmptyProtocolSelection
      {
         set => _uxProtocolSelection.AllowEmptySelection = value;
         get => _uxProtocolSelection.AllowEmptySelection;
      }

      protected override void AdjustLayoutItemSize(LayoutControlItem layoutControlItem, LayoutControl layoutControl, IResizableView view, int height)
      {
         base.AdjustLayoutItemSize(layoutControlItem, layoutControl, view, height);
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
      }

      public void AdjustHeight()
      {
         _resizableView?.AdjustHeight();
      }

      public void Repaint()
      {
         _resizableView?.Repaint();
      }

      public int OptimalHeight => layoutControlGroup.Height;

      public int DefaultHeight => UIConstants.Size.SIMULATION_COMPOUND_PROTOCOL_DEFAULT_HEIGHT;
   }
}