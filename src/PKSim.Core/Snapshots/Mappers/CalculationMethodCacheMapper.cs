using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using SnapshotCalculationMethodCache = PKSim.Core.Snapshots.CalculationMethodCache;
using ModelCalculationMethodCache = OSPSuite.Core.Domain.CalculationMethodCache;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CalculationMethodCacheMapper : SnapshotMapperBase<ModelCalculationMethodCache, SnapshotCalculationMethodCache, ModelCalculationMethodCache>
   {
      private readonly ICalculationMethodRepository _calculationMethodRepository;
      private readonly ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;

      public CalculationMethodCacheMapper(ICalculationMethodRepository calculationMethodRepository, ICalculationMethodCategoryRepository calculationMethodCategoryRepository)
      {
         _calculationMethodRepository = calculationMethodRepository;
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
      }

      public override Task<SnapshotCalculationMethodCache> MapToSnapshot(ModelCalculationMethodCache model)
      {
         return SnapshotFrom(model, snapshot =>
         {
            addCalculationMethodsToSnapshot(snapshot, model);
         });
      }

      private void addCalculationMethodsToSnapshot(SnapshotCalculationMethodCache snapshot, ModelCalculationMethodCache model)
      {
         model.All().Each(cm =>
         {
            var category = _calculationMethodCategoryRepository.FindBy(cm.Category);
            if (category.AllItems().Count() != 1)
               snapshot.Add(cm.Name);
         });
      }

      public override Task<ModelCalculationMethodCache> MapToModel(SnapshotCalculationMethodCache snapshot, ModelCalculationMethodCache calculationMethodCache)
      {
         snapshot.Each(cm => useCalculationMethodIn(calculationMethodCache, cm));
         return Task.FromResult(calculationMethodCache);
      }

      public virtual void UpdateCalculationMethodCache(IWithCalculationMethods withCalculationMethods, SnapshotCalculationMethodCache snapshot)
      {
         MapToModel(snapshot, withCalculationMethods.CalculationMethodCache);
      }

      private void useCalculationMethodIn(ModelCalculationMethodCache calculationMethodCache, string calculationMethodName)
      {
         var calculationMethod = _calculationMethodRepository.FindByName(calculationMethodName);
         if (calculationMethod == null)
            throw new SnapshotOutdatedException(PKSimConstants.Error.CalculationMethodNotFound(calculationMethodName));

         var existingCalculationMethod = calculationMethodCache.CalculationMethodFor(calculationMethod.Category);
         if (existingCalculationMethod != null)
            calculationMethodCache.RemoveCalculationMethod(existingCalculationMethod);

         calculationMethodCache.AddCalculationMethod(calculationMethod);
      }
   }
}