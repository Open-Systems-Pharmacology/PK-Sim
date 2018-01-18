using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IParametersInContainerRepository
   {
      ParameterMetaData ParameterMetaDataFor(IParameter parameter);
   }
}