using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Applications;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.Presenters.Applications
{
   public interface IApplicationParametersPresenter : ICustomParametersPresenter, IEditParameterPresenter
   {
      void SetFavorite(IParameterDTO parameterDTO, bool isFavorite);
   }

   public class ApplicationParametersPresenter : EditParameterPresenter<IApplicationParametersView, IApplicationParametersPresenter>, IApplicationParametersPresenter
   {
      private readonly IContainerToApplicationDTOMapper _applicationDTOMapper;
      public string Description { get; set; }
      public bool ForcesDisplay => false;
      public IEnumerable<IParameter> EditedParameters => Enumerable.Empty<IParameter>();

      public ApplicationParametersPresenter(IApplicationParametersView view, IContainerToApplicationDTOMapper applicationDTOMapper,
         IEditParameterPresenterTask editParameterPresenterTask)
         : base(view, editParameterPresenterTask)
      {
         _applicationDTOMapper = applicationDTOMapper;
      }

      public void Edit(IEnumerable<IParameter> parameters)
      {
         var schemaItemContainers = parameters.Where(p => p.Visible)
            .GroupBy(p => p.ParentContainer)
            .Select(x => x.Key);

         _view.BindTo(schemaItemContainers.MapAllUsing(_applicationDTOMapper).OrderBy(x => x.StartTime));
      }
   }
}