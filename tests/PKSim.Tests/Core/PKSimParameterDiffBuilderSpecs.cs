using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Comparison;
using OSPSuite.Core.Domain;
using PKSim.Core.Comparison;

namespace PKSim.Core
{
   public abstract class concern_for_PKSimParameterDiffBuilder : StaticContextSpecification
   {
      protected IParameter _parameter1;
      protected IParameter _parameter2;
      protected IComparison<IParameter> _comparison;
      protected ComparerSettings _settings;

      protected override void Context()
      {
         _settings = new ComparerSettings();
         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(visible: true);
         _parameter2 = DomainHelperForSpecs.ConstantParameterWithValue(visible: true);
         _comparison = new Comparison<IParameter>(_parameter1, _parameter2, _settings, new DiffReport(), null);
      }
   }

   public class When_checking_if_two_parameters_should_be_compared : concern_for_PKSimParameterDiffBuilder
   {
      [Observation]
      public void should_return_true_if_both_parameters_are_visible()
      {
         PKSimParameterDiffBuilder.ShouldCompareParametersIn(_comparison).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_at_least_one_parameter_is_hidden_and_hidden_entities_should_not_be_compared()
      {
         _parameter1.Visible = false;
         _settings.CompareHiddenEntities = false;
         PKSimParameterDiffBuilder.ShouldCompareParametersIn(_comparison).ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_hidden_entities_should_be_compared()
      {
         _parameter1.Visible = false;
         _settings.CompareHiddenEntities = true;
         PKSimParameterDiffBuilder.ShouldCompareParametersIn(_comparison).ShouldBeTrue();
      }
   }
}