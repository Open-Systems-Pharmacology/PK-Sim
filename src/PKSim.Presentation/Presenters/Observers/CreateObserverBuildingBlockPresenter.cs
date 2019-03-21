using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Observers;

namespace PKSim.Presentation.Presenters.Observers
{
   public interface ICreateObserverBuildingBlockPresenter : ICreateBuildingBlockPresenter<PKSimObserverBuildingBlock>, IContainerPresenter
   {
      PKSimObserverBuildingBlock ObserverBuildingBlock { get; }
   }

   public class CreateObserverBuildingBlockPresenter : AbstractSubPresenterContainerPresenter<ICreateObserverBuildingBlockView, ICreateObserverBuildingBlockPresenter, IObserverItemPresenter>, ICreateObserverBuildingBlockPresenter
   {
      private readonly IObjectBaseDTOFactory _buildingBlockDTOFactory;
      private readonly IBuildingBlockPropertiesMapper _propertiesMapper;
      private readonly IObserverBuildingBlockTask _observerBuildingBlockTask;
      private ObjectBaseDTO _observedBuildingBlockDTO;
      public PKSimObserverBuildingBlock ObserverBuildingBlock { get; private set; }

      public CreateObserverBuildingBlockPresenter(
         ICreateObserverBuildingBlockView view,
         ISubPresenterItemManager<IObserverItemPresenter> subPresenterItemManager,
         IDialogCreator dialogCreator,
         IObjectBaseDTOFactory buildingBlockDTOFactory,
         IBuildingBlockPropertiesMapper propertiesMapper,
         IObserverBuildingBlockTask observerBuildingBlockTask
      ) : base(view, subPresenterItemManager, ObserverItems.All, dialogCreator)
      {
         _buildingBlockDTOFactory = buildingBlockDTOFactory;
         _propertiesMapper = propertiesMapper;
         _observerBuildingBlockTask = observerBuildingBlockTask;
         ObserverBuildingBlock = observerBuildingBlockTask.Create();
      }

      public IPKSimCommand Create()
      {
         _observedBuildingBlockDTO = _buildingBlockDTOFactory.CreateFor<PKSimObserverBuildingBlock>();
         ObserverBuildingBlock = _observerBuildingBlockTask.Create();
         _subPresenterItemManager.AllSubPresenters.Each(x => x.Edit(ObserverBuildingBlock));
         _view.BindToProperties(_observedBuildingBlockDTO);
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         _propertiesMapper.MapProperties(_observedBuildingBlockDTO, ObserverBuildingBlock);

         return new PKSimMacroCommand();
      }

      public PKSimObserverBuildingBlock BuildingBlock => ObserverBuildingBlock;
   }
}