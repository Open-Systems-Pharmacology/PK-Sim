using System;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots.Mappers
{
   public abstract class ObjectBaseSnapshotMapperBase<TModel, TSnapshot, TSnapshotContext> : SnapshotMapperBase<TModel, TSnapshot, TSnapshotContext>
      where TModel : IWithName, IWithDescription
      where TSnapshot : IWithName, IWithDescription, new() 
      where TSnapshotContext : SnapshotContext
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

   public abstract class ObjectBaseSnapshotMapperBase<TModel, TSnapshot, TSnapshotContext, TModelContext> : ObjectBaseSnapshotMapperBase<TModel, TSnapshot, TSnapshotContext>
      where TModel : IWithName, IWithDescription
      where TSnapshot : IWithName, IWithDescription, new() 
      where TSnapshotContext : SnapshotContext
   {
      public abstract Task<TSnapshot> MapToSnapshot(TModel model, TModelContext context);

      public sealed override Task<TSnapshot> MapToSnapshot(TModel model)
      {
         return Task.FromException<TSnapshot>(new ModelMapToSnapshotNotSupportedException<TSnapshot, TSnapshotContext>());
      }
   }

   public abstract class ObjectBaseSnapshotMapperBase<TModel, TSnapshot> : ObjectBaseSnapshotMapperBase<TModel, TSnapshot, SnapshotContext> 
      where TModel : IWithName, IWithDescription 
      where TSnapshot : IWithName, IWithDescription, new()
   {

   }

}