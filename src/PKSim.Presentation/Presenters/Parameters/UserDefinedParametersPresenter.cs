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
      private List<IParameter> _allParameters;
      public override bool AlwaysRefresh { get; } = true;

      public UserDefinedParametersPresenter(IMultiParameterEditView view, IScaleParametersPresenter scaleParametersPresenter, IEditParameterPresenterTask editParameterPresenterTask, IParameterTask parameterTask, IParameterToParameterDTOMapper parameterDTOMapper, IParameterContextMenuFactory contextMenuFactory) : base(view, scaleParametersPresenter, editParameterPresenterTask, parameterTask, parameterDTOMapper, contextMenuFactory)
      {
      }

      public override void Edit(IEnumerable<IParameter> parameters)
      {
         _allParameters = parameters.ToList();
         updateParameters();
      }

      private void updateParameters()
      {
         base.Edit(_allParameters.Where(isUserDefinedParameter));
         _view.ParameterNameVisible = true;
         _view.DistributionVisible = false;
      }

      private bool isUserDefinedParameter(IParameter parameter)
      {
         return !parameter.ValueOrigin.Default && parameter.ValueOrigin.Source != ValueOriginSources.Undefined;
      }
   }
}