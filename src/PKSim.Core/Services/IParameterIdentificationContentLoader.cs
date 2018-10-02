using OSPSuite.Core.Domain.ParameterIdentifications;

namespace PKSim.Core.Services
{
   public interface IParameterIdentificationContentLoader
   {
      void LoadContentFor(ParameterIdentification parameterIdentification);
   }
}