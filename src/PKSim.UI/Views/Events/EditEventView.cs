using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Presentation.Presenters.Events;
using PKSim.Presentation.Views.Events;

namespace PKSim.UI.Views.Events
{
   public partial class EditEventView : BaseMdiChildView, IEditEventView
   {
      public EditEventView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IEditEventPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void AddSubItemView(ISubPresenterItem subPresenterItem, IView viewToAdd)
      {
         this.FillWith(viewToAdd);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.Event;
      }
   }
}