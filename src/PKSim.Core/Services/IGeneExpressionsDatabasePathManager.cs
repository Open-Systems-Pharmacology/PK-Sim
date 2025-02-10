using System;
using System.Collections.Generic;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IGeneExpressionsDatabasePathManager
   {
      /// <summary>
      ///    return true if a database has been defined for the species, otherwise false
      /// </summary>
      bool HasDatabaseFor(Species species);

      /// <summary>
      ///    return true if a database has been defined for the species, otherwise false
      /// </summary>
      bool HasDatabaseFor(string speciesName);

      /// <summary>
      ///    Update the current database connection to point on path defined for the species
      /// </summary>
      /// <exception cref="KeyNotFoundException">is thrown if no protein expression database is available for that species. </exception>
      IDisposable ConnectToDatabaseFor(Species species);

      /// <summary>
      ///    Update the current database connection to point on path defined for the species
      /// </summary>
      /// <exception cref="KeyNotFoundException">is thrown if no protein expression database is available for that species. </exception>
      IDisposable ConnectToDatabaseFor(string speciesName);
   }
}