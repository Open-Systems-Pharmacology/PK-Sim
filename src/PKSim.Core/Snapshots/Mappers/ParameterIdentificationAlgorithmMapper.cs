using System;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterIdentificationAlgorithmMapper : SnapshotMapperBase<OptimizationAlgorithmProperties, OptimizationAlgorithm>
   {
      private readonly ExtendedPropertyMapper _extendedPropertyMapper;

      public ParameterIdentificationAlgorithmMapper(ExtendedPropertyMapper extendedPropertyMapper)
      {
         _extendedPropertyMapper = extendedPropertyMapper;
      }

      public override async Task<OptimizationAlgorithm> MapToSnapshot(OptimizationAlgorithmProperties algorithmProperties)
      {
         var snapshot = await SnapshotFrom(algorithmProperties, x => { x.Name = algorithmProperties.Name;});
         snapshot.Properties = await _extendedPropertyMapper.MapToSnapshots(algorithmProperties);
         snapshot.Properties?.Each(_extendedPropertyMapper.ClearDatabaseProperties);
         return snapshot;
      }

      public override Task<OptimizationAlgorithmProperties> MapToModel(OptimizationAlgorithm snapshot)
      {
         throw new NotImplementedException();
      }
   }
}