using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Domain.Services;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_ParameterToMolWeightParameterDTOMapper : ContextSpecification<ParameterToMolWeightParameterDTOMapper>
   {
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected IFormulaToFormulaTypeMapper _formulaTypeMapper;
      protected IPathToPathElementsMapper _pathToPathElementsMapper;
      protected IEntityPathResolver _entityPathResolver;
      protected IFavoriteRepository _favoriteRepository;
      protected IParameterListOfValuesRetriever _parameterListOfValuesRetriever;

      protected override void Context()
      {
         _formulaTypeMapper = A.Fake<IFormulaToFormulaTypeMapper>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _favoriteRepository = A.Fake<IFavoriteRepository>();
         _pathToPathElementsMapper = A.Fake<IPathToPathElementsMapper>();
         _parameterListOfValuesRetriever = A.Fake<IParameterListOfValuesRetriever>();

         sut = new ParameterToMolWeightParameterDTOMapper(
            _representationInfoRepository,
            _formulaTypeMapper,
            _pathToPathElementsMapper,
            _favoriteRepository,
            _entityPathResolver,
            _parameterListOfValuesRetriever);
      }
   }

   public class When_mapping_a_molweight_parameter_dto_from_a_parameter : concern_for_ParameterToMolWeightParameterDTOMapper
   {
      private MolWeightParameterDTO _result;
      private IParameter _molWeightParameter;
      private IParameter _effectiveMolWeightParameter;
      private RepresentationInfo _repInfoParameter;
      private FormulaType _formulaType;
      private RepresentationInfo _path0;
      private RepresentationInfo _path1;

      protected override void Context()
      {
         base.Context();

         _molWeightParameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _effectiveMolWeightParameter = DomainHelperForSpecs.ConstantParameterWithValue(100);

         _formulaType = FormulaType.Rate;
         A.CallTo(() => _formulaTypeMapper.MapFrom(_molWeightParameter.Formula)).Returns(_formulaType);

         _repInfoParameter = new RepresentationInfo { DisplayName = "display para", Description = "desc para" };
         _path0 = new RepresentationInfo { DisplayName = "Organism" };
         _path1 = new RepresentationInfo { DisplayName = "Organ" };

         A.CallTo(() => _representationInfoRepository.InfoFor(_molWeightParameter)).Returns(_repInfoParameter);

         var cache = new PathElements();
         cache.Add(PathElementId.TopContainer, new PathElement { DisplayName = _path0.DisplayName });
         cache.Add(PathElementId.Container, new PathElement { DisplayName = _path1.DisplayName });
         A.CallTo(() => _pathToPathElementsMapper.MapFrom(_molWeightParameter)).Returns(cache);

         var parameterPath = new ObjectPath();
         A.CallTo(() => _entityPathResolver.ObjectPathFor(_molWeightParameter, false)).Returns(parameterPath);
         A.CallTo(() => _favoriteRepository.Contains(parameterPath)).Returns(true);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_molWeightParameter, _effectiveMolWeightParameter);
      }

      [Observation]
      public void should_return_a_MolWeightParameterDTO()
      {
         _result.ShouldBeAnInstanceOf<MolWeightParameterDTO>();
      }

      [Observation]
      public void should_set_the_name_to_parameter_name()
      {
         _result.Name.ShouldBeEqualTo(_molWeightParameter.Name);
      }

      [Observation]
      public void should_set_the_display_name_to_parameter_display_name()
      {
         _result.DisplayName.ShouldBeEqualTo(_repInfoParameter.DisplayName);
      }

      [Observation]
      public void should_set_the_description_to_parameter_description()
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
      public void should_set_the_display_unit_to_the_display_unit_of_the_parameter()
      {
         _result.DisplayUnit.ShouldBeEqualTo(_molWeightParameter.DisplayUnit);
      }

      [Observation]
      public void the_value_returned_by_the_dto_should_be_the_value_in_the_display_unit()
      {
         _result.Value.ShouldBeEqualTo(_molWeightParameter.ValueInDisplayUnit);
      }

      [Observation]
      public void the_returned_units_should_be_the_units_defined_for_the_parameter()
      {
         _result.AllUnits.ShouldOnlyContain(_molWeightParameter.Dimension.Units);
      }

      [Observation]
      public void should_set_the_parameter_to_the_mapped_parameter()
      {
         _result.Parameter.ShouldBeEqualTo(_molWeightParameter);
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
}