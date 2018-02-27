using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterIdentificationContext
   {
      public OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentification ParameterIdentification { get; }
      public PKSimProject Project { get; }

      public ParameterIdentificationContext(OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentification parameterIdentification, PKSimProject project)
      {
         ParameterIdentification = parameterIdentification;
         Project = project;
      }
   }
}