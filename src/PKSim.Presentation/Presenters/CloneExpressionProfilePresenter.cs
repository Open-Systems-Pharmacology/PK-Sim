using OSPSuite.Assets;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters
{
   public interface ICloneExpressionProfilePresenter : ICloneBuildingBlockPresenter, IExpressionProfilePresenter
   {
   }

   public class CloneExpressionProfilePresenter : AbstractDisposablePresenter<ICreateExpressionProfileView, ICloneExpressionProfilePresenter>, ICloneExpressionProfilePresenter
   {
      private readonly IExpressionProfileToExpressionProfileDTOMapper _expressionProfileDTOMapper;
      private readonly IExpressionProfileUpdater _expressionProfileUpdater;
      private readonly IExpressionProfileFactory _expressionProfileFactory;
      private ExpressionProfileDTO _dto;
      private readonly IDialogCreator _dialogCreator;

      public CloneExpressionProfilePresenter(
         ICreateExpressionProfileView view,
         IExpressionProfileToExpressionProfileDTOMapper expressionProfileDTOMapper,
         IExpressionProfileUpdater expressionProfileUpdater,
         IExpressionProfileFactory expressionProfileFactory,
         IDialogCreator dialogCreator) : base(view)
      {
         _expressionProfileDTOMapper = expressionProfileDTOMapper;
         _expressionProfileUpdater = expressionProfileUpdater;
         _expressionProfileFactory = expressionProfileFactory;
         _dialogCreator = dialogCreator;
      }

      public IPKSimBuildingBlock CreateCloneFor(IPKSimBuildingBlock buildingBlockToClone)
      {
         //This should never fail
         var expressionProfile = buildingBlockToClone.DowncastTo<ExpressionProfile>();
         var (molecule, _) = expressionProfile;
         _dto = _expressionProfileDTOMapper.MapFrom(expressionProfile);
         _view.Caption = Captions.CloneObjectBase(PKSimConstants.ObjectTypes.ExpressionProfile, buildingBlockToClone.Name);

         _view.BindTo(_dto);
         _view.Display();

         if (_view.Canceled)
            return null;

         //We have a slightly different behavior for expression profile as the name is a composite name and we need to validate the object after the fact

         //create a new expression profile using the same molecule name as the original so that we can update the values. Then we rename
         var newExpressionProfile = _expressionProfileFactory.Create(molecule.MoleculeType, _dto.Species.Name, molecule.Name);
         newExpressionProfile.Category = _dto.Category;

         //synchronize values
         _expressionProfileUpdater.SynchronizeExpressionProfileWithExpressionProfile(expressionProfile, newExpressionProfile);

         //rename using the new name
         _expressionProfileUpdater.UpdateMoleculeName(newExpressionProfile, _dto.MoleculeName);

         return newExpressionProfile;
      }

      public override void ViewChanged()
      {
         base.ViewChanged();
         _view.OkEnabled = CanClose;
      }

      public void Save()
      {
         //We have a slightly different behavior for expression profile as the name is a composite name and we need to validate the object after the fact
         var rules = _dto.Validate(x => x.Name);
         if (rules.IsEmpty)
         {
            _view.CloseView();
            return;
         }

         _dialogCreator.MessageBoxInfo(rules.Message);
      }
   }
}