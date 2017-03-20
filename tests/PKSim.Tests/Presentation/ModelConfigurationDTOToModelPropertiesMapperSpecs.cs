using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_ModelConfigurationDTOToModelPropertiesMapper : ContextSpecification<IModelConfigurationDTOToModelPropertiesMapper>
   {
      protected override void Context()
      {
         sut = new ModelConfigurationDTOToModelPropertiesMapper();
      }
   }

   public class When_mapping_a_model_configuration_dto_to_a_model_properties : concern_for_ModelConfigurationDTOToModelPropertiesMapper
   {
      private ModelConfigurationDTO _modelConfigurationDTO;
      private CalculationMethod _cm1;
      private CalculationMethod _cm2;
      private ModelProperties _result;

      protected override void Context()
      {
         base.Context();
         _modelConfigurationDTO = new ModelConfigurationDTO();
         _modelConfigurationDTO.ModelConfiguration = A.Fake<ModelConfiguration>();
         _cm1 = new CalculationMethod().WithName("cm1");
         _cm2 = new CalculationMethod().WithName("cm2");
         _modelConfigurationDTO.CalculationMethodDTOs = new List<CategoryCalculationMethodDTO>
         {
            new CategoryCalculationMethodDTO {CalculationMethod = _cm1},
            new CategoryCalculationMethodDTO {CalculationMethod = _cm2}
         };
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_modelConfigurationDTO);
      }

      [Observation]
      public void should_return_a_model_properties_configured_with_the_selected_model()
      {
         _result.ModelConfiguration.ShouldBeEqualTo(_modelConfigurationDTO.ModelConfiguration);
      }

      [Observation]
      public void should_return_a_model_properties_configured_with_the_selected_calculation_methods()
      {
         _result.AllCalculationMethods().ShouldOnlyContain(_cm1, _cm2);
      }
   }
}