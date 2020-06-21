using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.DTO;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation
{
   public abstract class concern_for_ParameterToParameterDTOMapper : ContextSpecification<IParameterToParameterDTOMapper>
   {
      protected IParameter _parameter;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected IFormulaToFormulaTypeMapper _formulaTypeMapper;
      protected IPathToPathElementsMapper _pathToPathElementsMapper;
      protected IEntityPathResolver _entityPathResolver;
      protected IFavoriteRepository _favoriteRepository;
      protected IGroupRepository _groupRepository;
      protected IParameterListOfValuesRetriever _parameterListOfValuesRetriever;

      protected override void Context()
      {
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _formulaTypeMapper = A.Fake<IFormulaToFormulaTypeMapper>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _favoriteRepository = A.Fake<IFavoriteRepository>();
         _pathToPathElementsMapper = A.Fake<IPathToPathElementsMapper>();
         _parameterListOfValuesRetriever = A.Fake<IParameterListOfValuesRetriever>();
         sut = new ParameterToParameterDTOMapper(_representationInfoRepository, _formulaTypeMapper, _pathToPathElementsMapper, _favoriteRepository, _entityPathResolver, _parameterListOfValuesRetriever);
      }
   }

   public class When_mapping_a_parameter_dto_from_a_parameter : concern_for_ParameterToParameterDTOMapper
   {
      private IParameterDTO _result;
      private IContainer _parentContainer;
      private RepresentationInfo _repInfoParameter;
      private FormulaType _formulaType;
      private RepresentationInfo _path0;
      private RepresentationInfo _path1;
      private RepresentationInfo _path2;

      protected override void Context()
      {
         base.Context();

         _formulaType = FormulaType.Rate;
         _parentContainer = new Container();
         _parentContainer.Add(_parameter);
         _parameter.Name = "_parameter";
         A.CallTo(() => _formulaTypeMapper.MapFrom(_parameter.Formula)).Returns(_formulaType);
         _repInfoParameter = new RepresentationInfo {DisplayName = "display para", Description = "desc para"};
         _path0 = new RepresentationInfo {DisplayName = "Organism"};
         _path1 = new RepresentationInfo {DisplayName = "Organ"};
         _path2 = _repInfoParameter;
         A.CallTo(() => _representationInfoRepository.InfoFor(_parameter)).Returns(_repInfoParameter);
         var cache = new PathElements();
         cache.Add(PathElementId.TopContainer, new PathElement {DisplayName = _path0.DisplayName});
         cache.Add(PathElementId.Container, new PathElement {DisplayName = _path1.DisplayName});
         A.CallTo(() => _pathToPathElementsMapper.MapFrom(_parameter)).Returns(cache);
         var parameterPath = new ObjectPath();
         A.CallTo(() => _entityPathResolver.ObjectPathFor(_parameter, false)).Returns(parameterPath);
         A.CallTo(() => _favoriteRepository.Contains(parameterPath)).Returns(true);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_parameter);
      }

      [Observation]
      public void should_set_the_name_parameter_name()
      {
         _result.Name.ShouldBeEqualTo(_parameter.Name);
      }

      [Observation]
      public void should_set_the_display_name_parameter_display_name()
      {
         _result.DisplayName.ShouldBeEqualTo(_repInfoParameter.DisplayName);
      }

      [Observation]
      public void should_set_the_description_parameter_description()
      {
         _result.Description.ShouldBeEqualTo(_repInfoParameter.Description);
      }

      [Observation]
      public void should_have_set_the_parameter_hierarchy_path_according_to_the_parameter()
      {
         _result.PathElements[PathElementId.TopContainer].DisplayName.ShouldBeEqualTo(_path0.DisplayName);
         _result.PathElements[PathElementId.Container].DisplayName.ShouldBeEqualTo(_path1.DisplayName);
      }

      [Observation]
      public void should_have_set_the_parameter_path_corresponding_to_the_compartment_display_name_to_an_empty_string()
      {
         string.IsNullOrEmpty(_result.PathElements[PathElementId.BottomCompartment].DisplayName).ShouldBeTrue();
      }

      [Observation]
      public void should_set_the_display_unit_to_the_display_unit_of_the_parameter()
      {
         _result.DisplayUnit.ShouldBeEqualTo(_parameter.DisplayUnit);
      }

      [Observation]
      public void the_value_returned_by_the_dto_should_be_the_value_in_the_display_unit()
      {
         _result.Value.ShouldBeEqualTo(_parameter.ValueInDisplayUnit);
      }

      [Observation]
      public void the_returned_units_should_be_the_units_defined_for_the_parameter()
      {
         _result.AllUnits.ShouldOnlyContain(_parameter.Dimension.Units);
      }

      [Observation]
      public void should_set_the_parameter_to_the_mapped_parameter()
      {
         _result.Parameter.ShouldBeEqualTo(_parameter);
      }

      [Observation]
      public void the_returned_parameter_dto_formula_type_should_have_been_set_according_to_the_formula()
      {
         _result.FormulaType.ShouldBeEqualTo(_formulaType);
      }

      [Observation]
      public void should_set_the_favorite_flag_to_true_if_the_parameter_path_belongs_to_the_favorite()
      {
         _result.IsFavorite.ShouldBeTrue();
      }
   }

   public class When_mapping_a_null_parameter : concern_for_ParameterToParameterDTOMapper
   {
      private IParameterDTO _result;

      protected override void Context()
      {
         base.Context();
         _parameter = null;
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_parameter);
      }

      [Observation]
      public void the_returned_parameter_should_be_a_nullable_parameter()
      {
         _result.ShouldBeAnInstanceOf<NullParameterDTO>();
      }
   }
}