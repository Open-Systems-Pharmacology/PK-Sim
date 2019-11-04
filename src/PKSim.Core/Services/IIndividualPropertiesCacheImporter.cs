using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Import.Services;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IIndividualPropertiesCacheImporter
   {
      IndividualPropertiesCache ImportFrom(string fileFullPath, PathCache<IParameter> allParameters, IImportLogger logger);
   }
}