using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface IOriginDataTask
   {
      SubPopulation DefaultSubPopulationFor(Species species);
      IEnumerable<CalculationMethodCategory> AllCalculationMethodCategoryFor(Species species);
      IEnumerable<CalculationMethod> DefaultCalculationMethodFor(Species species);
   }

   public class OriginDataTask : IOriginDataTask
   {
      private readonly ICalculationMethodCategoryRepository _calculationMethodRepository;

      public OriginDataTask(ICalculationMethodCategoryRepository calculationMethodRepository)
      {
         _calculationMethodRepository = calculationMethodRepository;
      }

      public SubPopulation DefaultSubPopulationFor(Species species)
      {
         var subPopulation = new SubPopulation();

         foreach (var pvvCategory in species.PVVCategories)
            subPopulation.AddParameterValueVersion(pvvCategory.DefaultItem);

         return subPopulation;
      }

      public IEnumerable<CalculationMethodCategory> AllCalculationMethodCategoryFor(Species species)
      {
         return _calculationMethodRepository.All().Where(x => x.IsIndividual)
            .Where(cmc => cmc.DefaultItemForSpecies(species) != null);
      }

      public IEnumerable<CalculationMethod> DefaultCalculationMethodFor(Species species)
      {
         return AllCalculationMethodCategoryFor(species).Select(x => x.DefaultItemForSpecies(species));
      }
   }
}