using System;
using OSPSuite.DataBinding;
using DevExpress.XtraLayout;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Extensions;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;

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
            .OnValueSet += (o, e) => OnEvent(() => _presenter.ProtocolSelectionChanged(e.NewValue));

         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      public override bool HasError
      {
         get { return _screenBinder.HasError; }
      }

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
         set { uxProtocolSelection.AllowEmptySelection = value; }
         get { return uxProtocolSelection.AllowEmptySelection; }
      }

      protected override void AdjustLayoutItemSize(LayoutControlItem layoutControlItem, IResizableView view, int height)
      {
         base.AdjustLayoutItemSize(layoutControlItem, view, height);
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
      }

      public void AdjustHeight()
      {
         if (_resizableView == null) return;
         _resizableView.AdjustHeight();
      }

      public void Repaint()
      {
         if (_resizableView == null) return;
         _resizableView.Repaint();
      }

      public int OptimalHeight
      {
         get { return layoutControlGroup.Height; }
      }
   }
}