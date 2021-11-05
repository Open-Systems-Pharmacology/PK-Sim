using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters.ExpressionProfiles
{
   public interface IRenameExpressionProfilePresenter : IRenamePresenter
   {
   }

   public class RenameExpressionProfilePresenter : AbstractDisposablePresenter<IRenameExpressionProfileView, IRenameExpressionProfilePresenter>, IRenameExpressionProfilePresenter
   {
      private readonly IExpressionProfileToExpressionProfileDTOMapper _expressionProfileDTOMapper;

      public RenameExpressionProfilePresenter(
         IRenameExpressionProfileView view,
         IExpressionProfileToExpressionProfileDTOMapper expressionProfileDTOMapper) : base(view)
      {
         _expressionProfileDTOMapper = expressionProfileDTOMapper;
      }

      public string NewNameFrom(IWithName namedObject, IEnumerable<string> forbiddenNames, string entityType = null)
      {
         //This should never fail
         var expressionProfile = namedObject.DowncastTo<ExpressionProfile>();
         var expressionProfileDTO = _expressionProfileDTOMapper.MapFrom(expressionProfile);
         _view.Caption = PKSimConstants.UI.RenameEntityCaption(PKSimConstants.ObjectTypes.ExpressionProfile, namedObject.Name);

         _view.BindTo(expressionProfileDTO);
         _view.Display();

         if (_view.Canceled)
            return string.Empty;

         return expressionProfileDTO.Name;
      }

      public override void ViewChanged()
      {
         base.ViewChanged();
         _view.OkEnabled = CanClose;
      }
   }
}