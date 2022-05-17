using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters.ExpressionProfiles
{
   public interface IRenameExpressionProfilePresenter : IRenamePresenter, IExpressionProfilePresenter
   {
   }

   public class RenameExpressionProfilePresenter : AbstractDisposablePresenter<IRenameExpressionProfileView, IRenameExpressionProfilePresenter>, IRenameExpressionProfilePresenter
   {
      private readonly IExpressionProfileToExpressionProfileDTOMapper _expressionProfileDTOMapper;
      private ExpressionProfileDTO _expressionProfileDTO;
      private readonly IDialogCreator _dialogCreator;

      public RenameExpressionProfilePresenter(
         IRenameExpressionProfileView view,
         IExpressionProfileToExpressionProfileDTOMapper expressionProfileDTOMapper,
         IDialogCreator dialogCreator) : base(view)
      {
         _expressionProfileDTOMapper = expressionProfileDTOMapper;
         _dialogCreator = dialogCreator;
      }

      public string NewNameFrom(IWithName namedObject, IEnumerable<string> forbiddenNames, string entityType = null)
      {
         //This should never fail
         var expressionProfile = namedObject.DowncastTo<ExpressionProfile>();
         _expressionProfileDTO = _expressionProfileDTOMapper.MapFrom(expressionProfile);
         _view.Caption = PKSimConstants.UI.RenameEntityCaption(PKSimConstants.ObjectTypes.ExpressionProfile, namedObject.Name);

         _view.BindTo(_expressionProfileDTO);
         _view.Display();

         if (_view.Canceled)
            return string.Empty;

         return _expressionProfileDTO.Name;
      }

      public override void ViewChanged()
      {
         base.ViewChanged();
         _view.OkEnabled = CanClose;
      }

      public void Save()
      {
         //We have a slightly different behavior for expression profile as the name is a composite name and we need to validate the object after the fact
         var rules = _expressionProfileDTO.Validate();
         if (rules.IsEmpty)
         {
            _view.CloseView();
            return;
         }

         _dialogCreator.MessageBoxInfo(rules.Message);
      }
   }
}