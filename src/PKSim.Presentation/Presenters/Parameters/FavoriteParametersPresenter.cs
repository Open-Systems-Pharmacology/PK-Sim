using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Events;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Core.Services;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface IFavoriteParametersPresenter : ICustomParametersPresenter,
      IListener<AddParameterToFavoritesEvent>,
      IListener<RemoveParameterFromFavoritesEvent>,
      IListener<FavoritesLoadedEvent>,
      IListener<FavoritesOrderChangedEvent>

   {
      /// <summary>
      ///    Moves the selected parameter up once
      /// </summary>
      void MoveUp();

      /// <summary>
      ///    Moves the selected parameter down once
      /// </summary>
      void MoveDown();
   }

   public class FavoriteParametersPresenter : AbstractCommandCollectorPresenter<IFavoriteParametersView, IFavoriteParametersPresenter>, IFavoriteParametersPresenter
   {
      private readonly IMultiParameterEditPresenter _multiParameterEditPresenter;
      private readonly IParameterTask _parameterTask;
      private readonly IFavoriteRepository _favoriteRepository;
      private readonly IFavoriteTask _favoriteTask;
      private PathCache<IParameter> _allParameterCache;
      private readonly List<IParameter> _allFavorites = new List<IParameter>();

      public string Description { get; set; } = string.Empty;
      public bool ForcesDisplay { get; } = false;
      public bool AlwaysRefresh { get; } = false;

      public FavoriteParametersPresenter(
         IFavoriteParametersView view,
         IMultiParameterEditPresenter multiParameterEditPresenter,
         IParameterTask parameterTask,
         IFavoriteRepository favoriteRepository,
         IFavoriteTask favoriteTask) : base(view)
      {
         _multiParameterEditPresenter = multiParameterEditPresenter;
         _parameterTask = parameterTask;
         _favoriteRepository = favoriteRepository;
         _favoriteTask = favoriteTask;
         _allParameterCache = _parameterTask.PathCacheFor(Enumerable.Empty<IParameter>());
         _view.AddParametersView(_multiParameterEditPresenter.BaseView);
         AddSubPresenters(_multiParameterEditPresenter);
         _multiParameterEditPresenter.ScalingVisible = false;
         _multiParameterEditPresenter.OnSelectedParametersChanged += onSelectedParametersChanged;
      }

      private void onSelectedParametersChanged()
      {
         updateMoveButtonsEnableState(_multiParameterEditPresenter.SelectedParameters.Any());
      }

      public void Edit(IEnumerable<IParameter> parameters)
      {
         _allParameterCache = _parameterTask.PathCacheFor(parameters);
         updateParameters();
      }

      public IEnumerable<IParameter> EditedParameters => _allFavorites;

      private IEnumerable<IParameter> allFavoriteParameters()
      {
         return from favorite in _favoriteRepository.All()
            where _allParameterCache.Contains(favorite)
            select _allParameterCache[favorite];
      }

      private void updateParameters()
      {
         _allFavorites.Clear();
         _allFavorites.AddRange(allFavoriteParameters());
         _multiParameterEditPresenter.Edit(_allFavorites);

         //this needs to be done after the Edit to override whatever settings was updated in the Edit method
         _multiParameterEditPresenter.DistributionVisible = false;
         _multiParameterEditPresenter.ParameterNameVisible = true;

         updateMoveButtonsEnableState(_allFavorites.Any());
      }

      private void updateMoveButtonsEnableState(bool enable)
      {
         _view.DownEnabled = enable;
         _view.UpEnabled = enable;
      }

      public void Handle(AddParameterToFavoritesEvent eventToHandle) => updateParameters();

      public void Handle(RemoveParameterFromFavoritesEvent eventToHandle) => updateParameters();

      public void Handle(FavoritesLoadedEvent eventToHandle) => updateParameters();

      public void Handle(FavoritesOrderChangedEvent eventToHandle) => updateParameters();

      public void MoveUp() => moveFavorites(x => x.MoveUp);

      public void MoveDown() => moveFavorites(x => x.MoveDown);

      private void moveFavorites(Func<IFavoriteTask, Action<IEnumerable<string>>> moveActionFunc)
      {
         var selectedParameters = _multiParameterEditPresenter.SelectedParameters;
         var moveAction = moveActionFunc(_favoriteTask);
         moveAction(selectedParameters.Select(_parameterTask.PathFor));
         _multiParameterEditPresenter.SelectedParameters = selectedParameters;
      }
   }
}