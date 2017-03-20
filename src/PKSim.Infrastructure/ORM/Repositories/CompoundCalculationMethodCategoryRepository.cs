using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class CompoundCalculationMethodCategoryRepository : StartableRepository<CalculationMethodCategory>, ICompoundCalculationMethodCategoryRepository
   {
      private readonly ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;
      private readonly ICache<string, CalculationMethodCategory> _allCalculationMethodCategories;

      public CompoundCalculationMethodCategoryRepository(ICalculationMethodCategoryRepository calculationMethodCategoryRepository)
      {
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
         _allCalculationMethodCategories = new Cache<string, CalculationMethodCategory>(x => x.Name, x => null);
      }

      protected override void DoStart()
      {
         _calculationMethodCategoryRepository.All()
            .Where(x => x.IsMolecule)
            .Each(_allCalculationMethodCategories.Add);
      }

      public override IEnumerable<CalculationMethodCategory> All()
      {
         Start();
         return _allCalculationMethodCategories;
      }
   }
}