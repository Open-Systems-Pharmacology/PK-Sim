using System.Collections.Generic;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IBloodFlowRatesParametersPresenter : IMultiParameterEditPresenter
   {
   }

   public class BloodFlowRatesParametersPresenter : MultiParameterEditPresenter, IBloodFlowRatesParametersPresenter
   {
      public BloodFlowRatesParametersPresenter(IMultiParameterEditView view, IScaleParametersPresenter scaleParametersPresenter,
                                               IEditParameterPresenterTask editParameterPresenterTask, IParameterTask parameterTask,
                                               IParameterToParameterDTOMapper parameterDTOMapper, IParameterContextMenuFactory contextMenuFactory) : base(view, scaleParametersPresenter, editParameterPresenterTask, parameterTask, parameterDTOMapper, contextMenuFactory)
      {
      }

      public override void Edit(IEnumerable<IParameter> parameters)
      {
         base.Edit(parameters);
         _view.GroupByCategory(); //add grouping by organ type
         _view.GroupBy(pathElement: PathElementId.Container, groupIndex: 1, useCustomSort: true); //add grouping by organ name
         _view.GroupingVisible = false;
      }
   }
}