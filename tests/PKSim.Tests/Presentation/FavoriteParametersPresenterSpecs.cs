using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;


using PKSim.Presentation.Services;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation
{
   public abstract class concern_for_FavoriteParametersPresenter : ContextSpecification<IFavoriteParametersPresenter>
   {
      protected IMultiParameterEditView _view;
      private IScaleParametersPresenter _scaleParameterPresenter;
      protected IParameterTask _parameterTask;
      protected IParameterToParameterDTOMapper _parameterDTOMapper;
      private IParameterContextMenuFactory _contextMenuFactory;
      protected IFavoriteRepository _favoriteRepository;
      protected IParameter _par1, _par2, _par3;
      protected List<IParameter> _parameters;
      protected ParameterDTO _par1DTO;
      protected ParameterDTO _par2DTO;
      protected IList<string> _favorites;
      protected IEditParameterPresenterTask _editParameterPresenterTask;

      protected override void Context()
      {
         _view = A.Fake<IMultiParameterEditView>();
         _scaleParameterPresenter = A.Fake<IScaleParametersPresenter>();
         _parameterTask = A.Fake<IParameterTask>();
         _contextMenuFactory = A.Fake<IParameterContextMenuFactory>();
         _favoriteRepository = A.Fake<IFavoriteRepository>();
         _parameterDTOMapper = A.Fake<IParameterToParameterDTOMapper>();
         _editParameterPresenterTask = A.Fake<IEditParameterPresenterTask>();
         sut = new FavoriteParametersPresenter(_view, _scaleParameterPresenter, _editParameterPresenterTask, _parameterTask, _parameterDTOMapper, _contextMenuFactory, _favoriteRepository);
         _par1 = new PKSimParameter().WithName("par1");
         _par2 = new PKSimParameter().WithName("par2");
         _par3 = new PKSimParameter().WithName("par3");
         _parameters = new List<IParameter> {_par1, _par2, _par3};
         var pathCache = new PathCache<IParameter>(A.Fake<IEntityPathResolver>()) {{"par1", _par1}, {"par2", _par2}, {"par3", _par3}};
         A.CallTo(() => _parameterTask.PathCacheFor(_parameters)).Returns(pathCache);
         _par1DTO = new ParameterDTO(_par1);
         _par2DTO = new ParameterDTO(_par2);
         A.CallTo(() => _parameterDTOMapper.MapFrom(_par1)).Returns(_par1DTO);
         A.CallTo(() => _parameterDTOMapper.MapFrom(_par2)).Returns(_par2DTO);
         _favorites = new List<string>();
         A.CallTo(() => _favoriteRepository.All()).Returns(_favorites);
      }

      protected string PathFor(string paramName)
      {
         return new[] {paramName}.ToPathString();
      }
   }

   public class When_the_favorite_parameters_presenter_is_editing_a_set_of_parameters : concern_for_FavoriteParametersPresenter
   {
      private IEnumerable<IParameterDTO> _parameterDTOs;

      protected override void Context()
      {
         base.Context();
         _favorites.Add(PathFor("par1"));
         A.CallTo(() => _view.BindTo(A<IEnumerable<ParameterDTO>>._))
          .Invokes(x => _parameterDTOs = x.GetArgument<IEnumerable<IParameterDTO>>(0));
      }

      protected override void Because()
      {
         sut.Edit(_parameters);
      }

      [Observation]
      public void should_retrieve_the_parameters_defined_as_favorites_and_display_them()
      {
         _parameterDTOs.ShouldOnlyContain(_par1DTO);
      }
   }

   public class When_notify_that_a_parameter_was_removed_from_the_favorites : concern_for_FavoriteParametersPresenter
   {
      private IEnumerable<IParameterDTO> _parameterDTOs;

      protected override void Context()
      {
         base.Context();
         _favorites.Add(PathFor("par1"));
         sut.Edit(_parameters);
         A.CallTo(() => _view.BindTo(A<IEnumerable<ParameterDTO>>._))
          .Invokes(x => _parameterDTOs = x.GetArgument<IEnumerable<IParameterDTO>>(0));
      }

      protected override void Because()
      {
         _favorites.Add(PathFor("par2"));
         sut.Handle(new AddParameterToFavoritesEvent(PathFor("par2")));
      }

      [Observation]
      public void should_update_the_view_with_the_available_favorites()
      {
         _parameterDTOs.ShouldOnlyContain(_par1DTO, _par2DTO);
      }
   }

   public class When_notify_that_the_favorites_where_loaded : concern_for_FavoriteParametersPresenter
   {
      private IEnumerable<IParameterDTO> _parameterDTOs;

      protected override void Context()
      {
         base.Context();
         _favorites.Add(PathFor("par1"));
         sut.Edit(_parameters);
         A.CallTo(() => _view.BindTo(A<IEnumerable<ParameterDTO>>._))
          .Invokes(x => _parameterDTOs = x.GetArgument<IEnumerable<IParameterDTO>>(0));
      }

      protected override void Because()
      {
         sut.Handle(new FavoritesLoadedEvent());
      }

      [Observation]
      public void should_update_the_view_with_the_available_favorites()
      {
         _parameterDTOs.ShouldOnlyContain(_par1DTO);
      }
   }

   public class When_notify_that_a_parameter_was_added_to_the_favorites : concern_for_FavoriteParametersPresenter
   {
      private IEnumerable<IParameterDTO> _parameterDTOs;

      protected override void Context()
      {
         base.Context();
         _favorites.Add(PathFor("par1"));
         _favorites.Add(PathFor("par2"));
         sut.Edit(_parameters);
         A.CallTo(() => _view.BindTo(A<IEnumerable<ParameterDTO>>.Ignored))
          .Invokes(x => _parameterDTOs = x.GetArgument<IEnumerable<ParameterDTO>>(0));
      }

      protected override void Because()
      {
         _favorites.Remove(PathFor("par2"));
         sut.Handle(new RemoveParameterFromFavoritesEvent(PathFor("par2")));
      }

      [Observation]
      public void should_update_the_view_with_the_available_favorites()
      {
         _parameterDTOs.ShouldOnlyContain(_par1DTO);
      }
   }
}