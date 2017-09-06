using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers
{
   public interface ISnapshotMapperSpecification : ISnapshotMapper, ISpecification<Type>
   {
   }

   public abstract class SnapshotMapperBase<TModel, TSnapshot> : ISnapshotMapperSpecification
      where TSnapshot : ISnapshot, new()
      where TModel : IObjectBase
   {
      public virtual object MapToSnapshot(object model) => MapToSnapshot(model.DowncastTo<TModel>());

      public virtual object MapToModel(object snapshot) => MapToModel(snapshot.DowncastTo<TSnapshot>());

      public Type SnapshotTypeFor<T>() => typeof(TSnapshot);

      public abstract TSnapshot MapToSnapshot(TModel model);

      public abstract TModel MapToModel(TSnapshot snapshot);

      public virtual bool IsSatisfiedBy(Type item)
      {
         return item.IsAnImplementationOf<TModel>() || item.IsAnImplementationOf<TSnapshot>();
      }

      protected void MapModelPropertiesToSnapshot(TModel model, TSnapshot snapshot)
      {
         snapshot.Name = model.Name;
         snapshot.Description = SnapshotValueFor(model.Description);
      }

      protected void MapSnapshotPropertiesToModel(TSnapshot snapshot, TModel model)
      {
         model.Name = snapshot.Name;
         model.Description = snapshot.Description;
      }

      protected string SnapshotValueFor(string value) => !string.IsNullOrEmpty(value) ? value : null;

      protected string UnitValueFor(string unit) => unit ?? "";

      protected virtual TSnapshot SnapshotFrom(TModel model, Action<TSnapshot> configurationAction = null)
      {
         var snapshot = new TSnapshot();
         MapModelPropertiesToSnapshot(model, snapshot);
         configurationAction?.Invoke(snapshot);
         return snapshot;
      }
   }

   public abstract class ParameterContainerSnapshotMapperBase<TModel, TSnapshot> : SnapshotMapperBase<TModel, TSnapshot>
      where TModel : IContainer
      where TSnapshot : ParameterContainerSnapshotBase, new()
   {
      protected readonly ParameterMapper _parameterMapper;

      protected ParameterContainerSnapshotMapperBase(ParameterMapper parameterMapper)
      {
         _parameterMapper = parameterMapper;
      }

      protected Parameter ParameterSnapshotFor(IParameter parameter) => _parameterMapper.MapToSnapshot(parameter);

      protected void MapParameters(IEnumerable<IParameter> parameters, TSnapshot snapshot)
      {
         snapshot.Parameters.AddRange(parameters.Select(ParameterSnapshotFor));
      }

      protected override TSnapshot SnapshotFrom(TModel model, Action<TSnapshot> configurationAction = null)
      {
         return base.SnapshotFrom(model, snapshot =>
         {
            MapModelParameters(model, snapshot);
            configurationAction?.Invoke(snapshot);
         });
      }

      protected virtual void MapModelParameters(TModel model, TSnapshot snapshot)
      {
         MapParameters(model.AllParameters(ParameterHasChanged), snapshot);
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