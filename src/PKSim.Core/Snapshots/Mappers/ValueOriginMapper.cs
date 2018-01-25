﻿using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using ModelValueOrigin = OSPSuite.Core.Domain.ValueOrigin;
using SnapshotValueOrigin = PKSim.Core.Snapshots.ValueOrigin;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ValueOriginMapper : SnapshotMapperBase<ModelValueOrigin, SnapshotValueOrigin>
   {
      public override Task<SnapshotValueOrigin> MapToSnapshot(ModelValueOrigin valueOrigin)
      {
         if (valueOrigin.Source == ValueOriginSources.Undefined)
            return Task.FromResult<SnapshotValueOrigin>(null);

         return SnapshotFrom(valueOrigin, x =>
         {
            x.Id = valueOrigin.Id;
            x.Description = SnapshotValueFor(valueOrigin.Description);
            x.Source = SnapshotValueFor(valueOrigin.Source.Id, ValueOriginSourceId.Undefined);
            x.Method = SnapshotValueFor(valueOrigin.Method.Id, ValueOriginDeterminationMethodId.Undefined);
         });
      }

      public override Task<ModelValueOrigin> MapToModel(SnapshotValueOrigin snapshot)
      {
         return Task.FromException<ModelValueOrigin>(new SnapshotMapToModelNotSupportedException<ModelValueOrigin, ModelValueOrigin>());
      }

      public virtual void UpdateValueOrigin(ModelValueOrigin valueOrigin, SnapshotValueOrigin snapshot)
      {
         if (snapshot == null)
            return;

         var snapshotValueOrigin = new ModelValueOrigin
         {
            Id = snapshot.Id,
            Source = ValueOriginSources.ById(ModelValueFor(snapshot.Source, ValueOriginSourceId.Undefined)),
            Method = ValueOriginDeterminationMethods.ById(ModelValueFor(snapshot.Method, ValueOriginDeterminationMethodId.Undefined)),
            Description = ModelValueFor(snapshot.Description),
         };

         valueOrigin.UpdateAllFrom(snapshotValueOrigin);
      }
   }
}