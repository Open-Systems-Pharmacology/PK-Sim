using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class Species : ObjectBase
   {
      private readonly List<SpeciesPopulation> _allPopulations = new List<SpeciesPopulation>();
      private readonly Cache<string, ParameterValueVersionCategory> _pvvCategories = new Cache<string, ParameterValueVersionCategory>(pvv => pvv.Name);
      public virtual string DisplayName { get; set; }
      public virtual bool IsHuman { get; set; }

      public virtual SpeciesPopulation PopulationByName(string name)
      {
         return _allPopulations.FindByName(name);
      }

      public virtual IReadOnlyList<SpeciesPopulation> Populations => _allPopulations;

      public virtual void AddPopulation(SpeciesPopulation speciesPopulation)
      {
         _allPopulations.Add(speciesPopulation);
      }

      public virtual IReadOnlyCollection<ParameterValueVersionCategory> PVVCategories => _pvvCategories;

      public virtual void AddPVVCategory(ParameterValueVersionCategory pvvCategory)
      {
         _pvvCategories.Add(pvvCategory);
      }

      public virtual ParameterValueVersionCategory PVVCategoryByName(string categoryName)
      {
         return _pvvCategories[categoryName];
      }

   }
}