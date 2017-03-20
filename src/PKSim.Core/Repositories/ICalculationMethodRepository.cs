using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Repositories
{
   public interface ICalculationMethodRepository : IStartableRepository<CalculationMethod>
   {
      CalculationMethod FindBy(string name);
   }
}