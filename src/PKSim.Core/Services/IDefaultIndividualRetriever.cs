using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IDefaultIndividualRetriever
   {
      /// <summary>
      ///    Retrieves a default individual for the default species
      /// </summary>
      Individual DefaultIndividual();

      /// <summary>
      ///    Retrieves the default human
      /// </summary>
      Individual DefaultHuman();

      /// <summary>
      ///    Retrieves the default individual for the given species
      /// </summary>
      Individual DefaultIndividualFor(Species species);

      /// <summary>
      ///    Retrieves the default human for the given population
      /// </summary>
      Individual DefaultIndividualFor(SpeciesPopulation speciesPopulation);
   }
}