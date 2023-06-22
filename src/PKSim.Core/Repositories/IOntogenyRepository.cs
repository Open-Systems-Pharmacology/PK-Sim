using System.Collections.Generic;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
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

      DataRepository OntogenyToRepository(Ontogeny ontogeny, string containerName);
      DistributedTableFormula DataRepositoryToDistributedTableFormula(DataRepository ontogenyDataRepository);
      DistributedTableFormula OntogenyToDistributedTableFormula(Ontogeny ontogeny, string containerName);

      double PlasmaProteinOntogenyFactor(string protein, OriginData originData, RandomGenerator randomGenerator = null);
      double PlasmaProteinOntogenyFactor(string protein, double? age, double? gestationalAge, string species, RandomGenerator randomGenerator);

      IReadOnlyList<OntogenyMetaData> AllValuesFor(Ontogeny ontogeny);
      IReadOnlyList<OntogenyMetaData> AllValuesFor(Ontogeny ontogeny, string containerName);

      /// <summary>
      ///    Returns all ontogenies factor defined for the given origin data with pma > origin data.pma
      /// </summary>
      /// <param name="ontogeny">ontogeny</param>
      /// <param name="originData">origin data</param>
      /// <param name="containerName">container were the ontogeny is defined</param>
      /// <param name="randomGenerator">If defined, the factor will be distributed using the existing gsd</param>
      /// <returns>A set of Sample {X = age, Y = factor}</returns>
      IReadOnlyList<Sample> AllOntogenyFactorForStrictBiggerThanPMA(Ontogeny ontogeny, OriginData originData, string containerName, RandomGenerator randomGenerator = null);

      /// <summary>
      ///    Returns all ontogenies factor defined for the given origin data with pma > origin data.pma
      /// </summary>
      /// <param name="parameterName">name of ontogeny factor parameter</param>
      /// <param name="originData">origin data</param>
      /// <param name="randomGenerator">If defined, the factor will be distributed using the existing gsd</param>
      /// <returns>A set of Sample {X = age, Y = factor}</returns>
      IReadOnlyList<Sample> AllPlasmaProteinOntogenyFactorForStrictBiggerThanPMA(string parameterName, OriginData originData, RandomGenerator randomGenerator = null);

      ICache<string, string> SupportedProteins { get; }

      /// <summary>
      /// Returns the parameter distribution associated with the ontogeny for the PMA defined in OriginData at location <paramref name="containerName"/>
      /// </summary>
      /// <param name="ontogeny">Ontogeny for which a distribution should be retrieved</param>
      /// <param name="originData">Origin data used to evaluate the PMA (as a function of age and gestational age)</param>
      /// <param name="containerName">Location of the ontogeny distribution to retrieve. (e.g. GI vs Liver)</param>
      (double mean, double std, DistributionType distributionType) OntogenyParameterDistributionFor(Ontogeny ontogeny, OriginData originData, string containerName);
   }
}