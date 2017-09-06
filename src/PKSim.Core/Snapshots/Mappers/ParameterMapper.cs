using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using SnapshotParameter = PKSim.Core.Snapshots.Parameter;
using SnapshotTableFormula = PKSim.Core.Snapshots.TableFormula;
using ModelTableFormula = OSPSuite.Core.Domain.Formulas.TableFormula;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterMapper : ObjectBaseSnapshotMapperBase<IParameter, SnapshotParameter>
   {
      private readonly TableFormulaMapper _tableFormulaMapper;
      private readonly IEntityPathResolver _entityPathResolver;

      public ParameterMapper(TableFormulaMapper tableFormulaMapper, IEntityPathResolver entityPathResolver)
      {
         _tableFormulaMapper = tableFormulaMapper;
         _entityPathResolver = entityPathResolver;
      }

      public override SnapshotParameter MapToSnapshot(IParameter modelParameter)
      {
         return createFrom<SnapshotParameter>(modelParameter, x => { x.Name = modelParameter.Name; });
      }

      private TSnapshotParameter createFrom<TSnapshotParameter>(IParameter modelParameter, Action<TSnapshotParameter> configurationAction) where TSnapshotParameter : SnapshotParameter, new()
      {
         var parameter = new TSnapshotParameter
         {
            Value = modelParameter.ValueInDisplayUnit,
            Unit = SnapshotValueFor(modelParameter.DisplayUnit.Name),
            ValueDescription = SnapshotValueFor(modelParameter.ValueDescription),
            TableFormula = mapFormula(modelParameter.Formula)
         };
         configurationAction(parameter);
         return parameter;
      }

      public override IParameter MapToModel(SnapshotParameter snapshot)
      {
         throw new NotSupportedException("Parameter should not be created from snapshot. Instead use the update method");
      }

      public virtual void UpdateParameterFromSnapshot(IParameter parameter, SnapshotParameter snapshotParameter)
      {
         parameter.ValueDescription = snapshotParameter.ValueDescription;
         parameter.DisplayUnit = parameter.Dimension.Unit(UnitValueFor(snapshotParameter.Unit));

         //only update formula if required
         if (snapshotParameter.TableFormula != null)
            parameter.Formula = _tableFormulaMapper.MapToModel(snapshotParameter.TableFormula);

         //This needs to come AFTER formula update so that the base value is accurate
         var baseValue = parameter.Value;
         var snapshotValueInBaseUnit = parameter.ConvertToBaseUnit(snapshotParameter.Value);

         if (!ValueComparer.AreValuesEqual(baseValue, snapshotValueInBaseUnit))
            parameter.Value = snapshotValueInBaseUnit;
      }

      private SnapshotTableFormula mapFormula(IFormula formula)
      {
         var tableFormula = formula as ModelTableFormula;
         return tableFormula == null ? null : _tableFormulaMapper.MapToSnapshot(tableFormula);
      }

      public virtual SnapshotParameter ParameterFrom(double? parameterBaseValue, string parameterDisplayUnit, IDimension dimension)
      {
         if (parameterBaseValue == null)
            return null;

         return new SnapshotParameter
         {
            Value = dimension.BaseUnitValueToUnitValue(dimension.Unit(parameterDisplayUnit), parameterBaseValue.Value),
            Unit = parameterDisplayUnit
         };
      }

      public virtual LocalizedParameter LocalizedParameterFrom(IParameter parameter)
      {
         return LocalizedParameterFrom(parameter, _entityPathResolver.PathFor);
      }

      public virtual LocalizedParameter LocalizedParameterFrom(IParameter parameter, Func<IParameter, string> pathResolverFunc)
      {
         return createFrom<LocalizedParameter>(parameter, x => { x.Path = pathResolverFunc(parameter); });
      }

      public List<LocalizedParameter> LocalizedParametersFrom(IEnumerable<IParameter> parameters)
      {
         return parameters.Select(LocalizedParameterFrom).ToList();
      }

      public List<LocalizedParameter> LocalizedParametersFrom(IEnumerable<IParameter> parameters, Func<IParameter, string> pathResolverFunc)
      {
         return parameters.Select(x => LocalizedParameterFrom(x, pathResolverFunc)).ToList();
      }
   }
}