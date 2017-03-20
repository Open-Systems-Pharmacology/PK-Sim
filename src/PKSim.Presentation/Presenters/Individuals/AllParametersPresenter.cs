using System.Collections.Generic;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IAllParametersPresenter : IMultiParameterEditPresenter
   {
   }

   public class AllParametersPresenter : MultiParameterEditPresenter, IAllParametersPresenter
   {
      public AllParametersPresenter(IMultiParameterEditView view, IScaleParametersPresenter scaleParametersPresenter, IEditParameterPresenterTask editParameterPresenterTask, IParameterTask parameterTask, IParameterToParameterDTOMapper parameterDTOMapper, IParameterContextMenuFactory contextMenuFactory) : base(view, scaleParametersPresenter, editParameterPresenterTask, parameterTask, parameterDTOMapper, contextMenuFactory)
      {
      }

      public override void Edit(IEnumerable<IParameter> parameters)
      {
         base.Edit(parameters);

         //all view=>should disable sort by name
         _view.CustomSortEnabled = false;
      }
   }
}