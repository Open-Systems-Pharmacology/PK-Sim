using System.Threading.Tasks;
using OSPSuite.Core.Domain.ParameterIdentifications;
using ModelParameterIdentificationConfiguration =  OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentificationConfiguration;
using SnapshotParameterIdentificationConfiguration = PKSim.Core.Snapshots.ParameterIdentificationConfiguration;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterIdentificationConfigurationMapper : SnapshotMapperBase<ModelParameterIdentificationConfiguration, SnapshotParameterIdentificationConfiguration>
   {
      private readonly ExtendedPropertyMapper _extendedPropertyMapper;

      public ParameterIdentificationConfigurationMapper(ExtendedPropertyMapper extendedPropertyMapper)
      {
         _extendedPropertyMapper = extendedPropertyMapper;
      }

      public override async Task<SnapshotParameterIdentificationConfiguration> MapToSnapshot(ModelParameterIdentificationConfiguration configuration)
      {
         var snapshot = await SnapshotFrom(configuration, x =>
         {
            x.LLOQMode = configuration.LLOQMode.Name;
            x.RemoveLLOQMode = configuration.RemoveLLOQMode.Name;
            x.CalculateJacobian = configuration.CalculateJacobian;
         });

         await mapAlgoritmProperties(snapshot, configuration.AlgorithmProperties);
         return snapshot;
         
      }

      private async Task mapAlgoritmProperties(SnapshotParameterIdentificationConfiguration snapshot, OptimizationAlgorithmProperties configurationAlgorithmProperties)
      {
         if (configurationAlgorithmProperties == null)
            return;

         snapshot.Name = configurationAlgorithmProperties.Name;
         snapshot.Properties = await _extendedPropertyMapper.MapToSnapshots(configurationAlgorithmProperties);
      }

      public override Task<ModelParameterIdentificationConfiguration> MapToModel(SnapshotParameterIdentificationConfiguration snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}