using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_ParameterAlternativeDTO : ContextSpecification<ParameterAlternativeDTO>
   {
      protected ParameterAlternative _parameterAlternative;
      protected ValueOrigin _valueOrigin;

      protected override void Context()
      {
         _parameterAlternative = new ParameterAlternative();
         _valueOrigin = new ValueOrigin();
         sut = new ParameterAlternativeDTO(_parameterAlternative, _valueOrigin);
      }
   }

   public class When_creating_a_parmaeter_alternative_dto_for_an_alternative_that_have_some_input_parameters : concern_for_ParameterAlternativeDTO
   {
      private IParameter _defaultParameter;
      private IParameter _inputParameter;
      private IParameter _anotherInputParameter;

      protected override void Context()
      {
         base.Context();

         _defaultParameter = DomainHelperForSpecs.ConstantParameterWithValue(isDefault: true).WithName("P1");
         _inputParameter = DomainHelperForSpecs.ConstantParameterWithValue(isDefault: false).WithName("P2");
         _anotherInputParameter = DomainHelperForSpecs.ConstantParameterWithValue(isDefault: false).WithName("P3");
         _parameterAlternative.Add(_defaultParameter);
         _parameterAlternative.Add(_inputParameter);
         _parameterAlternative.Add(_anotherInputParameter);

         sut = new ParameterAlternativeDTO(_parameterAlternative, _inputParameter.ValueOrigin);
      }

      [Observation]
      public void should_reference_the_value_origin_passed_as_parameter()
      {
         Assert.AreSame(sut.ValueOrigin, _inputParameter.ValueOrigin);
      }
   }
}