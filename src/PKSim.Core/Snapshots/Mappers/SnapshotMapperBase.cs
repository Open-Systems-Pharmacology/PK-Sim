using System;
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
      public virtual object MapToSnapshot(object model) => MapToSnapshot(model.DowncastTo<TModel>());

      public virtual object MapToModel(object snapshot) => MapToModel(snapshot.DowncastTo<TSnapshot>());

      public abstract TSnapshot MapToSnapshot(TModel model);

      public abstract TModel MapToModel(TSnapshot snapshot);

      public Type SnapshotTypeFor<T>() => typeof(TSnapshot);

      public virtual bool IsSatisfiedBy(Type item)
      {
         return item.IsAnImplementationOf<TModel>() || item.IsAnImplementationOf<TSnapshot>();
      }

      protected string SnapshotValueFor(string value) => !string.IsNullOrEmpty(value) ? value : null;

      protected string UnitValueFor(string unit) => unit ?? "";

      protected virtual TSnapshot SnapshotFrom(TModel model, Action<TSnapshot> configurationAction = null)
      {
         var snapshot = new TSnapshot();
         configurationAction?.Invoke(snapshot);
         return snapshot;
      }
   }

   public abstract class SnapshotMapperBase<TModel, TSnapshot, TContext> : SnapshotMapperBase<TModel, TSnapshot>
      where TSnapshot : new()
   {
      public abstract TModel MapToModel(TSnapshot snapshot, TContext context);

      public override TModel MapToModel(TSnapshot snapshot)
      {
         throw new SnapshotMapToModelNotSupportedNotSupportedException<TModel, TContext>();
      }
   }
}