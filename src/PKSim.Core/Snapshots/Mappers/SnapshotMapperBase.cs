using System;
using System.Threading.Tasks;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Snapshots.Mappers
{
   public interface ISnapshotMapperSpecification : ISnapshotMapper, ISpecification<Type>
   {
   }

   public abstract class SnapshotMapperBase<TModel, TSnapshot> : ISnapshotMapperSpecification
      where TSnapshot : new()
   {
      public virtual async Task<object> MapToSnapshot(object model) => await MapToSnapshot(model.DowncastTo<TModel>());

      public virtual async Task<object> MapToModel(object snapshot) => await MapToModel(snapshot.DowncastTo<TSnapshot>());

      public abstract Task<TSnapshot> MapToSnapshot(TModel model);

      public abstract Task<TModel> MapToModel(TSnapshot snapshot);

      public Type SnapshotTypeFor<T>() => typeof(TSnapshot);

      public virtual bool IsSatisfiedBy(Type item)
      {
         return item.IsAnImplementationOf<TModel>() || item.IsAnImplementationOf<TSnapshot>();
      }

      protected string SnapshotValueFor(string value) => !string.IsNullOrEmpty(value) ? value : null;

      protected T? SnapshotValueFor<T>(T value, T defaultValue = default(T)) where T : struct
      {
         if (Equals(value, defaultValue))
            return null;

         return value;
      }

      protected string ModelValueFor(string snapshotValue) => snapshotValue ?? "";

      protected T ModelValueFor<T>(T? snapshotValue, T defaultValue = default(T)) where T : struct
      {
         return snapshotValue.GetValueOrDefault(defaultValue);
      }

      protected virtual Task<TSnapshot> SnapshotFrom(TModel model, Action<TSnapshot> configurationAction = null)
      {
         var snapshot = new TSnapshot();
         configurationAction?.Invoke(snapshot);
         return Task.FromResult(snapshot);
      }
   }

   public abstract class SnapshotMapperBase<TModel, TSnapshot, TContext> : SnapshotMapperBase<TModel, TSnapshot>
      where TSnapshot : new()
   {
      public abstract Task<TModel> MapToModel(TSnapshot snapshot, TContext context);

      public sealed override Task<TModel> MapToModel(TSnapshot snapshot)
      {
         return Task.FromException<TModel>(new SnapshotMapToModelNotSupportedException<TModel, TContext>());
      }
   }

   public abstract class SnapshotMapperBase<TModel, TSnapshot, TModelContext, TSnapshotContext> : SnapshotMapperBase<TModel, TSnapshot, TModelContext>
      where TSnapshot : new()
   {
      public abstract Task<TSnapshot> MapToSnapshot(TModel model, TSnapshotContext context);

      public sealed override Task<TSnapshot> MapToSnapshot(TModel model)
      {
         return Task.FromException<TSnapshot>(new ModelMapToSnapshotNotSupportedException<TSnapshot, TSnapshotContext>());
      }
   }
}