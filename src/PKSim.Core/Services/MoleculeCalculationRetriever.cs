using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using ICoreCalculationMethodRepository = PKSim.Core.Repositories.ICoreCalculationMethodRepository;

namespace PKSim.Core.Services
{
   public interface IMoleculeCalculationRetriever
   {
      /// <summary>
      /// Returns the distinct <see cref="ICoreCalculationMethod"/> used in the <paramref name="simulation"/>
      /// </summary>
      IEnumerable<CoreCalculationMethod> AllMoleculeCalculationMethodsUsedBy(Simulation simulation);


      /// <summary>
      /// Returns the distinct <see cref="CoreCalculationMethod"/> used in the <paramref name="withCalculationMethods"/>
      /// </summary>
      IEnumerable<CoreCalculationMethod> AllMoleculeCalculationMethodsUsedBy(IWithCalculationMethods withCalculationMethods);

   }

   public class MoleculeCalculationRetriever : IMoleculeCalculationRetriever
   {
      private readonly ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;
      private readonly ICoreCalculationMethodRepository _coreCalculationMethodRepository;

      public MoleculeCalculationRetriever(ICalculationMethodCategoryRepository calculationMethodCategoryRepository, ICoreCalculationMethodRepository coreCalculationMethodRepository)
      {
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
         _coreCalculationMethodRepository = coreCalculationMethodRepository;
      }

      public IEnumerable<CoreCalculationMethod> AllMoleculeCalculationMethodsUsedBy(Simulation simulation)
      {
         return simulation.CompoundPropertiesList.SelectMany(AllMoleculeCalculationMethodsUsedBy)
            .Distinct();
      }

      public IEnumerable<CoreCalculationMethod> AllMoleculeCalculationMethodsUsedBy(IWithCalculationMethods withCalculationMethods)
      {
         return withCalculationMethods.AllCalculationMethods()
            .Select(calculationMethod => new {calculationMethod, category = _calculationMethodCategoryRepository.FindBy(calculationMethod.Category)})
            .Where(cmc => cmc.category.CategoryType == CategoryType.Molecule)
            .Select(cmc => _coreCalculationMethodRepository.FindByName(cmc.calculationMethod.Name))
            .Where(coreCalculationMethod => coreCalculationMethod != null);
      }
   }
}