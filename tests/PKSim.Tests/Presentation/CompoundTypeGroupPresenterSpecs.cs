using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;

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
      protected CompoundTypeDTO _compoundTypeDTO;

      protected override void Context()
      {
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _view = A.Fake<ICompoundTypeGroupView>();
         _representationInfoRep = A.Fake<IRepresentationInfoRepository>();
         _mapper = A.Fake<ICompoundToCompoundTypeDTOMapper>();
         _parameterTask = A.Fake<IParameterTask>();

         sut = new CompoundTypeGroupPresenter(_view,_representationInfoRep,_mapper,_parameterTask,_entityPathResolver);

         _parameters = new List<IParameter>();
         _compoundTypeDTO =new CompoundTypeDTO();
         A.CallTo(() => _mapper.MapFrom(_parameters)).Returns(_compoundTypeDTO);

      }
   }

   public class When_notify_that_a_parameter_that_is_not_a_compound_parameter_is_removed_from_favorite    : concern_for_CompoundTypeGroupPresenter
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
         _compoundTypeDTO.AddTypePKa(new TypePKaDTO{PKaParameter = new ParameterDTO(_parameter)});
      }

      [Observation]
      public void should_set_the_favorite_flag_to_false()
      {
         sut.Handle(new RemoveParameterFromFavoritesEvent("EXISTS"));
      }
   }
}	