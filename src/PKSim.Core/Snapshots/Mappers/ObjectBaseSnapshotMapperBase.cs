using System;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots.Mappers
{
   public abstract class ObjectBaseSnapshotMapperBase<TModel, TSnapshot> : SnapshotMapperBase<TModel, TSnapshot>
      where TSnapshot : ISnapshot, new()
      where TModel : IObjectBase
   {
      protected void MapModelPropertiesToSnapshot(TModel model, TSnapshot snapshot)
      {
         snapshot.Name = model.Name;
         snapshot.Description = SnapshotValueFor(model.Description);
      }

      protected void MapSnapshotPropertiesToModel(TSnapshot snapshot, TModel model)
      {
         model.Name = snapshot.Name;
         model.Description = snapshot.Description;
      }

      protected override TSnapshot SnapshotFrom(TModel model, Action<TSnapshot> configurationAction = null)
      {
         return base.SnapshotFrom(model, snapshot =>
         {
            MapModelPropertiesToSnapshot(model, snapshot);
            configurationAction?.Invoke(snapshot);
         });
      }
   }
}