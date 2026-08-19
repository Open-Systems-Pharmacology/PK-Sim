using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Repositories;
using ModelParameterValue = OSPSuite.Core.Domain.Builder.ParameterValue;
using SnapshotParameterValue = PKSim.Core.Snapshots.ParameterValue;

namespace PKSim.Core.Snapshots.Mappers;

public class ParameterValueMapper : SnapshotMapperBase<ModelParameterValue, SnapshotParameterValue>
{
   private readonly IDimensionRepository _dimensionRepository;
   private readonly ValueOriginMapper _valueOriginMapper;

   public ParameterValueMapper(IDimensionRepository dimensionRepository, ValueOriginMapper valueOriginMapper)
   {
      _dimensionRepository = dimensionRepository;
      _valueOriginMapper = valueOriginMapper;
   }

   public override async Task<SnapshotParameterValue> MapToSnapshot(ModelParameterValue parameterValue)
   {
      //dimension, unit and allowed range are carried so the value can be displayed and validated in the compound,
      //where the parameter itself cannot be resolved
      var snapshot = await SnapshotFrom(parameterValue, x =>
      {
         x.Path = parameterValue.Path.ToString();
         x.Value = parameterValue.ConvertToDisplayUnit(parameterValue.Value);
         x.Dimension = dimensionNameFor(parameterValue);
         x.Unit = SnapshotValueFor(parameterValue.DisplayUnit.Name);
         updateAllowedRange(x, parameterValue);
      });

      snapshot.ValueOrigin = await _valueOriginMapper.MapToSnapshot(parameterValue.ValueOrigin);

      return snapshot;
   }

   public override Task<ModelParameterValue> MapToModel(SnapshotParameterValue snapshot, SnapshotContext snapshotContext)
   {
      var dimension = _dimensionRepository.DimensionByName(snapshot.Dimension);
      var displayUnit = dimension.UnitOrDefault(snapshot.Unit);

      var parameterValue = new ModelParameterValue
      {
         Path = snapshot.Path.ToObjectPath(),
         Dimension = dimension,
         DisplayUnit = displayUnit,
         Value = dimension.UnitValueToBaseUnitValue(displayUnit, snapshot.Value),
         Info = allowedRangeFrom(snapshot, dimension, displayUnit)
      };

      _valueOriginMapper.UpdateValueOrigin(parameterValue.ValueOrigin, snapshot.ValueOrigin);

      return Task.FromResult(parameterValue);
   }

   private static string dimensionNameFor(ModelParameterValue parameterValue) =>
      string.Equals(parameterValue.Dimension.Name, Constants.Dimension.DIMENSIONLESS) ? null : parameterValue.Dimension.Name;

   private void updateAllowedRange(SnapshotParameterValue snapshot, ModelParameterValue parameterValue)
   {
      var info = parameterValue.Info;
      if (info == null)
         return;

      snapshot.MinValue = valueInDisplayUnit(info.MinValue, parameterValue);
      snapshot.MaxValue = valueInDisplayUnit(info.MaxValue, parameterValue);
      snapshot.MinIsAllowed = info.MinValue.HasValue ? SnapshotValueFor(info.MinIsAllowed, true) : null;
      snapshot.MaxIsAllowed = info.MaxValue.HasValue ? SnapshotValueFor(info.MaxIsAllowed, true) : null;
   }

   private ParameterInfo allowedRangeFrom(SnapshotParameterValue snapshot, IDimension dimension, Unit displayUnit)
   {
      if (snapshot.MinValue == null && snapshot.MaxValue == null)
         return null;

      return new ParameterInfo
      {
         MinValue = valueInBaseUnit(snapshot.MinValue, dimension, displayUnit),
         MinIsAllowed = ModelValueFor(snapshot.MinIsAllowed, true),
         MaxValue = valueInBaseUnit(snapshot.MaxValue, dimension, displayUnit),
         MaxIsAllowed = ModelValueFor(snapshot.MaxIsAllowed, true)
      };
   }

   private static double? valueInDisplayUnit(double? valueInBaseUnit, ModelParameterValue parameterValue) =>
      valueInBaseUnit.HasValue ? parameterValue.ConvertToDisplayUnit(valueInBaseUnit) : null;

   private static double? valueInBaseUnit(double? valueInDisplayUnit, IDimension dimension, Unit displayUnit) =>
      valueInDisplayUnit.HasValue ? dimension.UnitValueToBaseUnitValue(displayUnit, valueInDisplayUnit.Value) : null;
}
