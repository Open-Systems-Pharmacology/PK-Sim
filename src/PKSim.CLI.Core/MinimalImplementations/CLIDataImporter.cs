using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Infrastructure.Import.Core;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Utility.Collections;
using ImporterConfiguration = OSPSuite.Core.Import.ImporterConfiguration;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIDataImporter : IDataImporter
   {
      public (IReadOnlyList<DataRepository> DataRepositories, ImporterConfiguration Configuration) ImportDataSets(IReadOnlyList<MetaDataCategory> metaDataCategories, IReadOnlyList<ColumnInfo> columnInfos,
         DataImporterSettings dataImporterSettings)
      {
         throw new System.NotImplementedException();
      }

      public IReadOnlyList<DataRepository> ImportFromConfiguration(ImporterConfiguration configuration, IReadOnlyList<MetaDataCategory> metaDataCategories, IReadOnlyList<ColumnInfo> columnInfos,
         DataImporterSettings dataImporterSettings)
      {
         throw new System.NotImplementedException();
      }

      Cache<string, IEnumerable<DataRepository>> IDataImporter.ReloadFromConfiguration(IEnumerable<DataRepository> dataSetsToImport, IEnumerable<DataRepository> existingDataSets)
      {
         throw new System.NotImplementedException();
      }

      public IList<MetaDataCategory> DefaultMetaDataCategories()
      {
         throw new System.NotImplementedException();
      }
   }
}