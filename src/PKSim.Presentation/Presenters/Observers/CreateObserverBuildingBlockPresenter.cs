using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Observers;

namespace PKSim.Presentation.Presenters.Observers
{
   public interface ICreateObserverBuildingBlockPresenter : ICreateBuildingBlockPresenter<PKSimObserverBuildingBlock>, IContainerPresenter
   {
      PKSimObserverBuildingBlock Observers { get; }
   }

   public class CreateObserverBuildingBlockPresenter : AbstractSubPresenterContainerPresenter<ICreateObserverBuildingBlockView, ICreateObserverBuildingBlockPresenter, IObserverItemPresenter>, ICreateObserverBuildingBlockPresenter
   {
      private readonly IObjectBaseDTOFactory _buildingBlockDTOFactory;
      private readonly IBuildingBlockPropertiesMapper _propertiesMapper;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private ObjectBaseDTO _observedBuildingBlockDTO;
      public PKSimObserverBuildingBlock Observers { get; private set; }

      public CreateObserverBuildingBlockPresenter(
         ICreateObserverBuildingBlockView view,
         ISubPresenterItemManager<IObserverItemPresenter> subPresenterItemManager,
         IDialogCreator dialogCreator,
         IObjectBaseDTOFactory buildingBlockDTOFactory,
         IBuildingBlockPropertiesMapper propertiesMapper,
         IObjectBaseFactory objectBaseFactory
      ) : base(view, subPresenterItemManager, ObserverItems.All, dialogCreator)
      {
         _buildingBlockDTOFactory = buildingBlockDTOFactory;
         _propertiesMapper = propertiesMapper;
         _objectBaseFactory = objectBaseFactory;
      }

      public IPKSimCommand Create()
      {
         _observedBuildingBlockDTO = _buildingBlockDTOFactory.CreateFor<PKSimObserverBuildingBlock>();
         _view.BindToProperties(_observedBuildingBlockDTO);
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();


         Observers = _objectBaseFactory.Create<PKSimObserverBuildingBlock>();

         _propertiesMapper.MapProperties(_observedBuildingBlockDTO, BuildingBlock);

         return new PKSimMacroCommand();
      }

      public PKSimObserverBuildingBlock BuildingBlock => Observers;
   }
}