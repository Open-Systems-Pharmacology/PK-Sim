using OSPSuite.Assets;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters
{
   public interface ICloneExpressionProfilePresenter : ICloneBuildingBlockPresenter
   {
   }

   public class CloneExpressionProfilePresenter : AbstractDisposablePresenter<ICloneExpressionProfileView, ICloneExpressionProfilePresenter>, ICloneExpressionProfilePresenter
   {
      private readonly IExpressionProfileToExpressionProfileDTOMapper _expressionProfileDTOMapper;
      private readonly IExpressionProfileUpdater _expressionProfileUpdater;
      private readonly IExpressionProfileFactory _expressionProfileFactory;

      public CloneExpressionProfilePresenter(
         ICloneExpressionProfileView view,
         IExpressionProfileToExpressionProfileDTOMapper expressionProfileDTOMapper, 
         IExpressionProfileUpdater expressionProfileUpdater, 
         IExpressionProfileFactory expressionProfileFactory) : base(view)
      {
         _expressionProfileDTOMapper = expressionProfileDTOMapper;
         _expressionProfileUpdater = expressionProfileUpdater;
         _expressionProfileFactory = expressionProfileFactory;
      }

      public IPKSimBuildingBlock CreateCloneFor(IPKSimBuildingBlock buildingBlockToClone)
      {
         //This should never fail
         var expressionProfile = buildingBlockToClone.DowncastTo<ExpressionProfile>();
         var (molecule, _) = expressionProfile;
         var expressionProfileDTO = _expressionProfileDTOMapper.MapFrom(expressionProfile);
         _view.Caption = Captions.CloneObjectBase(PKSimConstants.ObjectTypes.ExpressionProfile, buildingBlockToClone.Name);

         _view.BindTo(expressionProfileDTO);
         _view.Display();

         if (_view.Canceled)
            return null;

         //create a new expression profile using the same molecule name as the original so that we can update the values. Then we rename
         var newExpressionProfile = _expressionProfileFactory.CreateFor(molecule.MoleculeType, expressionProfileDTO.Species.Name, molecule.Name);
         newExpressionProfile.Category = expressionProfileDTO.Category;

         //synchronize values
         _expressionProfileUpdater.SynchronizeExpressionProfileWithExpressionProfile(expressionProfile, newExpressionProfile);

         //rename using the new name
         _expressionProfileUpdater.UpdateMoleculeName(newExpressionProfile, expressionProfileDTO.MoleculeName);
         
         return newExpressionProfile;
      }

      public override void ViewChanged()
      {
         base.ViewChanged();
         _view.OkEnabled = CanClose;
      }
   }
}