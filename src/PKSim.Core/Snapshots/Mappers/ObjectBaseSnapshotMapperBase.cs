using System;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots.Mappers
{
   public abstract class ObjectBaseSnapshotMapperBase<TModel, TSnapshot> : SnapshotMapperBase<TModel, TSnapshot>
      where TModel : IWithName, IWithDescription
      where TSnapshot : IWithName, IWithDescription, new()
   {
      protected void MapModelPropertiesToSnapshot(TModel model, TSnapshot snapshot)
      {
         snapshot.Name = SnapshotValueFor(model.Name);
         snapshot.Description = SnapshotValueFor(model.Description);
      }

      protected virtual void MapSnapshotPropertiesToModel(TSnapshot snapshot, TModel model)
      {
         model.Name = snapshot.Name;
         model.Description = snapshot.Description;
      }

      protected override Task<TSnapshot> SnapshotFrom(TModel model, Action<TSnapshot> configurationAction = null)
      {
         return base.SnapshotFrom(model, snapshot =>
         {
            MapModelPropertiesToSnapshot(model, snapshot);
            configurationAction?.Invoke(snapshot);
         });
      }
   }

   public abstract class ObjectBaseSnapshotMapperBase<TModel, TSnapshot, TContext> : ObjectBaseSnapshotMapperBase<TModel, TSnapshot>
      where TModel : IWithName, IWithDescription
      where TSnapshot : IWithName, IWithDescription, new()
   {
      public abstract Task<TModel> MapToModel(TSnapshot snapshot, TContext context);

      public sealed override Task<TModel> MapToModel(TSnapshot snapshot)
      {
         return Task.FromException<TModel>(new SnapshotMapToModelNotSupportedException<TModel, TContext>());
      }
   }

   public abstract class ObjectBaseSnapshotMapperBase<TModel, TSnapshot, TModelContext, TSnapshotContext> : ObjectBaseSnapshotMapperBase<TModel, TSnapshot, TModelContext>
      where TModel : IWithName, IWithDescription
      where TSnapshot : IWithName, IWithDescription, new()
   {
      public abstract Task<TSnapshot> MapToSnapshot(TModel model, TSnapshotContext context);

      public sealed override Task<TSnapshot> MapToSnapshot(TModel model)
      {
         return Task.FromException<TSnapshot>(new ModelMapToSnapshotNotSupportedException<TSnapshot, TSnapshotContext>());
      }
   }
}