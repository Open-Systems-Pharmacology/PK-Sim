using System.Collections.Generic;
using PKSim.Core.Model;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core.Services
{
   public interface IIndividualResultsImporter
   {
      IEnumerable<IndividualResults> ImportFrom(string fileFullPath, Simulation simulation,  IImportLogger logger);
   }
}  