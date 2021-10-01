using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.PopulationAnalyses;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper : ContextSpecification<IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper>
   {
      protected IDimensionRepository _dimensionRepository;

      protected override void Context()
      {
         _dimensionRepository = A.Fake<IDimensionRepository>();
         sut = new PopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper(_dimensionRepository);
      }
   }

   public class When_mapping_a_population_field_with_dimension_to_a_population_field_dto : concern_for_PopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper
   {
      private PopulationAnalysisNumericField _field;
      private PopulationAnalysisFieldDTO _dto;
      private IDimension _mergedDimension;

      protected override void Context()
      {
         base.Context();
         _mergedDimension = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         _field = A.Fake<PopulationAnalysisNumericField>();
         _field.Dimension = DomainHelperForSpecs.FractionDimensionForSpecs();
         A.CallTo((_dimensionRepository)).WithReturnType<IDimension>().Returns(_mergedDimension);
      }

      protected override void Because()
      {
         _dto = sut.MapFrom(_field);
      }

      [Observation]
      public void should_return_a_population_field_dto_referencing_the_underlying_field()
      {
         _dto.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_updated_the_dimension_to_be_a_merged_dimension()
      {
         _dto.Dimension.ShouldBeEqualTo(_mergedDimension);
      }
   }
}