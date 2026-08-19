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

   public ParameterValueMapper(IDimensionRepository dimensionRepository)
   {
      _dimensionRepository = dimensionRepository;
   }

   public override Task<SnapshotParameterValue> MapToSnapshot(ModelParameterValue parameterValue)
   {
      //dimension, unit and allowed range are carried so the value can be displayed and validated in the compound,
      //where the parameter itself cannot be resolved
      return SnapshotFrom(parameterValue, snapshot =>
      {
         snapshot.Path = parameterValue.Path.ToString();
         snapshot.Value = parameterValue.ConvertToDisplayUnit(parameterValue.Value);
         snapshot.Dimension = dimensionNameFor(parameterValue);
         snapshot.Unit = SnapshotValueFor(parameterValue.DisplayUnit.Name);
         updateAllowedRange(snapshot, parameterValue);
      });
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
