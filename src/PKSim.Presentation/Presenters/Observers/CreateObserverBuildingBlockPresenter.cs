using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Observers;

namespace PKSim.Presentation.Presenters.Observers
{
   public interface ICreateObserverBuildingBlockPresenter : ICreateBuildingBlockPresenter<PKSimObserverBuildingBlock>
   {
   }

   public class CreateObserverBuildingBlockPresenter : AbstractDisposableCommandCollectorPresenter<ICreateObserverBuildingBlockView, ICreateObserverBuildingBlockPresenter>, ICreateObserverBuildingBlockPresenter
   {
      private readonly IObjectBaseDTOFactory _buildingBlockDTOFactory;
      private readonly IBuildingBlockPropertiesMapper _propertiesMapper;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private ObjectBaseDTO _observedBuildingBlockDTO;
      public PKSimObserverBuildingBlock BuildingBlock { get; private set; }

      public CreateObserverBuildingBlockPresenter(ICreateObserverBuildingBlockView view,
         IObjectBaseDTOFactory buildingBlockDTOFactory,
         IBuildingBlockPropertiesMapper propertiesMapper,
         IObjectBaseFactory objectBaseFactory) : base(view)
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


         BuildingBlock = _objectBaseFactory.Create<PKSimObserverBuildingBlock>();

         _propertiesMapper.MapProperties(_observedBuildingBlockDTO, BuildingBlock);

         return new PKSimMacroCommand();
      }
   }
}