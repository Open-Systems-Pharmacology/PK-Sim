using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IParametersInContainerRepository : IStartableRepository<ParameterMetaData>
   {
      ParameterMetaData ParameterMetaDataFor(string parameterPath);
      ParameterMetaData ParameterMetaDataFor(IParameter parameter);
   }
}