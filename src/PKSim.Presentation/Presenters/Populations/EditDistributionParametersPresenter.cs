using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface IEditDistributionParametersPresenter : IMultiParameterEditPresenter
   {
   }

   public class EditDistributionParametersPresenter : MultiParameterEditPresenter, IEditDistributionParametersPresenter
   {
      public EditDistributionParametersPresenter(IMultiParameterEditView view, IScaleParametersPresenter scaleParametersPresenter, IEditParameterPresenterTask editParameterPresenterTask,
         IParameterTask parameterTask, IParameterToParameterDTOMapper parameterDTOMapper, IParameterContextMenuFactory contextMenuFactory)
         : base(view, scaleParametersPresenter, editParameterPresenterTask, parameterTask, parameterDTOMapper, contextMenuFactory)
      {
         IsSimpleEditor = true;
      }

      public override void SetParameterValue(IParameterDTO parameterDTO, double valueInGuiUnit)
      {
         AddCommand(_parameterTask.SetAdvancedParameterDisplayValue(ParameterFrom(parameterDTO), valueInGuiUnit));
      }

      public override void SetParameterUnit(IParameterDTO parameterDTO, Unit displayUnit)
      {
         AddCommand(_parameterTask.SetAdvancedParameterUnit(ParameterFrom(parameterDTO), displayUnit));
      }
   }
}