using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Presentation.Presenters.Observers;
using PKSim.Presentation.Views.Observers;

namespace PKSim.UI.Views.Observers
{
   public partial class EditObserverSetView : BaseMdiChildView, IEditObserverSetView
   {
      public EditObserverSetView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      public override void AddSubItemView(ISubPresenterItem subPresenterItem, IView viewToAdd)
      {
         panelControl.FillWith(viewToAdd);
      }

      public void AttachPresenter(IEditObserverSetPresenter presenter)
      {
         _presenter = presenter;
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Observer;
   }
}