using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Presentation.Views.Observers;

namespace PKSim.Presentation.Presenters.Observers
{
   public interface IEditObserverSetPresenter : IEditBuildingBockPresenter<ObserverSet>,
      IListener<RemoveObserverFromObserverSetEvent>,
      IListener<AddObserverToObserverSetEvent>
   {
   }

   public class EditObserverSetPresenter : SingleStartContainerPresenter<IEditObserverSetView, IEditObserverSetPresenter, ObserverSet, IObserverSetItemPresenter>, IEditObserverSetPresenter
   {
      private ObserverSet _observerSet;

      public EditObserverSetPresenter(IEditObserverSetView view, ISubPresenterItemManager<IObserverSetItemPresenter> subPresenterItemManager) :
         base(view, subPresenterItemManager, ObserverSetItems.All)
      {
      }
      

      protected override void UpdateCaption()
      {
         _view.Caption = PKSimConstants.UI.EditObserverSet(_observerSet.Name);
      }

      public override object Subject => _observerSet;

      public override void Edit(ObserverSet observerSet)
      {
         _observerSet = observerSet;
         _subPresenterItemManager.AllSubPresenters.Each(x => x.Edit(_observerSet));
         importObserversPresenter.ShowFilePath = false;
         UpdateCaption();
         _view.Display();
      }

      public void Handle(RemoveObserverFromObserverSetEvent eventToHandle) => handle(eventToHandle);

      public void Handle(AddObserverToObserverSetEvent eventToHandle) => handle(eventToHandle);

      private void handle(IEntityContainerEvent eventToHandle)
      {
         if (!canHandle(eventToHandle))
            return;

         Edit(_observerSet);
      }

      private bool canHandle(IEntityContainerEvent eventToHandle)
      {
         return Equals(eventToHandle.ContainerSubject, _observerSet);
      }

      private IImportObserverSetPresenter importObserversPresenter => _subPresenterItemManager.PresenterAt(ObserverSetItems.ImportSettings);
   }
}