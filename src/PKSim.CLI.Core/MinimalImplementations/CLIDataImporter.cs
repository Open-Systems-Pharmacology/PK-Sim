using System;
using System.Collections.Generic;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Infrastructure.Import.Core;
using OSPSuite.Infrastructure.Import.Services;
using ImporterConfiguration = OSPSuite.Core.Import.ImporterConfiguration;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIDataImporter : IDataImporter
   {
      public (IReadOnlyList<DataRepository> DataRepositories, ImporterConfiguration Configuration) ImportDataSets(IReadOnlyList<MetaDataCategory> metaDataCategories, IReadOnlyList<ColumnInfo> columnInfos, DataImporterSettings dataImporterSettings, string dataFileName)
      {
         throw new NotImplementedException();
      }

      public IReadOnlyList<DataRepository> ImportFromConfiguration(ImporterConfiguration configuration, IReadOnlyList<MetaDataCategory> metaDataCategories, IReadOnlyList<ColumnInfo> columnInfos, DataImporterSettings dataImporterSettings, string dataFileName)
      {
         throw new NotImplementedException();
      }

      public ReloadDataSets CalculateReloadDataSetsFromConfiguration(IReadOnlyList<DataRepository> dataSetsToImport, IReadOnlyList<DataRepository> existingDataSets)
      {
         throw new NotImplementedException();
      }

      public IReadOnlyList<MetaDataCategory> DefaultMetaDataCategoriesForObservedData()
      {
         throw new NotImplementedException();
      }

  
      public IReadOnlyList<ColumnInfo> ColumnInfosForObservedData()
      {
         throw new NotImplementedException();
      }

      public bool AreFromSameMetaDataCombination(DataRepository sourceDataRepository, DataRepository targetDataRepository)
      {
         throw new NotImplementedException();
      }

      public ImporterConfiguration ConfigurationFromData(string dataPath, IReadOnlyList<ColumnInfo> columnInfos, IReadOnlyList<MetaDataCategory> metaDataCategories, string sheetName = null)
      {
         throw new NotImplementedException();
      }
   }
}