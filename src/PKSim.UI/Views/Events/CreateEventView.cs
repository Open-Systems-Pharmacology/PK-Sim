using PKSim.Assets;
using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Events;
using PKSim.Presentation.Views.Events;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation;

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
         Icon = ApplicationIcons.Event.WithSize(IconSizes.Size16x16);
         Caption = PKSimConstants.UI.CreateEvent;
      }

      protected override int TopicId => HelpId.PKSim_Events_NewEvent;
   }
}