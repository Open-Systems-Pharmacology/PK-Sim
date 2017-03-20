using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Reflection;
using PKSim.Core.Model;

namespace PKSim.Core
{
   /// <summary>
   ///    User independent settings
   /// </summary>
   public interface IApplicationSettings
   {
      IEnumerable<SpeciesDatabaseMap> SpeciesDataBaseMaps { get; }
      void AddSpeciesDatabaseMap(SpeciesDatabaseMap speciesDatabaseMap);
      void RemoveSpeciesDatabaseMap(string speciesName);
      bool HasExpressionsDatabaseFor(Species species);
      SpeciesDatabaseMap SpeciesDatabaseMapsFor(string speciesName);
   }

   public class ApplicationSettings : Notifier, IApplicationSettings
   {
      private readonly ICache<string, SpeciesDatabaseMap> _allMaps;

      public ApplicationSettings()
      {
         _allMaps = new Cache<string, SpeciesDatabaseMap>(x => x.Species);
      }

      public IEnumerable<SpeciesDatabaseMap> SpeciesDataBaseMaps
      {
         get { return _allMaps; }
      }

      public void AddSpeciesDatabaseMap(SpeciesDatabaseMap speciesDatabaseMap)
      {
         _allMaps.Add(speciesDatabaseMap);
      }

      public void RemoveSpeciesDatabaseMap(string species)
      {
         _allMaps.Remove(species);
      }

      public bool HasExpressionsDatabaseFor(Species species)
      {
         return _allMaps.Contains(species.Name);
      }

      public SpeciesDatabaseMap SpeciesDatabaseMapsFor(string speciesName)
      {
         if (!_allMaps.Contains(speciesName))
         {
            AddSpeciesDatabaseMap(new SpeciesDatabaseMap {Species = speciesName});
         }
         return _allMaps[speciesName];
      }
   }
}