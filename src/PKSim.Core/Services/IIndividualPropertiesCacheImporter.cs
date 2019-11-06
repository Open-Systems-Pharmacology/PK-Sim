using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Infrastructure.Import.Services;

namespace PKSim.Core.Services
{
   public interface IIndividualPropertiesCacheImporter
   {
      IndividualValuesCache ImportFrom(string fileFullPath, PathCache<IParameter> allParameters, IImportLogger logger);
   }
}