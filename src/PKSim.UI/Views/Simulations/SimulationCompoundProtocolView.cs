using System;
using OSPSuite.DataBinding;
using DevExpress.XtraLayout;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Extensions;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationCompoundProtocolView : BaseContainerUserControl, ISimulationCompoundProtocolView
   {
      private ISimulationCompoundProtocolPresenter _presenter;
      private readonly ScreenBinder<ProtocolSelectionDTO> _screenBinder;
      private IResizableView _resizableView;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };

      public SimulationCompoundProtocolView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<ProtocolSelectionDTO> {SavingMode = SavingMode.Always};
         uxProtocolSelection.AllowEmptySelection = true;
         layoutControl.AutoScroll = false;
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.BuildingBlock)
            .To(uxProtocolSelection)
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
         layoutItemProtocol.Text = uxProtocolSelection.BuildingBlockType.FormatForLabel();
      }

      public void AddFormulationMappingView(IView view)
      {
         _resizableView = view as IResizableView;
         AddViewToGroup(layoutControlGroup, view);
      }

      public bool AllowEmptyProtocolSelection
      {
         set => uxProtocolSelection.AllowEmptySelection = value;
         get => uxProtocolSelection.AllowEmptySelection;
      }

      protected override void AdjustLayoutItemSize(LayoutControlItem layoutControlItem, IResizableView view, int height)
      {
         base.AdjustLayoutItemSize(layoutControlItem, view, height);
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
   }
}