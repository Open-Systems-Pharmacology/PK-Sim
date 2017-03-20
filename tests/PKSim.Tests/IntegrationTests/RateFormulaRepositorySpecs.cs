using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_RateFormulaRepository : ContextForIntegration<IRateFormulaRepository>
   {
   }

   
   public class When_retrieving_the_available_rate_formula_repository : concern_for_RateFormulaRepository
   {
      [Observation]
      public void should_retrieve_nonempty_formula_string()
      {
         string formula = sut.FormulaFor(new RateKey("CompoundMW_PKSim", "PARAM_MWEff"));
         string.IsNullOrEmpty(formula).ShouldBeFalse();
      }
   }
}