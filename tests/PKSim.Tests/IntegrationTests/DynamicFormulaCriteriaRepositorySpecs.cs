using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_DynamicFormulaCriteriaRepository : ContextForIntegration<IDynamicFormulaCriteriaRepository>
   {
      protected ICalculationMethodRepository _calcMethodsRepo;
      protected IEnumerable<string> _dynamicFormulasCalcMethodNames;
      protected IRateFormulaRepository _formulaRepo;

      protected override void Context()
      {
         var _calcMethodsRepo = IoC.Resolve<ICalculationMethodRepository>();

         IList<string> dynFormulaCalcMethodNames=new List<string>();

         var dynamicFormulasCalcMethods =
            _calcMethodsRepo.All().Where(cm => cm.Category.Equals(CoreConstants.Category.DynamicFormulas));
         dynamicFormulasCalcMethods.Each(cm => dynFormulaCalcMethodNames.Add(cm.Name));
         _dynamicFormulasCalcMethodNames = dynFormulaCalcMethodNames;

         _formulaRepo = IoC.Resolve<IRateFormulaRepository>();

         sut = IoC.Resolve<IDynamicFormulaCriteriaRepository>();
      }
   }

   
   public class when_getting_dynamic_formula_criteria : concern_for_DynamicFormulaCriteriaRepository
   {
      protected IEnumerable<RateFormula> _allDynamicFormulas;

      protected override void Because()
      {
         _allDynamicFormulas=_formulaRepo.All().Where(f => _dynamicFormulasCalcMethodNames.Contains(f.CalculationMethod));
      }

      [Observation]
      public void every_dynamic_formula_should_have_nonempty_criteria_list()
      {
         foreach (var dynamicFormula in _allDynamicFormulas)
         {
            sut.CriteriaFor(dynamicFormula.CalculationMethod,dynamicFormula.Rate).Count().ShouldBeGreaterThan(0);
         }
      }
   }
}
