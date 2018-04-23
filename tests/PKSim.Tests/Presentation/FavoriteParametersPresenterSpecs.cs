using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation
{
   public abstract class concern_for_FavoriteParametersPresenter : ContextSpecification<IFavoriteParametersPresenter>
   {
      protected IFavoriteParametersView _view;
      protected IParameterTask _parameterTask;
      protected IFavoriteRepository _favoriteRepository;
      protected IParameter _par1, _par2, _par3;
      protected List<IParameter> _parameters;
      protected IList<string> _favorites;
      protected IMultiParameterEditPresenter _multiParameterEditPresenter;
      protected IEnumerable<IParameter> _allEditedParameters;
      protected IFavoriteTask _favoriteTask;

      protected override void Context()
      {
         _view = A.Fake<IFavoriteParametersView>();
         _parameterTask = A.Fake<IParameterTask>();
         _favoriteRepository = A.Fake<IFavoriteRepository>();
         _multiParameterEditPresenter = A.Fake<IMultiParameterEditPresenter>();
         _favoriteTask = A.Fake<IFavoriteTask>();

         sut = new FavoriteParametersPresenter(_view, _multiParameterEditPresenter, _parameterTask, _favoriteRepository, _favoriteTask);

         _par1 = new PKSimParameter().WithName("par1");
         _par2 = new PKSimParameter().WithName("par2");
         _par3 = new PKSimParameter().WithName("par3");
         _parameters = new List<IParameter> {_par1, _par2, _par3};
         var pathCache = new PathCache<IParameter>(A.Fake<IEntityPathResolver>()) {{"par1", _par1}, {"par2", _par2}, {"par3", _par3}};
         A.CallTo(() => _parameterTask.PathCacheFor(_parameters)).Returns(pathCache);
         _favorites = new List<string>();
         A.CallTo(() => _favoriteRepository.All()).Returns(_favorites);

         A.CallTo(() => _multiParameterEditPresenter.Edit(A<IEnumerable<IParameter>>._))
            .Invokes(x => _allEditedParameters = x.GetArgument<IEnumerable<IParameter>>(0));
      }

      protected string PathFor(string paramName)
      {
         return new[] {paramName}.ToPathString();
      }
   }

   public class When_the_favorite_parameters_presenter_is_editing_a_set_of_parameters : concern_for_FavoriteParametersPresenter
   {
      protected override void Context()
      {
         base.Context();
         _favorites.Add(PathFor("par1"));
      }

      protected override void Because()
      {
         sut.Edit(_parameters);
      }

      [Observation]
      public void should_retrieve_the_parameters_defined_as_favorites_and_display_them()
      {
         _allEditedParameters.ShouldOnlyContain(_par1);
      }
   }

   public class When_notify_that_a_parameter_was_removed_from_the_favorites : concern_for_FavoriteParametersPresenter
   {
      protected override void Context()
      {
         base.Context();
         _favorites.Add(PathFor("par1"));
         sut.Edit(_parameters);
      }

      protected override void Because()
      {
         _favorites.Add(PathFor("par2"));
         sut.Handle(new AddParameterToFavoritesEvent(PathFor("par2")));
      }

      [Observation]
      public void should_update_the_view_with_the_available_favorites()
      {
         _allEditedParameters.ShouldOnlyContain(_par1, _par2);
      }
   }

   public class When_retrieving_the_list_of_favorite_parameters : concern_for_FavoriteParametersPresenter
   {
      protected override void Context()
      {
         base.Context();
         _favorites.Add(PathFor("par1"));
         sut.Edit(_parameters);
         sut.Handle(new FavoritesLoadedEvent());
      }

      [Observation]
      public void should_return_only_favorite_parameters()
      {
         sut.EditedParameters.ShouldOnlyContain(_par1);
      }
   }

   public class When_notify_that_the_favorites_where_loaded : concern_for_FavoriteParametersPresenter
   {
      protected override void Context()
      {
         base.Context();
         _favorites.Add(PathFor("par1"));
         sut.Edit(_parameters);
      }

      protected override void Because()
      {
         sut.Handle(new FavoritesLoadedEvent());
      }

      [Observation]
      public void should_update_the_view_with_the_available_favorites()
      {
         _allEditedParameters.ShouldOnlyContain(_par1);
      }
   }

   public class When_notify_that_a_parameter_was_added_to_the_favorites : concern_for_FavoriteParametersPresenter
   {
      protected override void Context()
      {
         base.Context();
         _favorites.Add(PathFor("par1"));
         _favorites.Add(PathFor("par2"));
         sut.Edit(_parameters);
      }

      protected override void Because()
      {
         _favorites.Remove(PathFor("par2"));
         sut.Handle(new RemoveParameterFromFavoritesEvent(PathFor("par2")));
      }

      [Observation]
      public void should_update_the_view_with_the_available_favorites()
      {
         _allEditedParameters.ShouldOnlyContain(_par1);
      }
   }

   public class When_the_favorite_parameter_presenter_is_being_notified_that_some_parameters_are_selected : concern_for_FavoriteParametersPresenter
   {
      private List<IParameter> _selectedParameters;

      protected override void Context()
      {
         base.Context();
         _selectedParameters = new List<IParameter> {_par2};
         A.CallTo(() => _multiParameterEditPresenter.SelectedParameters).Returns(_selectedParameters);
      }

      protected override void Because()
      {
         _multiParameterEditPresenter.OnSelectedParametersChanged += Raise.FreeForm.With();
      }

      [Observation]
      public void should_enable_the_move_up_and_down_button()
      {
         _view.UpEnabled.ShouldBeTrue();
         _view.DownEnabled.ShouldBeTrue();
      }
   }

   public class When_the_favorite_parameter_presenter_is_being_notified_that_some_parameters_are_selected_and_the_selection_contains_no_parameter : concern_for_FavoriteParametersPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _multiParameterEditPresenter.SelectedParameters).Returns(new List<IParameter>());
      }

      protected override void Because()
      {
         _multiParameterEditPresenter.OnSelectedParametersChanged += Raise.FreeForm.With();
      }

      [Observation]
      public void should_disable_the_move_up_and_down_button()
      {
         _view.UpEnabled.ShouldBeFalse();
         _view.DownEnabled.ShouldBeFalse();
      }
   }

   public class When_the_favorite_parameters_is_moving_selected_parameters_up : concern_for_FavoriteParametersPresenter
   {
      private IParameter _parameter1;
      private IParameter _parameter2;
      private IEnumerable<string> _movedParameters;
      private IParameter[] _selectedParameters;

      protected override void Context()
      {
         base.Context();
         _parameter1 = A.Fake<IParameter>();
         _parameter2 = A.Fake<IParameter>();
         A.CallTo(() => _parameterTask.PathFor(_parameter1)).Returns("PATH1");
         A.CallTo(() => _parameterTask.PathFor(_parameter2)).Returns("PATH2");

         _selectedParameters = new[] {_parameter1, _parameter2};
         A.CallTo(() => _multiParameterEditPresenter.SelectedParameters).Returns(_selectedParameters);

         A.CallTo(() => _favoriteTask.MoveUp(A<IEnumerable<string>>._))
            .Invokes(x => _movedParameters = x.GetArgument<IEnumerable<string>>(0));
      }

      protected override void Because()
      {
         sut.MoveUp();
      }

      [Observation]
      public void should_retrieve_the_selected_parameters_from_the_view_and_move_them_up()
      {
         _movedParameters.ShouldOnlyContainInOrder("PATH1", "PATH2");
      }

      [Observation]
      public void should_update_the_parameter_selection()
      {
         _multiParameterEditPresenter.SelectedParameters.ShouldBeEqualTo(_selectedParameters);
      }
   }

   public class When_the_favorite_parameters_presenter_is_notified_that_the_order_of_favorites_was_updated : concern_for_FavoriteParametersPresenter
   {
      protected override void Because()
      {
         sut.Handle(new FavoritesOrderChangedEvent());
      }

      [Observation]
      public void should_refresh_the_edited_parameters()
      {
         A.CallTo(() => _multiParameterEditPresenter.Edit(A<IEnumerable<IParameter>>._)).MustHaveHappened();
      }
   }
}