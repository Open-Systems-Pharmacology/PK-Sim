using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IContainerParametersNotCommonForAllSpeciesRepository : IStartableRepository<ContainerParameterBySpecies>
   {
      bool UsedForAllSpecies(string containerPath, string parameterName);
      bool UsedForAllSpecies(string parameterFullPath);
      bool UsedForAllSpecies(ParameterMetaData parameterMetaData);
   }
}