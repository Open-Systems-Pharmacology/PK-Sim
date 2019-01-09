using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundTypeGroupPresenter : ContextSpecification<ICompoundTypeGroupPresenter>
   {
      protected IEntityPathResolver _entityPathResolver;
      private ICompoundTypeGroupView _view;
      private IRepresentationInfoRepository _representationInfoRep;
      protected ICompoundToCompoundTypeDTOMapper _mapper;
      private IParameterTask _parameterTask;
      protected List<IParameter> _parameters;
      private IEditValueOriginPresenter _editValueOriginPresenter;
      protected List<TypePKaDTO> _compoundTypesDTOs;

      protected override void Context()
      {
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _view = A.Fake<ICompoundTypeGroupView>();
         _representationInfoRep = A.Fake<IRepresentationInfoRepository>();
         _mapper = A.Fake<ICompoundToCompoundTypeDTOMapper>();
         _parameterTask = A.Fake<IParameterTask>();
         _editValueOriginPresenter= A.Fake<IEditValueOriginPresenter>();

         sut = new CompoundTypeGroupPresenter(_view, _representationInfoRep, _mapper, _parameterTask, _entityPathResolver, _editValueOriginPresenter);

         _parameters = new List<IParameter>();
         _compoundTypesDTOs = new List<TypePKaDTO>();
         A.CallTo(() => _mapper.MapFrom(_parameters)).Returns(_compoundTypesDTOs);
      }
   }

   public class When_notify_that_a_parameter_that_is_not_a_compound_parameter_is_removed_from_favorite : concern_for_CompoundTypeGroupPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.EditCompoundParameters(_parameters);
      }

      [Observation]
      public void should_do_nothing()
      {
         sut.Handle(new RemoveParameterFromFavoritesEvent("DOES NOT EXIST"));
      }
   }

   public class When_notify_that_a_parameter_that_is_a_compound_parameter_but_not_a_pka_parameter_is_removed_from_favorite : concern_for_CompoundTypeGroupPresenter
   {
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = new PKSimParameter();
         A.CallTo(() => _entityPathResolver.PathFor(_parameter)).Returns("EXISTS");
         _parameters.Add(_parameter);
         sut.EditCompoundParameters(_parameters);
      }

      [Observation]
      public void should_do_nothing()
      {
         sut.Handle(new RemoveParameterFromFavoritesEvent("EXISTS"));
      }
   }

   public class When_notify_that_a_parameter_that_is_a_compound_parametera_pka_parameter_is_removed_from_favorite : concern_for_CompoundTypeGroupPresenter
   {
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = new PKSimParameter();
         A.CallTo(() => _entityPathResolver.PathFor(_parameter)).Returns("EXISTS");
         _parameters.Add(_parameter);
         sut.EditCompoundParameters(_parameters);
         _compoundTypesDTOs.Add(new TypePKaDTO {PKaParameter = new ParameterDTO(_parameter)});
      }

      [Observation]
      public void should_set_the_favorite_flag_to_false()
      {
         sut.Handle(new RemoveParameterFromFavoritesEvent("EXISTS"));
      }
   }
}