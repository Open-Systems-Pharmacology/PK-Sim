using System;
using System.Collections.Generic;
using System.Linq;
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

      protected Parameter ParameterSnapshotFor(IParameter parameter) => _parameterMapper.MapToSnapshot(parameter);

      protected void AddParametersToSnapshot(IEnumerable<IParameter> parameters, TSnapshot snapshot)
      {
         snapshot.Parameters.AddRange(parameters.Select(ParameterSnapshotFor));
      }

      protected override TSnapshot SnapshotFrom(TModel model, Action<TSnapshot> configurationAction = null)
      {
         return base.SnapshotFrom(model, snapshot =>
         {
            AddModelParametersToSnapshot(model, snapshot);
            configurationAction?.Invoke(snapshot);
         });
      }

      protected virtual void AddModelParametersToSnapshot(TModel model, TSnapshot snapshot)
      {
         AddParametersToSnapshot(model.AllParameters(ParameterHasChanged), snapshot);
      }

      protected virtual bool ParameterHasChanged(IParameter parameter)
      {
         //TODO Use ValueOrigin state when implemented. It should be != than PKSim default;
         var canBeEdited = parameter.Visible && parameter.Editable && parameter.Formula.IsConstant();
         return parameter.ValueDiffersFromDefault() || canBeEdited && parameter.Value != 0;
      }

      protected void UpdateParametersFromSnapshot(IContainer container, TSnapshot snapshot, string containerDesciptor)
      {
         foreach (var snapshotParameter in snapshot.Parameters)
         {
            var modelParameter = container.Parameter(snapshotParameter.Name);

            if (modelParameter == null)
               throw new SnapshotParameterNotFoundException(snapshotParameter.Name, containerDesciptor);

            _parameterMapper.UpdateParameterFromSnapshot(modelParameter, snapshotParameter);
         }
      }
   }
}