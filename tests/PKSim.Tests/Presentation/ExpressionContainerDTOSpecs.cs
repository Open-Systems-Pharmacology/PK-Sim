using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExpressionContainerDTO : ContextSpecification<ExpressionContainerDTO>
   {
      protected ParameterDTO _normParameter;

      protected override void Context()
      {
         _normParameter = new ParameterDTO(DomainHelperForSpecs.ConstantParameterWithValue(0.10).WithDimension(DomainHelperForSpecs.FractionDimensionForSpecs()));
         sut = new ExpressionContainerDTO {RelativeExpressionNormParameter = _normParameter};
      }
   }

   public class When_resolving_the_value_of_the_relative_expression_norm_parameter_when_the_display_unit_is_percent : concern_for_ExpressionContainerDTO
   {
      protected override void Context()
      {
         base.Context();
         _normParameter.DisplayUnit = _normParameter.Dimension.Unit("%");
      }

      [Observation]
      public void should_return_the_kernel_value_multiplied_by_100()
      {
         sut.RelativeExpressionNorm.ShouldBeEqualTo(10);
      }
   }

   public class When_resolving_the_value_of_the_relative_expression_norm_parameter_when_the_display_unit_is_fraction : concern_for_ExpressionContainerDTO
   {
      protected override void Context()
      {
         base.Context();
         _normParameter.DisplayUnit = _normParameter.Dimension.Unit("");
      }

      [Observation]
      public void should_return_the_kernel_value_multiplied_by_100()
      {
         sut.RelativeExpressionNorm.ShouldBeEqualTo(10);
      }
   }
}