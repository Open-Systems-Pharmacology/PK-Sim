using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class CalculationMethodCategoryRepository : StartableRepository<CalculationMethodCategory>, ICalculationMethodCategoryRepository
   {
      private readonly IFlatCategoryRepository _flatCategoryRepository;
      private readonly IFlatCalculationMethodRepository _flatCalculationMethodRepository;
      private readonly ICalculationMethodRepository _calculationMethodRepository;
      private readonly ICache<string, CalculationMethodCategory> _calculationMethodCategories = new Cache<string, CalculationMethodCategory>(x => x.Name);

      public CalculationMethodCategoryRepository(IFlatCategoryRepository flatCategoryRepository, IFlatCalculationMethodRepository flatCalculationMethodRepository, ICalculationMethodRepository calculationMethodRepository)
      {
         _flatCategoryRepository = flatCategoryRepository;
         _flatCalculationMethodRepository = flatCalculationMethodRepository;
         _calculationMethodRepository = calculationMethodRepository;
      }

      public override IEnumerable<CalculationMethodCategory> All()
      {
         Start();
         return _calculationMethodCategories;
      }

      protected override void DoStart()
      {
         foreach (var calculationMethods in _calculationMethodRepository.All().GroupBy(x => x.Category))
         {
            var sortedCalculationMethods = calculationMethods.OrderBy(cm => _flatCalculationMethodRepository.FindById(cm.Name).Sequence);
            var flatCategory = _flatCategoryRepository.FindBy(calculationMethods.Key);
            var calculationMethodCategory = new CalculationMethodCategory {Name = flatCategory.Name, CategoryType = flatCategory.CategoryType};
            sortedCalculationMethods.Each(calculationMethodCategory.Add);
            _calculationMethodCategories.Add(calculationMethodCategory);
         }
      }

      public CalculationMethodCategory FindBy(string categoryName)
      {
         Start();
         return _calculationMethodCategories[categoryName];
      }
   }
}