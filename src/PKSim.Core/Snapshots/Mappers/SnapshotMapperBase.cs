using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Snapshots.Mappers
{
   public interface ISnapshotMapperSpecification : ISnapshotMapper, ISpecification<Type>
   {
   }

   public abstract class SnapshotMapperBase<TModel, TSnapshot, TSnapshotContext> : ISnapshotMapperSpecification, ISnapshotMapperWithContext<TModel, TSnapshot, TSnapshotContext>
      where TSnapshot : new()
      where TSnapshotContext : SnapshotContext
   {
      public virtual async Task<object> MapToSnapshot(object model) => await MapToSnapshot(model.DowncastTo<TModel>());

      public async Task<object> MapToModel(object snapshot, SnapshotContext snapshotContext) => await MapToModel(snapshot.DowncastTo<TSnapshot>(), snapshotContext.DowncastTo<TSnapshotContext>());

      public abstract Task<TModel> MapToModel(TSnapshot snapshot, TSnapshotContext context);

      public abstract Task<TSnapshot> MapToSnapshot(TModel model);

      public Type SnapshotTypeFor<T>() => typeof(TSnapshot);

      public ISnapshotMapper MapperFor(object modelOrSnapshotType) => this;

      public ISnapshotMapper MapperFor(Type modelOrSnapshotType) => this;

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

      protected T? SnapshotValueFor<T>(T? value, T defaultValue = default(T)) where T : struct
      {
         if (value == null)
            return null;

         return SnapshotValueFor(value.Value, defaultValue);
      }

      protected T[] SnapshotValueFor<T>(IEnumerable<T> values)
      {
         if (values == null)
            return null;

         var array = values.ToArray();
         return !array.Any() ? null : array;
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

   public abstract class SnapshotMapperBase<TModel, TSnapshot, TSnapshotContext, TModelContext> : SnapshotMapperBase<TModel, TSnapshot, TSnapshotContext>
      where TSnapshot : new() where TSnapshotContext : SnapshotContext
   {
      public abstract Task<TSnapshot> MapToSnapshot(TModel model, TModelContext context);

      public sealed override Task<TSnapshot> MapToSnapshot(TModel model)
      {
         return Task.FromException<TSnapshot>(new ModelMapToSnapshotNotSupportedException<TSnapshot, TModelContext>());
      }
   }

   public abstract class SnapshotMapperBase<TModel, TSnapshot> : SnapshotMapperBase<TModel, TSnapshot, SnapshotContext> where TSnapshot : new()
   {
   }
}