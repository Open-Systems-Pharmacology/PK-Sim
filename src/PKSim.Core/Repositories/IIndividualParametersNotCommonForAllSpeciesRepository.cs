﻿using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IIndividualParametersNotCommonForAllSpeciesRepository : IStartableRepository<IndividualParameterBySpecies>
   {
      bool UsedForAllSpecies(string containerPath, string parameterName);
      bool UsedForAllSpecies(string parameterFullPath);
      bool UsedForAllSpecies(ParameterMetaData parameterMetaData);
   }
}