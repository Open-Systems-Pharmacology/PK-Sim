using System.Collections.Generic;
using PKSim.Assets;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IEditSolverSettingsPresenter : IMultiParameterEditPresenter
   {
   }

   public class EditSolverSettingsPresenter : MultiParameterEditPresenter, IEditSolverSettingsPresenter
   {
      public EditSolverSettingsPresenter(IMultiParameterEditView view, IScaleParametersPresenter scaleParametersPresenter, IEditParameterPresenterTask editParameterPresenterTask,
         IParameterTask parameterTask, IParameterToParameterDTOMapper parameterDTOMapper, IParameterContextMenuFactory contextMenuFactory) : base(view, scaleParametersPresenter, editParameterPresenterTask, parameterTask, parameterDTOMapper, contextMenuFactory)
      {
         view.ApplicationIcon = ApplicationIcons.Solver;
         view.Caption = PKSimConstants.UI.Solver;
      }

      public override void Edit(IEnumerable<IParameter> parameters)
      {
         base.Edit(parameters);
         _view.GroupingVisible = false;
         _view.ScalingVisible = false;
      }
   }
}