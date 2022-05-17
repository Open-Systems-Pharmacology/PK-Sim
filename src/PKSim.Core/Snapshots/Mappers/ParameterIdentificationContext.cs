using PKSim.Core.Model;
using ModelParameterIdentification = OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentification;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterIdentificationContext : SnapshotContext
   {
      public ModelParameterIdentification ParameterIdentification { get; }

      public ParameterIdentificationContext(ModelParameterIdentification parameterIdentification, SnapshotContext snapshotContext) : base(snapshotContext)
      {
         ParameterIdentification = parameterIdentification;
      }
   }
}