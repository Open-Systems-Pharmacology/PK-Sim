using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface ICalculationMethodCategoryRepository : IStartableRepository<CalculationMethodCategory>
   {
      CalculationMethodCategory FindBy(string categoryName);
      bool HasMoreThanOneOption(CalculationMethod calculationMethod, Species species);
   }
}