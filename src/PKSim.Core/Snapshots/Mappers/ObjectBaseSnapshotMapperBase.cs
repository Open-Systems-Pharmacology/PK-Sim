using System;
using OSPSuite.Core.Domain;
using PKSim.Assets;

namespace PKSim.Core.Snapshots.Mappers
{
   public abstract class ObjectBaseSnapshotMapperBase<TModel, TSnapshot> : SnapshotMapperBase<TModel, TSnapshot>
      where TModel : IObjectBase
      where TSnapshot : ISnapshot, new()
   {
      protected void MapModelPropertiesToSnapshot(TModel model, TSnapshot snapshot)
      {
         snapshot.Name = model.Name;
         snapshot.Description = SnapshotValueFor(model.Description);
      }

      protected void MapSnapshotPropertiesToModel(TSnapshot snapshot, TModel model)
      {
         model.Name = snapshot.Name;

         if (!string.IsNullOrEmpty(snapshot.Description))
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

   public abstract class ObjectBaseSnapshotMapperBase<TModel, TSnapshot, TContext> : ObjectBaseSnapshotMapperBase<TModel, TSnapshot>
      where TModel : IObjectBase
      where TSnapshot : ISnapshot, new()
   {
      public abstract TModel MapToModel(TSnapshot snapshot, TContext context);

      public override TModel MapToModel(TSnapshot snapshot)
      {
         throw new SnapshotMapToModelNotSupportedNotSupportedException<TModel, TContext>();
      }
   }
}