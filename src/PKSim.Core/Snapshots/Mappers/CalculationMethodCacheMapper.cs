using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using SnapshotCalculationMethodCache = PKSim.Core.Snapshots.CalculationMethodCache;
using ModelCalculationMethodCache = OSPSuite.Core.Domain.CalculationMethodCache;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CalculationMethodCacheSnapshotContext : SnapshotContext
   {
      public ModelCalculationMethodCache CalculationMethodCache { get; }

      public CalculationMethodCacheSnapshotContext(ModelCalculationMethodCache calculationMethodCache, SnapshotContext baseContext) : base(baseContext)
      {
         CalculationMethodCache = calculationMethodCache;
      }
   }

   public class CalculationMethodCacheMapper : SnapshotMapperBase<ModelCalculationMethodCache, SnapshotCalculationMethodCache, CalculationMethodCacheSnapshotContext>
   {
      private readonly ICalculationMethodRepository _calculationMethodRepository;
      private readonly ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;
      private readonly IOSPSuiteLogger _logger;

      public CalculationMethodCacheMapper(
         ICalculationMethodRepository calculationMethodRepository, 
         ICalculationMethodCategoryRepository calculationMethodCategoryRepository,
         IOSPSuiteLogger logger)
      {
         _calculationMethodRepository = calculationMethodRepository;
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
         _logger = logger;
      }

      public override Task<SnapshotCalculationMethodCache> MapToSnapshot(ModelCalculationMethodCache model)
      {
         return mapCalculationMethodsToSnapshot(model);
      }

      public virtual Task<SnapshotCalculationMethodCache> MapToSnapshot(ModelCalculationMethodCache model, string species)
      {
         return mapCalculationMethodsToSnapshot(model, species);
      }

      private Task<SnapshotCalculationMethodCache> mapCalculationMethodsToSnapshot(ModelCalculationMethodCache model, string species = null)
      {
         return SnapshotFrom(model, snapshot => { addCalculationMethodsToSnapshot(snapshot, model, species); });
      }

      private void addCalculationMethodsToSnapshot(SnapshotCalculationMethodCache snapshot, ModelCalculationMethodCache calculationMethodCache, string species)
      {
         calculationMethodCache.All().Each(cm => addCalculationMethodToSnapshot(snapshot, cm, species));
      }

      private void addCalculationMethodToSnapshot(SnapshotCalculationMethodCache snapshot, CalculationMethod calculationMethod, string species)
      {
         var category = _calculationMethodCategoryRepository.FindBy(calculationMethod.Category);

         var allPossibleCalculationMethods = category.AllItems().ToList();

         //only one CM in this category. Nothing to do
         if (allPossibleCalculationMethods.Count <= 1)
            return;

         //all CM have the same name? they will never be displayed together
         if (allPossibleCalculationMethods.Select(x => x.DisplayName).Distinct().Count() == 1)
            return;

         //only one calculation method exists for the given species. Nothing to do
         if (!string.IsNullOrEmpty(species) && allPossibleCalculationMethods.Count(x => x.AllSpecies.Contains(species)) == 1)
            return;

         //at least one CM that can be used in two models or different names. We may have a choice here. save it
         snapshot.Add(calculationMethod.Name);
      }

      public override Task<ModelCalculationMethodCache> MapToModel(SnapshotCalculationMethodCache snapshot, CalculationMethodCacheSnapshotContext snapshotContext)
      {
         UpdateCalculationMethodCache(snapshotContext.CalculationMethodCache, snapshot);
         return Task.FromResult(snapshotContext.CalculationMethodCache);
      }

      public virtual void UpdateCalculationMethodCache(IWithCalculationMethods withCalculationMethods, SnapshotCalculationMethodCache snapshot)
      {
         UpdateCalculationMethodCache(withCalculationMethods.CalculationMethodCache, snapshot);
      }
      
      public virtual void UpdateCalculationMethodCache(ModelCalculationMethodCache calculationMethodCache, SnapshotCalculationMethodCache snapshot, bool oneCalculationMethodPerCategory = true)
      {
         snapshot?.Each(cm => useCalculationMethodIn(calculationMethodCache, cm, oneCalculationMethodPerCategory));
      }

      private void useCalculationMethodIn(ModelCalculationMethodCache calculationMethodCache, string calculationMethodName, bool oneCalculationMethodPerCategory)
      {
         var calculationMethod = _calculationMethodRepository.FindByName(calculationMethodName);
         if (calculationMethod == null)
         {
            _logger.AddWarning(PKSimConstants.Error.CalculationMethodNotFound(calculationMethodName));
            return;
         }

         if(oneCalculationMethodPerCategory)
         {
            var existingCalculationMethod = calculationMethodCache.CalculationMethodFor(calculationMethod.Category);
            if (existingCalculationMethod != null)
               calculationMethodCache.RemoveCalculationMethod(existingCalculationMethod);
         }

         calculationMethodCache.AddCalculationMethod(calculationMethod);
      }
   }
}