using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Import.Services;

namespace PKSim.Core.Services
{
   public interface ISimulationPKAnalysesImporter
   {
      IEnumerable<QuantityPKParameter> ImportPKParameters(string fileFullPath, IImportLogger logger);
   }
}