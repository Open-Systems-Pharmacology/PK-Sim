using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using ModelIdentificationParameter = OSPSuite.Core.Domain.ParameterIdentifications.IdentificationParameter;
using SnapshotIdentificationParameter = PKSim.Core.Snapshots.IdentificationParameter;

namespace PKSim.Core.Snapshots.Mappers
{
   public class IdentificationParameterMapper : ParameterContainerSnapshotMapperBase<ModelIdentificationParameter, SnapshotIdentificationParameter>
   {
      public IdentificationParameterMapper(ParameterMapper parameterMapper) : base(parameterMapper)
      {
      }

      public override Task<SnapshotIdentificationParameter> MapToSnapshot(ModelIdentificationParameter identificationParameter)
      {
         return SnapshotFrom(identificationParameter, x =>
         {
            x.Scaling = identificationParameter.Scaling;
            x.UseAsFactor = SnapshotValueFor(identificationParameter.UseAsFactor);
            x.IsFixed = SnapshotValueFor(identificationParameter.IsFixed);
            x.LinkedParameters = linkedParametersFrom(identificationParameter.AllLinkedParameters);
         });
      }

      private string[] linkedParametersFrom(IReadOnlyList<ParameterSelection> linkedParameters)
      {
         return !linkedParameters.Any() ? null : linkedParameters.Select(x => x.FullQuantityPath).ToArray();
      }

      public override Task<ModelIdentificationParameter> MapToModel(SnapshotIdentificationParameter snapshot)
      {
         throw new NotImplementedException();
      }
   }
}