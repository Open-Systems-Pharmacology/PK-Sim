using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers
{
   public abstract class ParameterContainerSnapshotMapperBase<TModel, TSnapshot, TSnapshotContext> : ObjectBaseSnapshotMapperBase<TModel, TSnapshot, TSnapshotContext>
      where TModel : IContainer
      where TSnapshot : ParameterContainerSnapshotBase, new()
      where TSnapshotContext : SnapshotContext
   {
      protected readonly ParameterMapper _parameterMapper;

      protected ParameterContainerSnapshotMapperBase(ParameterMapper parameterMapper)
      {
         _parameterMapper = parameterMapper;
      }

      protected async Task AddParametersToSnapshot(IEnumerable<IParameter> parameters, TSnapshot snapshot)
      {
         snapshot.Parameters = await _parameterMapper.MapToSnapshots(parameters);
      }

      protected override Task<TSnapshot> SnapshotFrom(TModel model, Action<TSnapshot> configurationAction = null)
      {
         return base.SnapshotFrom(model, snapshot =>
         {
            AddModelParametersToSnapshot(model, snapshot);
            configurationAction?.Invoke(snapshot);
         });
      }

      protected virtual Task AddModelParametersToSnapshot(TModel model, TSnapshot snapshot)
      {
         return AddParametersToSnapshot(model.AllParameters(ShouldExportParameterToSnapshot), snapshot);
      }

      protected virtual bool ShouldExportParameterToSnapshot(IParameter parameter) => parameter.ShouldExportToSnapshot();

      protected Task UpdateParametersFromSnapshot(TSnapshot snapshot, IContainer container, SnapshotContext snapshotContext, string containerDescriptor = null)
      {
         return _parameterMapper.MapParameters(snapshot.Parameters, container, containerDescriptor ?? container.Name, snapshotContext);
      }
   }

   public abstract class ParameterContainerSnapshotMapperBase<TModel, TSnapshot, TSnapshotContext, TModelContext> :
      ParameterContainerSnapshotMapperBase<TModel, TSnapshot, TSnapshotContext>
      where TModel : IContainer
      where TSnapshot : ParameterContainerSnapshotBase, new() where TSnapshotContext : SnapshotContext
   {
      protected ParameterContainerSnapshotMapperBase(ParameterMapper parameterMapper) : base(parameterMapper)
      {
      }

      public abstract Task<TSnapshot> MapToSnapshot(TModel model, TModelContext context);

      public sealed override Task<TSnapshot> MapToSnapshot(TModel model)
      {
         return Task.FromException<TSnapshot>(new ModelMapToSnapshotNotSupportedException<TSnapshot, TModelContext>());
      }
   }

   public abstract class ParameterContainerSnapshotMapperBase<TModel, TSnapshot> : ParameterContainerSnapshotMapperBase<TModel, TSnapshot, SnapshotContext>
      where TModel : IContainer
      where TSnapshot : ParameterContainerSnapshotBase, new()
   {
      protected ParameterContainerSnapshotMapperBase(ParameterMapper parameterMapper) : base(parameterMapper)
      {
      }
   }
}