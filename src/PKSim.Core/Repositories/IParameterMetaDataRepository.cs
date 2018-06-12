using System.Collections.Generic;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Repositories
{
   public interface IParameterMetaDataRepository<TParameterMetaData> : IStartableRepository<TParameterMetaData>
   {
      IReadOnlyList<TParameterMetaData> AllFor(string containerPath);
      TParameterMetaData ParameterMetaDataFor(string containerPath, string parameterName);
   }
}