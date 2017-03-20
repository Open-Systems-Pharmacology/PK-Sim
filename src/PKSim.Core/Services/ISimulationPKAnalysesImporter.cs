using System.Collections.Generic;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface ISimulationPKAnalysesImporter
   {
      IEnumerable<QuantityPKParameter> ImportPKParameters(string fileFullPath, IImportLogger logger);
   }
}