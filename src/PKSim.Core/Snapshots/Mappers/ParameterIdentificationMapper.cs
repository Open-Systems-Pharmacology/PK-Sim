using System;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using ModelParameterIdentification = OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentification;
using SnapshotParameterIdentification = PKSim.Core.Snapshots.ParameterIdentification;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterIdentificationMapper : ObjectBaseSnapshotMapperBase<ModelParameterIdentification, SnapshotParameterIdentification, PKSimProject, PKSimProject>
   {
      private readonly ParameterIdentificationConfigurationMapper _parameterIdentificationConfigurationMapper;
      private readonly OutputMappingMapper _outputMappingMapper;

      public ParameterIdentificationMapper(ParameterIdentificationConfigurationMapper parameterIdentificationConfigurationMapper, OutputMappingMapper outputMappingMapper)
      {
         _parameterIdentificationConfigurationMapper = parameterIdentificationConfigurationMapper;
         _outputMappingMapper = outputMappingMapper;
      }

      public override async Task<SnapshotParameterIdentification> MapToSnapshot(ModelParameterIdentification parameterIdentification, PKSimProject context)
      {
         var snapshot = await SnapshotFrom(parameterIdentification, x => { x.Simulations = parameterIdentification.AllSimulations.AllNames().ToArray(); });
         snapshot.Configuration = await _parameterIdentificationConfigurationMapper.MapToSnapshot(parameterIdentification.Configuration);
         snapshot.OutputMappings = await _outputMappingMapper.MapToSnapshots(parameterIdentification.AllOutputMappings);
         return snapshot;
      }

      public override Task<ModelParameterIdentification> MapToModel(SnapshotParameterIdentification snapshot, PKSimProject context)
      {
         throw new NotImplementedException();
      }
   }
}