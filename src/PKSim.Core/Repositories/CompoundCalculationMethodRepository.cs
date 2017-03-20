using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Repositories;
using CalculationMethod = OSPSuite.Core.Domain.CalculationMethod;

namespace PKSim.Core.Repositories
{
   public class CompoundCalculationMethodRepository : ICompoundCalculationMethodRepository
   {
      private readonly ICompoundCalculationMethodCategoryRepository _calculationMethodCategoryRepository;

      public CompoundCalculationMethodRepository(ICompoundCalculationMethodCategoryRepository calculationMethodCategoryRepository)
      {
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
      }

      public IEnumerable<CalculationMethod> All()
      {
         return _calculationMethodCategoryRepository.All().SelectMany(x => x.AllItems());
      }
   }
}