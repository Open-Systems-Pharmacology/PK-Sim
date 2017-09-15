using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using SnapshotParameter = PKSim.Core.Snapshots.Parameter;
using SnapshotTableFormula = PKSim.Core.Snapshots.TableFormula;
using ModelTableFormula = OSPSuite.Core.Domain.Formulas.TableFormula;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterMapper : ObjectBaseSnapshotMapperBase<IParameter, SnapshotParameter, IParameter>
   {
      private readonly TableFormulaMapper _tableFormulaMapper;
      private readonly IEntityPathResolver _entityPathResolver;

      public ParameterMapper(TableFormulaMapper tableFormulaMapper, IEntityPathResolver entityPathResolver)
      {
         _tableFormulaMapper = tableFormulaMapper;
         _entityPathResolver = entityPathResolver;
      }

      public override Task<SnapshotParameter> MapToSnapshot(IParameter modelParameter)
      {
         return createFrom<SnapshotParameter>(modelParameter, x => { x.Name = modelParameter.Name; });
      }

      private async Task<TSnapshotParameter> createFrom<TSnapshotParameter>(IParameter modelParameter, Action<TSnapshotParameter> configurationAction) where TSnapshotParameter : SnapshotParameter, new()
      {
         var parameter = new TSnapshotParameter
         {
            Value = modelParameter.ValueInDisplayUnit,
            Unit = SnapshotValueFor(modelParameter.DisplayUnit.Name),
            ValueDescription = SnapshotValueFor(modelParameter.ValueDescription),
            TableFormula = await mapFormula(modelParameter.Formula)
         };
         configurationAction(parameter);
         return parameter;
      }

      public override async Task<IParameter> MapToModel(SnapshotParameter snapshot, IParameter parameter)
      {
         parameter.ValueDescription = snapshot.ValueDescription;
         parameter.DisplayUnit = parameter.Dimension.Unit(UnitValueFor(snapshot.Unit));

         //only update formula if required
         if (snapshot.TableFormula != null)
            parameter.Formula = await _tableFormulaMapper.MapToModel(snapshot.TableFormula);

         //This needs to come AFTER formula update so that the base value is accurate
         var baseValue = parameter.Value;
         var snapshotValueInBaseUnit = parameter.ConvertToBaseUnit(snapshot.Value);

         if (!ValueComparer.AreValuesEqual(baseValue, snapshotValueInBaseUnit))
            parameter.Value = snapshotValueInBaseUnit;

         return parameter;
      }

      private Task<SnapshotTableFormula> mapFormula(IFormula formula)
      {
         var tableFormula = formula as ModelTableFormula;
         if (tableFormula == null)
            return Task.FromResult<SnapshotTableFormula>(null);

         return _tableFormulaMapper.MapToSnapshot(tableFormula);
      }

      public virtual SnapshotParameter ParameterFrom(double? parameterBaseValue, string parameterDisplayUnit, IDimension dimension)
      {
         if (parameterBaseValue == null)
            return null;

         var displayUnitToUse = parameterDisplayUnit ?? dimension.BaseUnit.Name;

         return new SnapshotParameter
         {
            Value = dimension.BaseUnitValueToUnitValue(dimension.Unit(displayUnitToUse), parameterBaseValue.Value),
            Unit = displayUnitToUse
         };
      }

      public virtual Task<LocalizedParameter> LocalizedParameterFrom(IParameter parameter)
      {
         return LocalizedParameterFrom(parameter, _entityPathResolver.PathFor);
      }

      public virtual Task<LocalizedParameter> LocalizedParameterFrom(IParameter parameter, Func<IParameter, string> pathResolverFunc)
      {
         return createFrom<LocalizedParameter>(parameter, x => { x.Path = pathResolverFunc(parameter); });
      }

      public Task<LocalizedParameter[]> LocalizedParametersFrom(IEnumerable<IParameter> parameters)
      {
         var tasks = parameters.Select(LocalizedParameterFrom);
         return Task.WhenAll(tasks);
      }

      public Task<LocalizedParameter[]> LocalizedParametersFrom(IEnumerable<IParameter> parameters, Func<IParameter, string> pathResolverFunc)
      {
         var tasks = parameters.Select(x => LocalizedParameterFrom(x, pathResolverFunc));
         return Task.WhenAll(tasks);
      }
   }
}