using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Events;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Events;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface IFavoriteParametersPresenter : ICustomParametersPresenter,
      IListener<AddParameterToFavoritesEvent>,
      IListener<RemoveParameterFromFavoritesEvent>,
      IListener<FavoritesLoadedEvent>

   {
   }

   public class FavoriteParametersPresenter : MultiParameterEditPresenter, IFavoriteParametersPresenter
   {
      private readonly IFavoriteRepository _favoriteRepository;
      private PathCache<IParameter> _allParameterCache;

      public FavoriteParametersPresenter(IMultiParameterEditView view, IScaleParametersPresenter scaleParametersPresenter, IEditParameterPresenterTask editParameterPresenterTask, IParameterTask parameterTask,
         IParameterToParameterDTOMapper parameterDTOMapper, IParameterContextMenuFactory contextMenuFactory, IFavoriteRepository favoriteRepository) :
            base(view, scaleParametersPresenter, editParameterPresenterTask, parameterTask, parameterDTOMapper, contextMenuFactory)
      {
         _favoriteRepository = favoriteRepository;
         _allParameterCache = _parameterTask.PathCacheFor(Enumerable.Empty<IParameter>());
      }

      public override void Edit(IEnumerable<IParameter> parameters)
      {
         _allParameterCache = _parameterTask.PathCacheFor(parameters);
         updateParameters();
      }

      private IEnumerable<IParameter> allFavoriteParameters()
      {
         return from favorite in _favoriteRepository.All()
            where _allParameterCache.Contains(favorite)
            select _allParameterCache[favorite];
      }

      private void updateParameters()
      {
         base.Edit(allFavoriteParameters());
         _view.ParameterNameVisible = true;
         _view.DistributionVisible = false;
      }

      public override void Handle(AddParameterToFavoritesEvent eventToHandle)
      {
         updateParameters();
      }

      public override void Handle(RemoveParameterFromFavoritesEvent eventToHandle)
      {
         updateParameters();
      }

      public void Handle(FavoritesLoadedEvent eventToHandle)
      {
         updateParameters();
      }
   }
}