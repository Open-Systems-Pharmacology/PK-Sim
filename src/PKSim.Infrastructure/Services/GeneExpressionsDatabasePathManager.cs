using System;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Core;

namespace PKSim.Infrastructure.Services
{
   public class GeneExpressionsDatabasePathManager : IGeneExpressionsDatabasePathManager
   {
      private readonly IGeneExpressionDatabase _database;
      private readonly IApplicationSettings _applicationSettings;
      private readonly IGeneExpressionQueries _geneExpressionQueries;

      public GeneExpressionsDatabasePathManager(IGeneExpressionDatabase database, IApplicationSettings applicationSettings, IGeneExpressionQueries geneExpressionQueries)
      {
         _database = database;
         _applicationSettings = applicationSettings;
         _geneExpressionQueries = geneExpressionQueries;
      }

      public bool HasDatabaseFor(Species species)
      {
         if (!_applicationSettings.HasExpressionsDatabaseFor(species)) 
            return false;

         return FileHelper.FileExists(databaseFor(species.Name));
      }

   
      public IDisposable ConnectToDatabaseFor(Species species)
      {
         _geneExpressionQueries.ClearCache();
         return new DatabaseDisposer(_database, databaseFor(species.Name));
      }

      private string databaseFor(string speciesName)
      {
         return _applicationSettings.SpeciesDatabaseMapsFor(speciesName).DatabaseFullPath;
      }
   
   }
}