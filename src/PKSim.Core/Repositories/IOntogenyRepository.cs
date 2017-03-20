using System.Collections.Generic;
using OSPSuite.Core.Maths.Interpolations;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IOntogenyRepository : IStartableRepository<Ontogeny>
   {
      IReadOnlyList<Ontogeny> AllFor(string speciesName);
      double OntogenyFactorFor(Ontogeny ontogeny, string containerName, OriginData originData, RandomGenerator randomGenerator = null);
      double OntogenyFactorFor(Ontogeny ontogeny, string containerName, double? age, double? gestationalAge, RandomGenerator randomGenerator);

      double PlasmaProteinOntogenyFactor(string protein, OriginData originData, RandomGenerator randomGenerator = null);
      double PlasmaProteinOntogenyFactor(string protein, double? age, double? gestationalAge, string species, RandomGenerator randomGenerator);
      
      IReadOnlyList<OntogenyMetaData> AllValuesFor(Ontogeny ontogeny);
      IReadOnlyList<OntogenyMetaData> AllValuesFor(Ontogeny ontogeny, string containerName);


      /// <summary>
      ///    Returns all ontogenie factor defined for the given origin data with pma > origin data.pma
      /// </summary>
      /// <param name="ontogeny">ontogeny</param>
      /// <param name="originData">origin data</param>
      /// <param name="containerName">container were the ontogeny is defined</param>
      /// <param name="randomGenerator">If defined, the factor will be distributed using the existing gsd</param>
      /// <returns>A set of Sample {X = age, Y = factor}</returns>
      IReadOnlyList<Sample> AllOntogenyFactorForStrictBiggerThanPMA(Ontogeny ontogeny, OriginData originData, string containerName, RandomGenerator randomGenerator = null);

      /// <summary>
      ///    Returns all ontogenie factor defined for the given origin data with pma > origin data.pma
      /// </summary>
      /// <param name="parameterName">name of ontogeny factor parameter</param>
      /// <param name="originData">origin data</param>
      /// <param name="randomGenerator">If defined, the factor will be distributed using the existing gsd</param>
      /// <returns>A set of Sample {X = age, Y = factor}</returns>
      IReadOnlyList<Sample> AllPlasmaProteinOntogenyFactorForStrictBiggerThanPMA(string parameterName, OriginData originData, RandomGenerator randomGenerator = null);

      ICache<string, string> SupportedProteins { get; }
   }
}