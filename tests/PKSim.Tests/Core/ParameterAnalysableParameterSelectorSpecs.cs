using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterAnalysableParameterSelector : ContextSpecification<IParameterAnalysableParameterSelector>
   {
      protected override void Context()
      {
         sut = new ParameterAnalysableParameterSelector();
      }
   }

   public class When_checking_if_a_parameter_can_be_optimized : concern_for_ParameterAnalysableParameterSelector
   {
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = new Parameter
         {
            CanBeVaried = true,
            Info = {ReadOnly = false},
            Visible = true
         };
      }

      [Observation]
      public void should_return_true_if_a_parameter_can_be_varied_and_is_not_readonly_and_is_visible()
      {
         sut.CanUseParameter(_parameter).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_a_parameter_is_a_table_parameter()
      {
         _parameter.Formula = new TableFormula();
         sut.CanUseParameter(_parameter).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_parameter_is_readonly()
      {
         _parameter.Info.ReadOnly = true;
         sut.CanUseParameter(_parameter).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_parameter_cannot_be_varied()
      {
         _parameter.CanBeVaried = false;
         sut.CanUseParameter(_parameter).ShouldBeFalse();
      }


      [Observation]
      public void should_return_false_if_the_parameter_is_hidden()
      {
         _parameter.Visible = false;
         sut.CanUseParameter(_parameter).ShouldBeFalse();
      }
   }
}	