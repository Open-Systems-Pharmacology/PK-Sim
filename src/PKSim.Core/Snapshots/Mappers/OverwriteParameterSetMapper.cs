using System.Threading.Tasks;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility.Extensions;
using static OSPSuite.Core.Extensions.SnapshotMapperBaseExtensions;
using ModelOverwriteParameterSet = PKSim.Core.Model.OverwriteParameterSet;
using SnapshotOverwriteParameterSet = PKSim.Core.Snapshots.OverwriteParameterSet;

namespace PKSim.Core.Snapshots.Mappers;

public class OverwriteParameterSetMapper : SnapshotMapperBase<ModelOverwriteParameterSet, SnapshotOverwriteParameterSet>
{
   private readonly ParameterValueMapper _parameterValueMapper;
   private readonly ExtendedPropertyMapper _extendedPropertyMapper;

   public OverwriteParameterSetMapper(ParameterValueMapper parameterValueMapper, ExtendedPropertyMapper extendedPropertyMapper)
   {
      _parameterValueMapper = parameterValueMapper;
      _extendedPropertyMapper = extendedPropertyMapper;
   }

   public override async Task<SnapshotOverwriteParameterSet> MapToSnapshot(ModelOverwriteParameterSet model)
   {
      var snapshot = await SnapshotFrom(model, x =>
      {
         x.Name = model.Name;
         x.IsDefault = SnapshotValueFor(model.IsDefault, false);
      });

      snapshot.ExtendedProperties = await _extendedPropertyMapper.MapToSnapshots(model.ExtendedProperties.All);
      snapshot.ParameterValues = await _parameterValueMapper.MapToSnapshots(model.ParameterValues);

      return snapshot;
   }

   public override async Task<ModelOverwriteParameterSet> MapToModel(SnapshotOverwriteParameterSet snapshot, SnapshotContext snapshotContext)
   {
      var overwriteParameterSet = new ModelOverwriteParameterSet
      {
         Name = snapshot.Name,
         IsDefault = ModelValueFor(snapshot.IsDefault, false)
      };

      var extendedProperties = await _extendedPropertyMapper.MapToModels(snapshot.ExtendedProperties, snapshotContext);
      extendedProperties?.Each(overwriteParameterSet.ExtendedProperties.Add);

      var parameterValues = await _parameterValueMapper.MapToModels(snapshot.ParameterValues, snapshotContext);
      parameterValues?.Each(overwriteParameterSet.Add);

      return overwriteParameterSet;
   }
}