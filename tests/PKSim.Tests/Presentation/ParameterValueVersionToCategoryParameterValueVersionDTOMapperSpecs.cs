using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;

namespace PKSim.Presentation
{
   public abstract class concern_for_parameter_value_version_to_category_parameter_value_version_mapper : ContextSpecification<IParameterValueVersionToCategoryParameterValueVersionDTOMapper>
   {
      protected IRepresentationInfoRepository _representationInfoRepository;

      protected override void Context()
      {
         _representationInfoRepository =A.Fake<IRepresentationInfoRepository>();
         sut = new ParameterValueVersionToCategoryParameterValueVersionDTOMapper(_representationInfoRepository);
      }
   }

   
   public class When_mapping_a_parameter_value_version_to_a_category_parameter_value_version_dto : concern_for_parameter_value_version_to_category_parameter_value_version_mapper
   {
      private ParameterValueVersion _pvv;
      private CategoryParameterValueVersionDTO _dto;
      private RepresentationInfo _categoryRepInfo;

      protected override void Context()
      {
         base.Context();
         _pvv =new ParameterValueVersion();
         _categoryRepInfo = new RepresentationInfo {Description = "CatDesc", DisplayName = "CatDisplay"};
         _pvv.Category = "tralala";
         A.CallTo(() => _representationInfoRepository.InfoFor(RepresentationObjectType.CATEGORY, _pvv.Category)).Returns(_categoryRepInfo);
      }
      protected override void Because()
      {
         _dto = sut.MapFrom(_pvv);
      }

      [Observation]
      public void should_return_a_dto_whose_description_was_set_to_the_one_defined_in_the_representation()
      {
         _dto.Description.ShouldBeEqualTo(_categoryRepInfo.Description);
      }

      [Observation]
      public void should_return_a_dto_whose_display_name_was_set_to_the_one_defined_in_the_representation()
      {
         _dto.DisplayName.ShouldBeEqualTo(_categoryRepInfo.DisplayName);
      }

      [Observation]
      public void should_return_a_dto_whose_category_was_set_to_the_parameter_value_version_category()
      {
         _dto.Category.ShouldBeEqualTo(_pvv.Category);
      }

      [Observation]
      public void should_return_a_dto_whose_parameter_value_version_was_set_to_the_mapped_parameter_value_version()
      {
         _dto.ParameterValueVersion.ShouldBeEqualTo(_pvv);
      }

   }
}	