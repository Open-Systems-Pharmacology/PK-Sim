using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using static OSPSuite.Core.Extensions.SnapshotMapperBaseExtensions;

namespace PKSim.Core.Snapshots.Mappers;

/// <summary>
///    Extensions instead of method in base class to simplify testing
/// </summary>
public static class SnapshotMapperBaseExtensions
{
   /// <summary>
   ///    Maps a list of snapshot to the corresponding model arrays. If the list if null or empty, null will be returned
   /// </summary>
   public static Task<TModel[]> MapToModels<TModel, TSnapshot, TSnapshotContext>(this ParameterContainerSnapshotMapperBase<TModel, TSnapshot, TSnapshotContext> mapper, IEnumerable<TSnapshot> snapshots, TSnapshotContext context)
      where TSnapshot : ParameterContainerSnapshotBase, new()
      where TModel : IContainer
      where TSnapshotContext : SnapshotContext
   {
      return mapper.MapToModels(snapshots, s => mapper.MapToModel(s, context));
   }

   /// <summary>
   ///    Maps a list of models to the corresponding snapshot arrays. If the list if null or empty, null will be returned
   /// </summary>
   public static Task<TSnapshot[]> MapToSnapshots<TModel, TSnapshot, TModelContext, TSnapshotContext>(this ParameterContainerSnapshotMapperBase<TModel, TSnapshot, TSnapshotContext, TModelContext> mapper, IEnumerable<TModel> models, TModelContext modelContext)
      where TSnapshot : ParameterContainerSnapshotBase, new()
      where TModel : IContainer
      where TSnapshotContext : SnapshotContext
   {
      return MapTo(models, m => mapper.MapToSnapshot(m, modelContext));
   }
}