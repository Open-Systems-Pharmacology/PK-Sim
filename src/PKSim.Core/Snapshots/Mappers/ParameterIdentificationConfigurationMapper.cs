using System.Threading.Tasks;
using OSPSuite.Core.Domain.ParameterIdentifications;
using ModelParameterIdentificationConfiguration =  OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentificationConfiguration;
using SnapshotParameterIdentificationConfiguration = PKSim.Core.Snapshots.ParameterIdentificationConfiguration;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterIdentificationConfigurationMapper : SnapshotMapperBase<ModelParameterIdentificationConfiguration, SnapshotParameterIdentificationConfiguration, ModelParameterIdentificationConfiguration>
   {
      private readonly ParameterIdentificationRunModeMapper _parameterIdentificationRunModeMapper;
      private readonly ParameterIdentificationAlgorithmMapper _parameterIdentificationAlgorithmMapper;

      public ParameterIdentificationConfigurationMapper(
         ParameterIdentificationRunModeMapper parameterIdentificationRunModeMapper,
         ParameterIdentificationAlgorithmMapper parameterIdentificationAlgorithmMapper)
      {
         _parameterIdentificationRunModeMapper = parameterIdentificationRunModeMapper;
         _parameterIdentificationAlgorithmMapper = parameterIdentificationAlgorithmMapper;
      }

      public override async Task<SnapshotParameterIdentificationConfiguration> MapToSnapshot(ModelParameterIdentificationConfiguration configuration)
      {
         var snapshot = await SnapshotFrom(configuration, x =>
         {
            x.LLOQMode = configuration.LLOQMode.Name;
            x.RemoveLLOQMode = configuration.RemoveLLOQMode.Name;
            x.CalculateJacobian = configuration.CalculateJacobian;
         });

         snapshot.RunMode = await _parameterIdentificationRunModeMapper.MapToSnapshot(configuration.RunMode);
         snapshot.Algorithm = await _parameterIdentificationAlgorithmMapper.MapToSnapshot(configuration.AlgorithmProperties);

         return snapshot;         
      }

      public override async Task<ModelParameterIdentificationConfiguration> MapToModel(SnapshotParameterIdentificationConfiguration snapshot, ModelParameterIdentificationConfiguration parameterIdentificationConfiguration)
      {
         parameterIdentificationConfiguration.LLOQMode = LLOQModes.ByName(snapshot.LLOQMode); 
         parameterIdentificationConfiguration.RemoveLLOQMode = RemoveLLOQModes.ByName(snapshot.RemoveLLOQMode);
         parameterIdentificationConfiguration.CalculateJacobian = snapshot.CalculateJacobian;

         parameterIdentificationConfiguration.AlgorithmProperties = await _parameterIdentificationAlgorithmMapper.MapToModel(snapshot.Algorithm);
         parameterIdentificationConfiguration.RunMode = await _parameterIdentificationRunModeMapper.MapToModel(snapshot.RunMode);

         return parameterIdentificationConfiguration;
      }
   }
}