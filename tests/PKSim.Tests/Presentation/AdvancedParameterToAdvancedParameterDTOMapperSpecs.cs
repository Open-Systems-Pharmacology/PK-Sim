using PKSim.Presentation.DTO.Mappers;
using OSPSuite.BDDHelper;


namespace PKSim.Presentation
{
   public abstract class concern_for_AdvancedParameterToAdvancedParameterDTOMapper : ContextSpecification<IAdvancedParameterToAdvancedParameterDTOMapper>
   {
      protected override void Context()
      {
         sut = new AdvancedParameterToAdvancedParameterDTOMapper();
      }
   }

   
   public class When_mapping_an_advanced_parameter_to_an_advanced_parameter_dto : concern_for_AdvancedParameterToAdvancedParameterDTOMapper
   {
      [Observation]
      public void should_return_an_advanced_parameter_dto_with_the_distribution_type_matching_the_advanced_parameter()
      {
         
      }
   }
}	