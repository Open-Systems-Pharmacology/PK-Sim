using System.Collections.Generic;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.Presenters.Formulations
{
   public interface IFormulationParametersPresenter : IMultiParameterEditPresenter
   {
   }

   public class FormulationParametersPresenter : MultiParameterEditPresenter, IFormulationParametersPresenter
   {
      public FormulationParametersPresenter(IMultiParameterEditView view, IScaleParametersPresenter scaleParametersPresenter,
         IEditParameterPresenterTask editParameterPresenterTask, IParameterTask parameterTask,
         IParameterToParameterDTOMapper parameterDTOMapper, IParameterContextMenuFactory contextMenuFactory) : base(view, scaleParametersPresenter, editParameterPresenterTask, parameterTask, parameterDTOMapper, contextMenuFactory)
      {
         view.UseAdvancedSortingMode = true;
      }

      public override void Edit(IEnumerable<IParameter> parameters)
      {
         base.Edit(parameters);
         _view.ParameterNameVisible = true;
         _view.SetVisibility(PathElement.Container, visible: true);
         _view.GroupBy(PathElement.Container);
         _view.GroupingVisible = false;
      }
   }
}