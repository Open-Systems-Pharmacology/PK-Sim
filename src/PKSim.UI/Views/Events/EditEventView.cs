using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Events;
using PKSim.Presentation.Views.Events;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

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

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Event;
   }
}