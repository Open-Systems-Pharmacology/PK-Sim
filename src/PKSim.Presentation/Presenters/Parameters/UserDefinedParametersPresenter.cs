using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface IUserDefinedParametersPresenter : ICustomParametersPresenter
   {
   }

   public class UserDefinedParametersPresenter : MultiParameterEditPresenter, IUserDefinedParametersPresenter
   {
      public override bool AlwaysRefresh { get; } = true;

      public UserDefinedParametersPresenter(IMultiParameterEditView view, IScaleParametersPresenter scaleParametersPresenter, IEditParameterPresenterTask editParameterPresenterTask, IParameterTask parameterTask, IParameterToParameterDTOMapper parameterDTOMapper, IParameterContextMenuFactory contextMenuFactory) : base(view, scaleParametersPresenter, editParameterPresenterTask, parameterTask, parameterDTOMapper, contextMenuFactory)
      {
         _view.ScalingVisible = false;
      }

      public override void Edit(IEnumerable<IParameter> parameters)
      {
         base.Edit(parameters.Where(isUserDefinedParameter));
         //this needs to be done after the Edit to override whatever settings was updated in the Edit method
         _view.ParameterNameVisible = true;
         _view.DistributionVisible = false;
      }

      private bool isUserDefinedParameter(IParameter parameter)
      {
         return !parameter.IsDefault;
      }
   }
}