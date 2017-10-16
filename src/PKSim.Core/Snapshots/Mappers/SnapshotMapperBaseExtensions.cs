using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots.Mappers
{
   /// <summary>
   ///    Extensions instead of method in base class to simplify testing
   /// </summary>
   public static class SnapshotMapperBaseExtensions
   {
      /// <summary>
      ///    Maps a list of models to the corresponding snapshot arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TSnapshot[]> MapToSnapshots<TModel, TSnapshot>(this SnapshotMapperBase<TModel, TSnapshot> mapper, IEnumerable<TModel> models) where TSnapshot : new()
      {
         return MapToSnapshots(mapper, models, mapper.MapToSnapshot);
      }

      /// <summary>
      ///    Maps a list of models to the corresponding snapshot arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TSnapshot[]> MapToSnapshots<TModel, TSnapshot>(this SnapshotMapperBase<TModel, TSnapshot> mapper, IEnumerable<TModel> models, Func<TModel, Task<TSnapshot>> mapToSnapshotFunc) where TSnapshot : new()
      {
         return MapTo(models, mapToSnapshotFunc);
      }

      /// <summary>
      ///    Maps a list of snapshot to the corresponding model arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TModel[]> MapToModels<TModel, TSnapshot>(this SnapshotMapperBase<TModel, TSnapshot> mapper, IEnumerable<TSnapshot> snapshots) where TSnapshot : new()
      {
         return MapToModels(mapper, snapshots, mapper.MapToModel);
      }

      /// <summary>
      ///    Maps a list of snapshot to the corresponding model arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TModel[]> MapToModels<TModel, TSnapshot>(this SnapshotMapperBase<TModel, TSnapshot> mapper, IEnumerable<TSnapshot> snapshots, Func<TSnapshot, Task<TModel>> mapToModelFunc) where TSnapshot : new()
      {
         return MapTo(snapshots, mapToModelFunc);
      }

      /// <summary>
      ///    Maps a list of snapshot to the corresponding model arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TModel[]> MapToModels<TModel, TSnapshot, TContext>(this SnapshotMapperBase<TModel, TSnapshot, TContext> mapper, IEnumerable<TSnapshot> snapshots, TContext context) where TSnapshot : new()
      {
         return MapToModels(mapper, snapshots, s => mapper.MapToModel(s, context));
      }

      /// <summary>
      ///    Maps a list of snapshot to the corresponding model arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TModel[]> MapToModels<TModel, TSnapshot, TContext>(this ObjectBaseSnapshotMapperBase<TModel, TSnapshot, TContext> mapper, IEnumerable<TSnapshot> snapshots, TContext context) where TSnapshot : IWithName, IWithDescription, new() where TModel : IWithName, IWithDescription
      {
         return MapToModels(mapper, snapshots, s => mapper.MapToModel(s, context));
      }

      /// <summary>
      ///    Maps a list of snapshot to the corresponding model arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TModel[]> MapToModels<TModel, TSnapshot, TContext>(this ParameterContainerSnapshotMapperBase<TModel, TSnapshot, TContext> mapper, IEnumerable<TSnapshot> snapshots, TContext context) where TSnapshot : ParameterContainerSnapshotBase, new() where TModel : IContainer
      {
         return MapToModels(mapper, snapshots, s => mapper.MapToModel(s, context));
      }

      /// <summary>
      ///    Maps a list of models to the corresponding snapshot arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TSnapshot[]> MapToSnapshots<TModel, TSnapshot, TModelContext, TSnapshotContext>(this SnapshotMapperBase<TModel, TSnapshot, TModelContext, TSnapshotContext> mapper, IEnumerable<TModel> models, TSnapshotContext snapshotContext) where TSnapshot : new()
      {
         return MapToSnapshots(mapper, models, m => mapper.MapToSnapshot(m, snapshotContext));
      }

      /// <summary>
      ///    Maps a list of models to the corresponding snapshot arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TSnapshot[]> MapToSnapshots<TModel, TSnapshot, TModelContext, TSnapshotContext>(this ObjectBaseSnapshotMapperBase<TModel, TSnapshot, TModelContext, TSnapshotContext> mapper, IEnumerable<TModel> models, TSnapshotContext snapshotContext) where TSnapshot : IWithName, IWithDescription, new() where TModel : IWithName, IWithDescription
      {
         return MapToSnapshots(mapper, models, m => mapper.MapToSnapshot(m, snapshotContext));
      }

      public static async Task<TTarget[]> MapTo<TSource, TTarget>(IEnumerable<TSource> sources, Func<TSource, Task<TTarget>> mapToFunc)
      {
         var list = sources?.ToList();

         if (list == null || !list.Any())
            return null;

         var targets = await Task.WhenAll(list.Select(mapToFunc));
         return targets.Where(x => x != null).ToArray();
      }
   }
}