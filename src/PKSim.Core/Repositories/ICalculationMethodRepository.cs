using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Repositories
{
   public interface ICalculationMethodRepository : IStartableRepository<CalculationMethod>
   {
      CalculationMethod FindBy(string name);
   }
}