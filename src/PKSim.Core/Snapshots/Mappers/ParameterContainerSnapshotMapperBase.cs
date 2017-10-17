using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers
{
   public abstract class ParameterContainerSnapshotMapperBase<TModel, TSnapshot> : ObjectBaseSnapshotMapperBase<TModel, TSnapshot>
      where TModel : IContainer
      where TSnapshot : ParameterContainerSnapshotBase, new()
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
         return AddParametersToSnapshot(model.AllParameters(x => x.ParameterHasChanged()), snapshot);
      }

      protected Task UpdateParametersFromSnapshot(TSnapshot snapshot, IContainer container, string containerDescriptor = null)
      {
         return _parameterMapper.MapParameters(snapshot.Parameters, container, containerDescriptor ?? container.Name);
      }
   }

   public abstract class ParameterContainerSnapshotMapperBase<TModel, TSnapshot, TContext> : ParameterContainerSnapshotMapperBase<TModel, TSnapshot>
      where TModel : IContainer
      where TSnapshot : ParameterContainerSnapshotBase, new()
   {
      protected ParameterContainerSnapshotMapperBase(ParameterMapper parameterMapper) : base(parameterMapper)
      {
      }

      public abstract Task<TModel> MapToModel(TSnapshot snapshot, TContext context);

      public override Task<TModel> MapToModel(TSnapshot snapshot)
      {
         return Task.FromException<TModel>(new SnapshotMapToModelNotSupportedException<TModel, TContext>());
      }
   }
}