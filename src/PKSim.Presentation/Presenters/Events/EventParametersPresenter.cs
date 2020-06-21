using System.Collections.Generic;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation.Presenters.Events
{
   public interface IEventParametersPresenter : IMultiParameterEditPresenter
   {
   }

   public class EventParametersPresenter : MultiParameterEditPresenter, IEventParametersPresenter
   {
      public EventParametersPresenter(IMultiParameterEditView view, IScaleParametersPresenter scaleParametersPresenter,
         IEditParameterPresenterTask editParameterPresenterTask, IParameterTask parameterTask,
         IParameterToParameterDTOMapper parameterDTOMapper, IParameterContextMenuFactory contextMenuFactory)
         : base(view, scaleParametersPresenter, editParameterPresenterTask, parameterTask, parameterDTOMapper, contextMenuFactory)
      {
         view.UseAdvancedSortingMode = true;
      }

      public override void Edit(IEnumerable<IParameter> parameters)
      {
         base.Edit(parameters);
         _view.ParameterNameVisible = true;
         _view.SetCaption(PathElementId.Container, caption: PKSimConstants.UI.Events);
      }
   }
}