﻿using OSPSuite.Core.Import;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IImportObservedDataTask
   {
      void AddObservedDataToProject();
      void AddObservedDataToProjectForCompound(Compound compound);
      void AddObservedDataFromXmlToProjectForCompound(Compound subject);
      void AddObservedDataFromConfigurationToProject(ImporterConfiguration configuration);
      void AddObservedDataFromConfigurationToProject(ImporterConfiguration configuration, string dataRepositoryName);
   }
}