using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class ModelConfigurationRepository : StartableRepository<ModelConfiguration>, IModelConfigurationRepository
   {
      private readonly IFlatModelSpeciesRepository _flatModelSpeciesRepo;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly ICalculationMethodRepository _calculationMethodRepository;
      private readonly ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;
      private readonly IFlatModelCalculationMethodRepository _flatModelCalculationMethodRepository;
      private readonly ICache<string, IEnumerable<ModelConfiguration>> _modelsForSpecies;

      public ModelConfigurationRepository(IFlatModelSpeciesRepository flatModelSpeciesRepo,
         ISpeciesRepository speciesRepository,
         ICalculationMethodRepository calculationMethodRepository,
         ICalculationMethodCategoryRepository calculationMethodCategoryRepository,
         IFlatModelCalculationMethodRepository flatModelCalculationMethodRepository)
      {
         _flatModelSpeciesRepo = flatModelSpeciesRepo;
         _speciesRepository = speciesRepository;
         _calculationMethodRepository = calculationMethodRepository;
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
         _flatModelCalculationMethodRepository = flatModelCalculationMethodRepository;
         _modelsForSpecies = new Cache<string, IEnumerable<ModelConfiguration>>(s => new List<ModelConfiguration>());
      }

      public override IEnumerable<ModelConfiguration> All()
      {
         Start();
         return _modelsForSpecies.SelectMany(x => x);
      }

      protected override void DoStart()
      {
         foreach (var species in _speciesRepository.All())
         {
            var speciesName = species.Name;
            var modelNamesForSpecies = from flatModelSpecies in _flatModelSpeciesRepo.All()
               where flatModelSpecies.Species == speciesName
               select flatModelSpecies.Model;

            var modelsForSpecies = new List<ModelConfiguration>();
            modelNamesForSpecies.Each(model => modelsForSpecies.Add(modelConfigurationFor(model, speciesName)));
            _modelsForSpecies.Add(species.Name, modelsForSpecies);
         }
      }

      private ModelConfiguration modelConfigurationFor(string modelName, string species)
      {
         var calculationMethods = _calculationMethodRepository.Where(cm => cm.AllModels.Contains(modelName))
            .Where(cm => cm.AllSpecies.Contains(species))
            .Distinct();

         var modelConfig = new ModelConfiguration {ModelName = modelName, SpeciesName = species};

         var calcMethodCategories = categoriesFrom(calculationMethods, modelName);
         calcMethodCategories.Each(modelConfig.AddCalculationMethodCategory);

         return modelConfig;
      }

      private IEnumerable<CalculationMethodCategory> categoriesFrom(IEnumerable<CalculationMethod> calculationMethods, string modelName)
      {
         return calculationMethods.GroupBy(x => x.Category)
            .Select(methods => mapCategoryFrom(methods, methods.Key, modelName))
            .Where(category => !category.IsMolecule);
      }

      private CalculationMethodCategory mapCategoryFrom(IEnumerable<CalculationMethod> calculationMethods, string calcMethodCategoryName, string modelName)
      {
         var sortedCalculationMethods = calculationMethods.OrderBy(cm => _flatModelCalculationMethodRepository.By(modelName, cm.Name).Sequence);
         var templateCalcMethodCategory = _calculationMethodCategoryRepository.FindBy(calcMethodCategoryName);
         var calcMethodCategory = new CalculationMethodCategory {Name = calcMethodCategoryName, CategoryType = templateCalcMethodCategory.CategoryType};
         sortedCalculationMethods.Each(calcMethodCategory.Add);
         return calcMethodCategory;
      }

      public IEnumerable<ModelConfiguration> AllFor(Species species)
      {
         Start();
         return _modelsForSpecies[species.Name];
      }

      public ModelConfiguration DefaultFor(Species species)
      {
         Start();
         return AllFor(species).FirstOrDefault();
      }

      public ModelConfiguration FindById(string modelConfigurationId)
      {
         return All().FirstOrDefault(x => x.Id == modelConfigurationId);
      }
   }
}