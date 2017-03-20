using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_SetCalculationMethodCommand : ContextSpecification<SetCalculationMethodCommand<Compound>>
   {
      protected CalculationMethod _oldCalculationMethod;
      protected CalculationMethod _newCalculationMethod;
      protected string _category;
      protected Compound _objectWithCalculationMethods;
      protected IExecutionContext _context;
      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         _objectWithCalculationMethods = new Compound();
         _category = "A Category";
         _oldCalculationMethod = new CalculationMethod {Category = _category};
         _newCalculationMethod = new CalculationMethod {Category = _category};

         sut = new SetCalculationMethodCommand<Compound>(_objectWithCalculationMethods, _category, _newCalculationMethod, _oldCalculationMethod);
      }
   }

   public class when_setting_a_new_calculation_method_for_a_compound : concern_for_SetCalculationMethodCommand
   {
      protected override void Because()
      {
         sut.Run(_context);
      }

      [Observation]
      public void the_compound_should_have_an_updated_calculation_method_for_the_category()
      {
         _objectWithCalculationMethods.CalculationMethodFor(_category).ShouldBeEqualTo(_newCalculationMethod);
      }
   }

   public class when_executing_and_reversing_an_update_to_a_calculation_method_ : concern_for_SetCalculationMethodCommand
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_context);
      }

      [Observation]
      public void the_compound_should_have_the_original_calculation_method_for_the_category()
      {
         _objectWithCalculationMethods.CalculationMethodFor(_category).ShouldBeEqualTo(_oldCalculationMethod);
      }
   }
}
