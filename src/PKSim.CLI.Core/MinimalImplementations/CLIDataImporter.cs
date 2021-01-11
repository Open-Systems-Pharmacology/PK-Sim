using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Infrastructure.Import.Core;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIDataImporter : IDataImporter
   {
      public IEnumerable<DataRepository> ImportDataSets(IReadOnlyList<MetaDataCategory> metaDataCategories, IReadOnlyList<ColumnInfo> columnInfos, DataImporterSettings dataImporterSettings)
      {
         return Enumerable.Empty<DataRepository>();
      }

      public DataRepository ImportDataSet(IReadOnlyList<MetaDataCategory> metaDataCategories, IReadOnlyList<ColumnInfo> columnInfos, DataImporterSettings dataImporterSettings)
      {
         return new NullDataRepository();
      }
   }
}