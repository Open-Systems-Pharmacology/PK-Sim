using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public enum CategoryType
   {
      Model,
      Individual,
      Molecule
   }

   public class CalculationMethodCategory : Category<CalculationMethod>
   {
      public CategoryType CategoryType { get; set; }

      public virtual CalculationMethod DefaultItemForSpecies(Species species)
      {
         var defaultItem = DefaultItem;
         if (defaultItem.AllSpecies.Contains(species.Name))
            return defaultItem;

         return AllItems().FirstOrDefault(x => x.AllSpecies.Contains(species.Name));
      }

      public virtual IEnumerable<CalculationMethod> AllForSpecies(Species species)
      {
         return AllItems().Where(x => x.AllSpecies.Contains(species.Name));
      }

      public virtual bool IsIndividual => CategoryType == CategoryType.Individual;

      public virtual bool IsMolecule => CategoryType == CategoryType.Molecule;
   }

   public class ParameterValueVersionCategory : Category<ParameterValueVersion>
   {
   }
}