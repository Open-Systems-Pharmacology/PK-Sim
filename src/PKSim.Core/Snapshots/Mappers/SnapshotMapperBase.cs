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

      protected T ModelValueFor<T>(T? snapshotValue, T defaultValue = default(T)) where T:struct
      {
         return snapshotValue.GetValueOrDefault(defaultValue);
      }

      protected virtual Task<TSnapshot> SnapshotFrom(TModel model, Action<TSnapshot> configurationAction = null)
      {
         var snapshot = new TSnapshot();
         configurationAction?.Invoke(snapshot);
         return Task.FromResult(snapshot);
      }

      public static Task<TResult> FromException<TResult>(Exception exc)
      {
         var tcs = new TaskCompletionSource<TResult>();
         tcs.SetException(exc);
         return tcs.Task;
      }

      public static Task FromException(Exception exc)
      {
         var tcs = new TaskCompletionSource<object>(null);
         tcs.SetException(exc);
         return tcs.Task;
      }

      /// <summary>
      ///    Maps a list of models to the corresponding snapshot arrays. If the list if null or empty, null will be returned
      /// </summary>
      public virtual Task<TSnapshot[]> MapToSnapshots(IEnumerable<TModel> models) => MapToSnapshots(models, MapToSnapshot);

      protected virtual Task<TSnapshot[]> MapToSnapshots(IEnumerable<TModel> models, Func<TModel, Task<TSnapshot>> mapToSnapshotFunc) => MapTo(models, mapToSnapshotFunc);

      public virtual Task<TModel[]> MapToModels(IEnumerable<TSnapshot> snapshots) => MapToModels(snapshots, MapToModel);

      protected virtual Task<TModel[]> MapToModels(IEnumerable<TSnapshot> snapshots, Func<TSnapshot, Task<TModel>> mapToModelFunc) => MapTo(snapshots, mapToModelFunc);

      protected virtual Task<TTarget[]> MapTo<TSource, TTarget>(IEnumerable<TSource> sources, Func<TSource, Task<TTarget>> mapToFunc)
      {
         var list = sources?.ToList();

         if (list == null || !list.Any())
            return Task.FromResult<TTarget[]>(null);

         return Task.WhenAll(list.Select(mapToFunc));
      }
   }

   public abstract class SnapshotMapperBase<TModel, TSnapshot, TContext> : SnapshotMapperBase<TModel, TSnapshot>
      where TSnapshot : new()
   {
      public abstract Task<TModel> MapToModel(TSnapshot snapshot, TContext context);

      public virtual Task<TModel[]> MapToModels(IEnumerable<TSnapshot> snapshots, TContext context) => MapToModels(snapshots, s => MapToModel(s, context));

      public sealed override Task<TModel> MapToModel(TSnapshot snapshot)
      {
         return FromException<TModel>(new SnapshotMapToModelNotSupportedException<TModel, TContext>());
      }
   }

   public abstract class SnapshotMapperBase<TModel, TSnapshot, TModelContext, TSnapshotContext> : SnapshotMapperBase<TModel, TSnapshot, TModelContext>
      where TSnapshot : new()
   {
      public abstract Task<TSnapshot> MapToSnapshot(TModel model, TSnapshotContext context);

      public virtual Task<TSnapshot[]> MapToSnapshots(IEnumerable<TModel> models, TSnapshotContext context) => MapToSnapshots(models, m => MapToSnapshot(m, context));

      public sealed override Task<TSnapshot> MapToSnapshot(TModel model)
      {
         return FromException<TSnapshot>(new ModelMapToSnapshotNotSupportedException<TSnapshot, TSnapshotContext>());
      }
   }
}