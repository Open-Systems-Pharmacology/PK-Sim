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
      ///    Maps a list of snapshot to the corresponding model arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TModel[]> MapToModels<TModel, TSnapshot, TSnapshotContext>(this SnapshotMapperBase<TModel, TSnapshot, TSnapshotContext> mapper, IEnumerable<TSnapshot> snapshots, TSnapshotContext snapshotContext) 
         where TSnapshot : new() 
         where TSnapshotContext : SnapshotContext
      {
         return MapToModels(mapper, snapshots, s => mapper.MapToModel(s, snapshotContext));
      }

      /// <summary>
      ///    Maps a list of snapshot to the corresponding model arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TModel[]> MapToModels<TModel, TSnapshot, TSnapshotContext>(this SnapshotMapperBase<TModel, TSnapshot, TSnapshotContext> mapper, IEnumerable<TSnapshot> snapshots, Func<TSnapshot, Task<TModel>> mapToModelFunc) 
         where TSnapshot : new() 
         where TSnapshotContext : SnapshotContext
      {
         return MapTo(snapshots, mapToModelFunc);
      }

      /// <summary>
      ///    Maps a list of snapshot to the corresponding model arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TModel[]> MapToModels<TModel, TSnapshot, TSnapshotContext>(this ObjectBaseSnapshotMapperBase<TModel, TSnapshot, TSnapshotContext> mapper, IEnumerable<TSnapshot> snapshots, TSnapshotContext context) 
         where TSnapshot : IWithName, IWithDescription, new()
         where TModel : IWithName, IWithDescription 
         where TSnapshotContext : SnapshotContext
      {
         return MapToModels(mapper, snapshots, s => mapper.MapToModel(s,  context));
      }

      /// <summary>
      ///    Maps a list of snapshot to the corresponding model arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TModel[]> MapToModels<TModel, TSnapshot, TSnapshotContext>(this ParameterContainerSnapshotMapperBase<TModel, TSnapshot, TSnapshotContext> mapper, IEnumerable<TSnapshot> snapshots, TSnapshotContext context) 
         where TSnapshot : ParameterContainerSnapshotBase, new()
         where TModel : IContainer 
         where TSnapshotContext : SnapshotContext
      {
         return MapToModels(mapper, snapshots, s => mapper.MapToModel(s, context));
      }

      /// <summary>
      ///    Maps a list of models to the corresponding snapshot arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TSnapshot[]> MapToSnapshots<TModel, TSnapshot, TSnapshotContext>(this SnapshotMapperBase<TModel, TSnapshot, TSnapshotContext> mapper, IEnumerable<TModel> models) where TSnapshot : new()
         where TSnapshotContext : SnapshotContext
      {
         return MapTo(models, mapper.MapToSnapshot);
      }

      /// <summary>
      ///    Maps a list of models to the corresponding snapshot arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TSnapshot[]> MapToSnapshots<TModel, TSnapshot, TSnapshotContext, TModelContext>(this SnapshotMapperBase<TModel, TSnapshot,  TSnapshotContext, TModelContext> mapper, IEnumerable<TModel> models, TModelContext modelContext) 
         where TSnapshot : new() 
         where TSnapshotContext : SnapshotContext
      {
         return MapTo(models, m => mapper.MapToSnapshot(m, modelContext));
      }

      /// <summary>
      ///    Maps a list of models to the corresponding snapshot arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TSnapshot[]> MapToSnapshots<TModel, TSnapshot, TModelContext, TSnapshotContext>(this ObjectBaseSnapshotMapperBase<TModel, TSnapshot,  TSnapshotContext, TModelContext> mapper, IEnumerable<TModel> models, TModelContext modelContext)
         where TSnapshot : IWithName, IWithDescription, new() 
         where TModel : IWithName, IWithDescription 
         where TModelContext : SnapshotContext 
         where TSnapshotContext : SnapshotContext
      {
         return MapTo(models, m => mapper.MapToSnapshot(m, modelContext));
      }

      /// <summary>
      ///    Maps a list of models to the corresponding snapshot arrays. If the list if null or empty, null will be returned
      /// </summary>
      public static Task<TSnapshot[]> MapToSnapshots<TModel, TSnapshot, TModelContext, TSnapshotContext>(this ParameterContainerSnapshotMapperBase<TModel, TSnapshot, TSnapshotContext, TModelContext> mapper, IEnumerable<TModel> models, TModelContext modelContext)
         where TSnapshot : ParameterContainerSnapshotBase, new() 
         where TModel : IContainer 
         where TSnapshotContext : SnapshotContext
      {
         return MapTo(models, m => mapper.MapToSnapshot(m,  modelContext));
      }

      public static async Task<TTarget[]> MapTo<TSource, TTarget>(IEnumerable<TSource> sources, Func<TSource, Task<TTarget>> mapToFunc)
      {
         var list = sources?.ToList();

         if (list == null || !list.Any())
            return null;

         var targets = (await Task.WhenAll(list.Select(mapToFunc))).Where(x => x != null).ToArray();

         return !targets.Any() ? null : targets;
      }
   }
}