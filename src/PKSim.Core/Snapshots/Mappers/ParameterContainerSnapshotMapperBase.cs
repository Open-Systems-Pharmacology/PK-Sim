using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
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

      protected Task<Parameter> ParameterSnapshotFor(IParameter parameter) => _parameterMapper.MapToSnapshot(parameter);

      protected async Task AddParametersToSnapshot(IEnumerable<IParameter> parameters, TSnapshot snapshot)
      {
         var tasks = parameters.Select(ParameterSnapshotFor);
         snapshot.Parameters.AddRange(await Task.WhenAll(tasks));
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
         return AddParametersToSnapshot(model.AllParameters(ParameterHasChanged), snapshot);
      }

      protected virtual bool ParameterHasChanged(IParameter parameter)
      {
         //TODO Use ValueOrigin state when implemented. It should be != than PKSim default;
         var canBeEdited = parameter.Visible && parameter.Editable && parameter.Formula.IsConstant();
         return parameter.ValueDiffersFromDefault() || canBeEdited && parameter.Value != 0;
      }

      protected Task UpdateParametersFromSnapshot(TSnapshot snapshot, IContainer container, string containerDesciptor)
      {
         var tasks = new List<Task>();
         foreach (var snapshotParameter in snapshot.Parameters)
         {
            var modelParameter = container.Parameter(snapshotParameter.Name);

            if (modelParameter == null)
               return FromException(new SnapshotParameterNotFoundException(snapshotParameter.Name, containerDesciptor));

            tasks.Add(_parameterMapper.MapToModel(snapshotParameter, modelParameter));
         }

         return Task.WhenAll(tasks);
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
         return FromException<TModel>(new SnapshotMapToModelNotSupportedNotSupportedException<TModel, TContext>());
      }
   }
}