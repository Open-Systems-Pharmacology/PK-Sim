using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IIndividualPropertiesCacheImporter
   {
      IndividualPropertiesCache ImportFrom(string fileFullPath, IImportLogger logger);
   }
}