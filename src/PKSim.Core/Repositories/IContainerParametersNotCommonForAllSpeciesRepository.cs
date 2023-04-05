using System;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Repositories
{
   public interface IContainerParametersNotCommonForAllSpeciesRepository : IStartableRepository<(string ContainerPath, string ParameterName, int SpeciesCount)>
   {
      bool UsedForAllSpecies(string containerPath, string parameterName);
   }
}