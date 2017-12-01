using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using PKSim.Assets;
using SnapshotParameter = PKSim.Core.Snapshots.Parameter;
using SnapshotTableFormula = PKSim.Core.Snapshots.TableFormula;
using ModelTableFormula = OSPSuite.Core.Domain.Formulas.TableFormula;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterMapper : ObjectBaseSnapshotMapperBase<IParameter, SnapshotParameter, IParameter>
   {
      private readonly TableFormulaMapper _tableFormulaMapper;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly ILogger _logger;

      public ParameterMapper(TableFormulaMapper tableFormulaMapper, IEntityPathResolver entityPathResolver, ILogger logger)
      {
         _tableFormulaMapper = tableFormulaMapper;
         _entityPathResolver = entityPathResolver;
         _logger = logger;
      }

      public override Task<SnapshotParameter> MapToSnapshot(IParameter modelParameter)
      {
         return createFrom<SnapshotParameter>(modelParameter, x => { x.Name = modelParameter.Name; });
      }

      public virtual async Task UpdateSnapshotFromParameter(SnapshotParameter snapshot, IParameter parameter)
      {
         updateParameterValue(snapshot, parameter.Value, parameter.DisplayUnit.Name, parameter.Dimension);
         snapshot.ValueDescription = SnapshotValueFor(parameter.ValueDescription);
         snapshot.TableFormula = await mapFormula(parameter.Formula);
      }

      public virtual SnapshotParameter ParameterFrom(double? parameterBaseValue, string parameterDisplayUnit, IDimension dimension)
      {
         if (parameterBaseValue == null)
            return null;

         var snapshot = new SnapshotParameter();
         updateParameterValue(snapshot, parameterBaseValue.Value, parameterDisplayUnit, dimension);
         return snapshot;
      }

      private void updateParameterValue(SnapshotParameter snapshot, double parameterBaseValue, string parameterDisplayUnit, IDimension dimension)
      {
         var unit = dimension.UnitOrDefault(parameterDisplayUnit);
         snapshot.Unit = SnapshotValueFor(unit.Name);
         snapshot.Value = dimension.BaseUnitValueToUnitValue(unit, parameterBaseValue);
      }

      private async Task<TSnapshotParameter> createFrom<TSnapshotParameter>(IParameter parameter, Action<TSnapshotParameter> configurationAction) where TSnapshotParameter : SnapshotParameter, new()
      {
         var snapshot = new TSnapshotParameter();
         await UpdateSnapshotFromParameter(snapshot, parameter);
         configurationAction(snapshot);
         return snapshot;
      }

      public override async Task<IParameter> MapToModel(SnapshotParameter snapshot, IParameter parameter)
      {
         parameter.ValueDescription = snapshot.ValueDescription;

         //only update formula if required
         if (snapshot.TableFormula != null)
            parameter.Formula = await _tableFormulaMapper.MapToModel(snapshot.TableFormula);

         if (snapshot.Value == null)
            return parameter;

         var displayUnit = ModelValueFor(snapshot.Unit);
         if (!parameter.Dimension.HasUnit(displayUnit))
            _logger.AddWarning(PKSimConstants.Warning.UnitNotFoundInDimensionForParameter(displayUnit, parameter.Dimension.Name, parameter.Name));

         parameter.DisplayUnit = parameter.Dimension.UnitOrDefault(displayUnit);

         //This needs to come AFTER formula update so that the base value is accurate
         var baseValue = parameter.Value;
         var snapshotValueInBaseUnit = parameter.ConvertToBaseUnit(snapshot.Value);

         if (!ValueComparer.AreValuesEqual(baseValue, snapshotValueInBaseUnit))
            parameter.Value = snapshotValueInBaseUnit;

         return parameter;
      }

      private async Task<SnapshotTableFormula> mapFormula(IFormula formula)
      {
         if (!(formula is ModelTableFormula tableFormula))
            return null;

         return await _tableFormulaMapper.MapToSnapshot(tableFormula);
      }

      public virtual Task<LocalizedParameter> LocalizedParameterFrom(IParameter parameter)
      {
         return LocalizedParameterFrom(parameter, _entityPathResolver.PathFor);
      }

      public virtual Task<LocalizedParameter> LocalizedParameterFrom(IParameter parameter, Func<IParameter, string> pathResolverFunc)
      {
         return createFrom<LocalizedParameter>(parameter, x => { x.Path = pathResolverFunc(parameter); });
      }

      public virtual Task<LocalizedParameter[]> LocalizedParametersFrom(IEnumerable<IParameter> parameters) => orderByPath(SnapshotMapperBaseExtensions.MapTo(parameters, LocalizedParameterFrom));

      public virtual Task<LocalizedParameter[]> LocalizedParametersFrom(IEnumerable<IParameter> parameters, Func<IParameter, string> pathResolverFunc)
      {
         return orderByPath(SnapshotMapperBaseExtensions.MapTo(parameters, x => LocalizedParameterFrom(x, pathResolverFunc)));
      }

      private async Task<LocalizedParameter[]> orderByPath(Task<LocalizedParameter[]> localizedParametersTask)
      {
         var localizedParameters = await localizedParametersTask;
         return localizedParameters?.OrderBy(x => x.Path).ToArray();
      }

      public virtual Task MapLocalizedParameters(IReadOnlyList<LocalizedParameter> localizedParameters, IContainer container)
      {
         if (localizedParameters == null || !localizedParameters.Any())
            return Task.FromResult(false);

         var allParameters = new PathCache<IParameter>(_entityPathResolver).For(container.GetAllChildren<IParameter>());
         return mapParameters(localizedParameters, x => allParameters[x.Path], x => x.Path, container.Name);
      }

      public virtual Task MapParameters(IReadOnlyList<SnapshotParameter> snapshots, IContainer container, string containerDescriptor)
      {
         return mapParameters(snapshots, x => container.Parameter(x.Name), x => x.Name, containerDescriptor);
      }

      private Task mapParameters<T>(IReadOnlyList<T> snapshots, Func<T, IParameter> parameterRetrieverFunc, Func<T, string> parameterIdentifierFunc, string containerDescriptor) where T : SnapshotParameter
      {
         if (snapshots == null || !snapshots.Any())
            return Task.FromResult(false);

         var tasks = new List<Task>();

         foreach (var snapshot in snapshots)
         {
            var parameter = parameterRetrieverFunc(snapshot);

            if (parameter == null)
               _logger.AddWarning(PKSimConstants.Error.SnapshotParameterNotFoundInContainer(parameterIdentifierFunc(snapshot), containerDescriptor));
            else
               tasks.Add(MapToModel(snapshot, parameter));
         }

         return Task.WhenAll(tasks);
      }
   }
}