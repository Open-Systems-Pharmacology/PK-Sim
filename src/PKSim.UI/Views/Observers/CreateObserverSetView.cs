using System.Drawing;
using OSPSuite.Assets;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.Presenters.Observers;
using PKSim.Presentation.Views.Observers;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Observers
{
   public partial class CreateObserverSetView : BuildingBlockContainerView, ICreateObserverSetView
   {
      public CreateObserverSetView(Shell shell) : base(shell)
      {
         InitializeComponent();
         ClientSize = new Size(CoreConstants.UI.OBSERVER_VIEW_WITDH, CoreConstants.UI.OBSERVER_VIEW_HEIGHT);
      }

      public void AttachPresenter(ICreateObserverSetPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Icon = ApplicationIcons.Observer.WithSize(IconSizes.Size16x16);
         Caption = PKSimConstants.UI.CreateObserverSet;
      }
   }
}