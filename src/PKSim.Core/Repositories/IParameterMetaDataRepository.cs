using System.Collections.Generic;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Repositories
{
   public interface IParameterMetaDataRepository<TParameterMetaData> : IStartableRepository<TParameterMetaData>
   {
      IEnumerable<TParameterMetaData> AllFor(string containerPath);
   }
}