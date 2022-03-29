using PKSim.Assets;
using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Events;
using PKSim.Presentation.Views.Events;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Events
{
   public partial class CreateEventView : BuildingBlockContainerView, ICreateEventView
   {
      public CreateEventView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(ICreateEventPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.Event;
         Caption = PKSimConstants.UI.CreateEvent;
      }
   }
}