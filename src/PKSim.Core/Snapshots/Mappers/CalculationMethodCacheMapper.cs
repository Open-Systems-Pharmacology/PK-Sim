using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using SnapshotCalculationMethodCache = PKSim.Core.Snapshots.CalculationMethodCache;
using ModelCalculationMethodCache = OSPSuite.Core.Domain.CalculationMethodCache;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CalculationMethodCacheMapper : SnapshotMapperBase<ModelCalculationMethodCache, SnapshotCalculationMethodCache>
   {
      private readonly ICalculationMethodRepository _calculationMethodRepository;
      private readonly ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;

      public CalculationMethodCacheMapper(ICalculationMethodRepository calculationMethodRepository, ICalculationMethodCategoryRepository calculationMethodCategoryRepository)
      {
         _calculationMethodRepository = calculationMethodRepository;
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
      }

      public override SnapshotCalculationMethodCache MapToSnapshot(ModelCalculationMethodCache model)
      {
         return SnapshotFrom(model, snapshot =>
         {
            addCalculationMethodsToSnapshot(snapshot, model);
         });
      }

      private void addCalculationMethodsToSnapshot(CalculationMethodCache snapshot, ModelCalculationMethodCache model)
      {
         model.All().Each(cm =>
         {
            var category = _calculationMethodCategoryRepository.FindBy(cm.Category);
            if(category.AllItems().Count()!=1)
               snapshot.Add(cm.Name);
         });
      }

      public override ModelCalculationMethodCache MapToModel(SnapshotCalculationMethodCache snapshot)
      {
         throw new NotImplementedException();
      }

      public virtual void UpdateCalculationMethodCache(IWithCalculationMethods withCalculationMethods, CalculationMethodCache snapshotCalculationMethods)
      {
         throw new NotImplementedException();
      }
   }
}