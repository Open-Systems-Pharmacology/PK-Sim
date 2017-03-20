using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_PKSimCalculationMethodsTask : ContextSpecification<PKSimCalculationMethodsTask>
   {
      private IExecutionContext _context;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         sut = new PKSimCalculationMethodsTask(_context);
      }
   }

   public class when_setting_new_calculation_method_on_a_compound_ : concern_for_PKSimCalculationMethodsTask
   {
      private Compound _compound;
      private string _cateogry;
      private CalculationMethod _newCalculationMethod;
      private CalculationMethod _oldCalculationMethod;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound();
         _cateogry = "A Category";
         _newCalculationMethod = new CalculationMethod {Category = _cateogry};
         _oldCalculationMethod = new CalculationMethod {Category = _cateogry};

         _compound.AddCalculationMethod(_oldCalculationMethod);
      }

      protected override void Because()
      {
         sut.SetCalculationMethod(_compound, _cateogry, _newCalculationMethod, _oldCalculationMethod);
      }

      [Observation]
      public void compound_must_have_new_calculation_method_set_for_category()
      {
         _compound.CalculationMethodFor(_cateogry).ShouldBeEqualTo(_newCalculationMethod);
      }

      [Observation]
      public void old_calculation_method_should_be_removed_from_compound()
      {
         _compound.AllCalculationMethods().ShouldOnlyContain(_newCalculationMethod);
      }
   }
}
