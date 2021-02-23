using OSPSuite.Core.Import;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IImportObservedDataTask
   {
      void AddObservedDataToProject();
      void AddObservedDataToProjectForCompound(Compound compound);
      void AddObservedDataFromConfigurationToProject(Compound compound, ImporterConfiguration configuration);
      void AddObservedDataFromConfigurationToProject(ImporterConfiguration configuration);
      void AddObservedDataFromConfigurationToProject(ImporterConfiguration configuration, string dataRepositoryName);
      ImporterConfiguration OpenXmlConfiguration();
   }
}