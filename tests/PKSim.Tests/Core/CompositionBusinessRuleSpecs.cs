using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Validation;

namespace PKSim.Core
{
   public class When_checking_the_validity_of_a_validatable_object_using_the_composition_business_rule : StaticContextSpecification
   {
      private WrapperObject _wrapperObject;

      protected override void Context()
      {
         _wrapperObject = new WrapperObject();
      }

      [Observation]
      public void should_return_a_valid_state_if_the_composed_property_is_valid()
      {
         _wrapperObject.MyValue = 4.5;
         _wrapperObject.IsValid().ShouldBeTrue();
         _wrapperObject.Validate(x => x.MyValue, 3.5).IsEmpty.ShouldBeTrue();
      }

      [Observation]
      public void should_return_an_invalid_state_if_the_composed_property_is_invalid()
      {
         _wrapperObject.MyValue = 8;
         _wrapperObject.IsValid().ShouldBeFalse();
         _wrapperObject.Validate(x => x.MyValue, 7).IsEmpty.ShouldBeFalse();
      }
   }

   internal class WrapperObject : IValidatable
   {
      private readonly IParameter _parameter;
      public IBusinessRuleSet Rules { get; } = new BusinessRuleSet();

      public double MyValue
      {
         get => _parameter.Value;
         set => _parameter.Value = value;
      }

      public WrapperObject()
      {
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(4);
         _parameter.MinValue = 3;
         _parameter.MaxValue = 5;
         Rules.Add(new CompositionBusinessRule<IParameter, double>(_parameter, x => x.Value, nameof(MyValue)));
      }
   }
}