using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   /// <summary>
   /// A model configuration defineds for a given model and species the list of possible category and all their calculation methods.
   /// This does not contain the selected calculation methods, just the possible selection
   /// </summary>
   public class ModelConfiguration 
   {
      private readonly ICache<string, CalculationMethodCategory> _cmCategories = new Cache<string, CalculationMethodCategory>(cm => cm.Name);
      public virtual string SpeciesName { get; set; }
      public virtual string ModelName { get; set; }

      public virtual string Id => $"{ModelName}|{SpeciesName}";

      public virtual IEnumerable<CalculationMethodCategory> CalculationMethodCategories => _cmCategories;

      public virtual void AddCalculationMethodCategory(CalculationMethodCategory calculationMethodCategory)
      {
         _cmCategories.Add(calculationMethodCategory);
      }

      public ModelConfiguration Clone(ICloneManager cloneManager)
      {
         //no clone here as ModelConfiguration are static objects
         return this;
      }
   }
}