using System.Collections.Generic;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Import;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IImportObservedDataTask
   {
      void AddObservedDataToProject();
      void AddObservedDataToProjectForCompound(Compound compound);
      void AddObservedDataFromConfigurationToProjectForCompound(Compound compound, ImporterConfiguration configuration);
      void AddObservedDataFromConfigurationToProject(ImporterConfiguration configuration);
      void AddObservedDataFromConfigurationToProjectForDataRepository(ImporterConfiguration configuration, string dataRepositoryName);
      ImporterConfiguration OpenXmlConfiguration();
      void AddAndReplaceObservedDataFromConfigurationToProject(ImporterConfiguration configuration, IEnumerable<DataRepository> observedDataFromSameFile);
   }
}