using System.Drawing;
using OSPSuite.Assets;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.Presenters.Observers;
using PKSim.Presentation.Views.Observers;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Observers
{
   public partial class CreateObserverBuildingBlockView : BuildingBlockContainerView, ICreateObserverBuildingBlockView
   {
      public CreateObserverBuildingBlockView(Shell shell) : base(shell)
      {
         InitializeComponent();
         ClientSize = new Size(CoreConstants.UI.OBSERVER_VIEW_WITDH, CoreConstants.UI.OBSERVER_VIEW_HEIGHT);
      }

      public void AttachPresenter(ICreateObserverBuildingBlockPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Icon = ApplicationIcons.Observer.WithSize(IconSizes.Size16x16);
         Caption = PKSimConstants.UI.CreateObserverBuildingBlock;
      }
   }
}