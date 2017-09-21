using System;
using PKSim.Assets;

namespace PKSim.Core.Snapshots
{
   public class SnapshotNotFoundException : Exception
   {
      public SnapshotNotFoundException(Type modelType) : base(PKSimConstants.Error.SnapshotNotFoundFor(modelType.FullName))
      {
      }
   }


   public class SnapshotMapToModelNotSupportedException<TModel, TContext> : NotSupportedException
   {
      public SnapshotMapToModelNotSupportedException() : base(PKSimConstants.Error.MapToModelNotSupportedWithoutContext(typeof(TModel).Name, typeof(TContext).Name))
      {
      }
   }

   public class ModelMapToSnapshotNotSupportedException<TSnapshot, TContext> : NotSupportedException
   {
      public ModelMapToSnapshotNotSupportedException() : base(PKSimConstants.Error.MapToSnapshotNotSupportedWithoutContext(typeof(TSnapshot).Name, typeof(TContext).Name))
      {
      }
   }
}