using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Views.Events;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Events
{
   public interface IEditEventPresenter : IEditBuildingBockPresenter<PKSimEvent>
   {
   }

   public class EditEventPresenter : SingleStartContainerPresenter<IEditEventView, IEditEventPresenter, PKSimEvent, IEventItemPresenter>, IEditEventPresenter
   {
      private PKSimEvent _event;

      public EditEventPresenter(IEditEventView view, ISubPresenterItemManager<IEventItemPresenter> subPresenterItemManager)
         : base(view, subPresenterItemManager, EventItems.All)
      {
      }

      public override void Edit(PKSimEvent pkSimEvent)
      {
         _event = pkSimEvent;
         UpdateCaption();
         eventSettingsPresenter.CanEditEventTemplate = false;
         eventSettingsPresenter.EditEvent(_event);
         _view.Display();
      }

      public override object Subject => _event;

      protected override void UpdateCaption()
      {
         _view.Caption = PKSimConstants.UI.EditEvent(_event.Name);
      }

      private IEventSettingsPresenter eventSettingsPresenter => PresenterAt(EventItems.Settings);
   }
}