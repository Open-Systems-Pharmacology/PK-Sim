using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.DTO;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IHalogensPresenter : IMultiParameterEditPresenter
   {
      void Edit(IReadOnlyList<IParameter> halogens, IParameter effectiveMolWeight);
   }

   public class HalogensPresenter : MultiParameterEditPresenter, IHalogensPresenter
   {
      private readonly IDialogCreator _dialogCreator;
      private IReadOnlyList<IParameter> _halogens;
      private IParameter _effectiveMolWeight;

      public HalogensPresenter(
         IMultiParameterEditView view,
         IScaleParametersPresenter scaleParametersPresenter,
         IEditParameterPresenterTask editParameterPresenterTask,
         IParameterTask parameterTask,
         IParameterToParameterDTOMapper parameterDTOMapper,
         IParameterContextMenuFactory contextMenuFactory,
         IDialogCreator dialogCreator) :
         base(view, scaleParametersPresenter, editParameterPresenterTask, parameterTask, parameterDTOMapper, contextMenuFactory)
      {
         _dialogCreator = dialogCreator;
      }

      public override void SetParameterValue(IParameterDTO parameterDTO, double valueInDisplayUnits)
      {
         var parameter = parameterDTO.Parameter;

         if (!parameter.IsEffectiveMolWeightPositive(parameter.ConvertToBaseUnit(valueInDisplayUnits), _effectiveMolWeight))
         {
            _dialogCreator.MessageBoxError(PKSimConstants.Error.EffectiveMolWeightCannotBeNegative);
            return;
         }

         base.SetParameterValue(parameterDTO, valueInDisplayUnits);
      }

      public void Edit(IReadOnlyList<IParameter> halogens, IParameter effectiveMolWeight)
      {
         _halogens = halogens;
         _effectiveMolWeight = effectiveMolWeight;
         base.Edit(_halogens);
      }
   }
}