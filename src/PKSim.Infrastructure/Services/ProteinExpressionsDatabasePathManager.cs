using System;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Core;

namespace PKSim.Infrastructure.Services
{
   public class ProteinExpressionsDatabasePathManager : IProteinExpressionsDatabasePathManager
   {
      private readonly IProteinExpressionDatabase _database;
      private readonly IApplicationSettings _applicationSettings;
      private readonly IProteinExpressionQueries _proteinExpressionQueries;

      public ProteinExpressionsDatabasePathManager(IProteinExpressionDatabase database, IApplicationSettings applicationSettings, IProteinExpressionQueries proteinExpressionQueries)
      {
         _database = database;
         _applicationSettings = applicationSettings;
         _proteinExpressionQueries = proteinExpressionQueries;
      }

      public bool HasDatabaseFor(Species species)
      {
         if (!_applicationSettings.HasExpressionsDatabaseFor(species)) 
            return false;

         return FileHelper.FileExists(databaseFor(species.Name));
      }

   
      public IDisposable ConnectToDatabaseFor(Species species)
      {
         _proteinExpressionQueries.ClearCache();
         return new DatabaseDisposer(_database, databaseFor(species.Name));
      }

      private string databaseFor(string speciesName)
      {
         return _applicationSettings.SpeciesDatabaseMapsFor(speciesName).DatabaseFullPath;
      }
   
   }
}