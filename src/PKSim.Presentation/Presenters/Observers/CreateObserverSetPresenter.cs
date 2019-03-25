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
   public interface ICreateObserverSetPresenter : ICreateBuildingBlockPresenter<ObserverSet>, IContainerPresenter
   {
      ObserverSet ObserverSet { get; }
   }

   public class CreateObserverSetPresenter : AbstractSubPresenterContainerPresenter<ICreateObserverSetView, ICreateObserverSetPresenter, IObserverSetItemPresenter>, ICreateObserverSetPresenter
   {
      private readonly IObjectBaseDTOFactory _buildingBlockDTOFactory;
      private readonly IBuildingBlockPropertiesMapper _propertiesMapper;
      private readonly IObserverSetFactory _observerSetFactory;
      private ObjectBaseDTO _observedBuildingBlockDTO;
      public ObserverSet ObserverSet { get; private set; }

      public CreateObserverSetPresenter(
         ICreateObserverSetView view,
         ISubPresenterItemManager<IObserverSetItemPresenter> subPresenterItemManager,
         IDialogCreator dialogCreator,
         IObjectBaseDTOFactory buildingBlockDTOFactory,
         IBuildingBlockPropertiesMapper propertiesMapper,
         IObserverSetFactory observerSetFactory

      ) : base(view, subPresenterItemManager, ObserverSetItems.All, dialogCreator)
      {
         _buildingBlockDTOFactory = buildingBlockDTOFactory;
         _propertiesMapper = propertiesMapper;
         _observerSetFactory = observerSetFactory;
      }

      public IPKSimCommand Create()
      {
         _observedBuildingBlockDTO = _buildingBlockDTOFactory.CreateFor<ObserverSet>();
         ObserverSet = _observerSetFactory.Create();
         _subPresenterItemManager.AllSubPresenters.Each(x => x.Edit(ObserverSet));
         _view.BindToProperties(_observedBuildingBlockDTO);
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         _propertiesMapper.MapProperties(_observedBuildingBlockDTO, ObserverSet);

         return new PKSimMacroCommand();
      }

      public ObserverSet BuildingBlock => ObserverSet;
   }
}