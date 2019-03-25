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
   public interface IEditObserverBuildingBlockPresenter : IEditBuildingBockPresenter<PKSimObserverBuildingBlock>,
      IListener<RemoveObserverFromObserverBuildingBlockEvent>,
      IListener<AddObserverToObserverBuildingBlockEvent>
   {
   }

   public class EditObserverBuildingBlockPresenter : SingleStartContainerPresenter<IEditObserverBuildingBlockView, IEditObserverBuildingBlockPresenter, PKSimObserverBuildingBlock, IObserverItemPresenter>, IEditObserverBuildingBlockPresenter
   {
      private PKSimObserverBuildingBlock _observerBuildingBlock;

      public EditObserverBuildingBlockPresenter(IEditObserverBuildingBlockView view, ISubPresenterItemManager<IObserverItemPresenter> subPresenterItemManager) :
         base(view, subPresenterItemManager, ObserverItems.All)
      {
      }
      

      protected override void UpdateCaption()
      {
         _view.Caption = PKSimConstants.UI.EditObserverBuildingBlock(_observerBuildingBlock.Name);
      }

      public override object Subject => _observerBuildingBlock;

      public override void Edit(PKSimObserverBuildingBlock observerBuildingBlock)
      {
         _observerBuildingBlock = observerBuildingBlock;
         _subPresenterItemManager.AllSubPresenters.Each(x => x.Edit(_observerBuildingBlock));
         importObserversPresenter.ShowFilePath = false;
         UpdateCaption();
         _view.Display();
      }

      public void Handle(RemoveObserverFromObserverBuildingBlockEvent eventToHandle) => handle(eventToHandle);

      public void Handle(AddObserverToObserverBuildingBlockEvent eventToHandle) => handle(eventToHandle);

      private void handle(IEntityContainerEvent eventToHandle)
      {
         if (!canHandle(eventToHandle))
            return;

         Edit(_observerBuildingBlock);
      }

      private bool canHandle(IEntityContainerEvent eventToHandle)
      {
         return Equals(eventToHandle.ContainerSubject, _observerBuildingBlock);
      }

      private IImportObserversPresenter importObserversPresenter => _subPresenterItemManager.PresenterAt(ObserverItems.ImportSettings);
   }
}