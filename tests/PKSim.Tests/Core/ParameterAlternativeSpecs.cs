using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterAlternative : ContextSpecification<ParameterAlternative>
   {
      protected IParameter _defaultParameter;
      protected IParameter _inputParameter;
      protected ParameterAlternativeGroup _parameterGroup;

      protected override void Context()
      {

         _defaultParameter = DomainHelperForSpecs.ConstantParameterWithValue(isDefault: true).WithName("Default");
         _inputParameter = DomainHelperForSpecs.ConstantParameterWithValue(isDefault: false).WithName("Input");
         sut = new ParameterAlternative {_defaultParameter, _inputParameter};

         _parameterGroup = new ParameterAlternativeGroup();
         _parameterGroup.AddAlternative(sut);
      }
   }

   public class When_retrieving_the_list_of_all_parameters_defined_in_a_parameter_alternative_that_should_received_the_same_value_origin : concern_for_ParameterAlternative
   {
      [Observation]
      public void should_return_all_parameters_for_a_calculated_alternative()
      {
         _parameterGroup.Name = CoreConstants.Groups.COMPOUND_INTESTINAL_PERMEABILITY;
         sut.Name = PKSimConstants.UI.CalculatedAlernative;
         sut.AlllParametersWithSameValueOrigin.ShouldOnlyContain(_defaultParameter,_inputParameter );
      }

      [Observation]
      public void should_return_all_parameters_if_no_parameter_is_found_as_input_parameter()
      {
         _inputParameter.IsDefault = true;
         sut.AlllParametersWithSameValueOrigin.ShouldOnlyContain(_defaultParameter, _inputParameter);
      }
      
      [Observation]
      public void should_return_all_input_parameters_if_at_least_one_input_paraemter_is_found()
      {
         sut.AlllParametersWithSameValueOrigin.ShouldOnlyContain(_inputParameter);
      }
   }
}	