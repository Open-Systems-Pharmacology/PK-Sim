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
      private readonly IdentificationParameterMapper _identificationParameterMapper;

      public ParameterIdentificationMapper(
         ParameterIdentificationConfigurationMapper parameterIdentificationConfigurationMapper,
         OutputMappingMapper outputMappingMapper,
         IdentificationParameterMapper identificationParameterMapper
      )
      {
         _parameterIdentificationConfigurationMapper = parameterIdentificationConfigurationMapper;
         _outputMappingMapper = outputMappingMapper;
         _identificationParameterMapper = identificationParameterMapper;
      }

      public override async Task<SnapshotParameterIdentification> MapToSnapshot(ModelParameterIdentification parameterIdentification, PKSimProject context)
      {
         var snapshot = await SnapshotFrom(parameterIdentification, x => { x.Simulations = parameterIdentification.AllSimulations.AllNames().ToArray(); });
         snapshot.Configuration = await _parameterIdentificationConfigurationMapper.MapToSnapshot(parameterIdentification.Configuration);
         snapshot.OutputMappings = await _outputMappingMapper.MapToSnapshots(parameterIdentification.AllOutputMappings);
         snapshot.IdentificationParameters = await _identificationParameterMapper.MapToSnapshots(parameterIdentification.AllIdentificationParameters);
         return snapshot;
      }

      public override Task<ModelParameterIdentification> MapToModel(SnapshotParameterIdentification snapshot, PKSimProject context)
      {
         throw new NotImplementedException();
      }
   }
}