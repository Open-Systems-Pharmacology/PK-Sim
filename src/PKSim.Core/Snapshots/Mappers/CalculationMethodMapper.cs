using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using SnapshotCalculationMethod = PKSim.Core.Snapshots.CalculationMethod;
using ModelCalculationMethod = OSPSuite.Core.Domain.CalculationMethod;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CalculationMethodMapper : SnapshotMapperBase<ModelCalculationMethod, SnapshotCalculationMethod>
   {
      public override SnapshotCalculationMethod MapToSnapshot(ModelCalculationMethod model)
      {
         return SnapshotFrom(model, snapshot =>
         {
            snapshot.Description = null;
         });
      }

      public override ModelCalculationMethod MapToModel(SnapshotCalculationMethod snapshot)
      {
         throw new NotImplementedException();
      }

      public virtual List<SnapshotCalculationMethod> MapToSnapshot(CalculationMethodCache calculationMethodCache)
      {
         return calculationMethodCache.All().Select(MapToSnapshot).ToList();
      }
   }
}