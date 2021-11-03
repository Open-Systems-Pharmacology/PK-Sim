using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters.ExpressionProfiles
{
   public interface ICreateExpressionProfilePresenter : ICreateBuildingBlockPresenter<ExpressionProfile>, IContainerPresenter
   {
      ExpressionProfile ExpressionProfile { get; }
   }

   public class CreateExpressionProfilePresenter : AbstractSubPresenterContainerPresenter<ICreateExpressionProfileView, ICreateExpressionProfilePresenter, IExpressionProfileItemPresenter>, ICreateExpressionProfilePresenter
   {
      private readonly IBuildingBlockPropertiesMapper _propertiesMapper;
      private readonly IObjectBaseDTOFactory _buildingBlockDTOFactory;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private ObjectBaseDTO _expressionProfileDTO;
      public ExpressionProfile ExpressionProfile { get; private set; }

      public CreateExpressionProfilePresenter(
         ICreateExpressionProfileView view,
         ISubPresenterItemManager<IExpressionProfileItemPresenter> subPresenterItemManager,
         IDialogCreator dialogCreator,
         IBuildingBlockPropertiesMapper propertiesMapper,
         IObjectBaseDTOFactory buildingBlockDTOFactory,
         IObjectBaseFactory objectBaseFactory) : base(view, subPresenterItemManager, ExpressionProfileItems.All, dialogCreator)
      {
         _propertiesMapper = propertiesMapper;
         _buildingBlockDTOFactory = buildingBlockDTOFactory;
         _objectBaseFactory = objectBaseFactory;
      }

      public IPKSimCommand Create()
      {
         _expressionProfileDTO = _buildingBlockDTOFactory.CreateFor<ExpressionProfile>();
         ExpressionProfile = _objectBaseFactory.Create<ExpressionProfile>();
         ExpressionProfile.IsLoaded = true;
         _view.BindToProperties(_expressionProfileDTO);
         _subPresenterItemManager.AllSubPresenters.Each(x => x.Edit(ExpressionProfile));
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         _propertiesMapper.MapProperties(_expressionProfileDTO, ExpressionProfile);

         return new PKSimMacroCommand();
      }

      public ExpressionProfile BuildingBlock => ExpressionProfile;
   }
}