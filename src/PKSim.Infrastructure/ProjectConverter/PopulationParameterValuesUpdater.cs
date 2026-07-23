using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ProjectConverter
{
   /// <summary>
   ///    A population saved by an older version carries one value per individual only for the parameters that existed when
   ///    it was created. Once a conversion adds parameters that vary between individuals, the population has no values for
   ///    them and every individual would silently share the value of the base individual. This updater replays the
   ///    variation for those parameters only.
   /// </summary>
   public interface IPopulationParameterValuesUpdater
   {
      /// <summary>
      ///    Adds one value per individual for every parameter of <paramref name="population" /> that takes part in the
      ///    creation of an individual but has no values yet. Returns <c>true</c> if any parameter was added.
      /// </summary>
      bool AddMissingParameterValuesTo(Population population);
   }

   public class PopulationParameterValuesUpdater : IPopulationParameterValuesUpdater
   {
      private readonly ICloner _cloner;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IContainerTask _containerTask;
      private readonly IDistributedParametersUpdater _distributedParametersUpdater;
      private readonly IGenderRepository _genderRepository;
      private readonly IPopulationRepository _populationRepository;

      public PopulationParameterValuesUpdater(
         ICloner cloner,
         IEntityPathResolver entityPathResolver,
         IContainerTask containerTask,
         IDistributedParametersUpdater distributedParametersUpdater,
         IGenderRepository genderRepository,
         IPopulationRepository populationRepository)
      {
         _cloner = cloner;
         _entityPathResolver = entityPathResolver;
         _containerTask = containerTask;
         _distributedParametersUpdater = distributedParametersUpdater;
         _genderRepository = genderRepository;
         _populationRepository = populationRepository;
      }

      public bool AddMissingParameterValuesTo(Population population)
      {
         var baseIndividual = population?.FirstIndividual;
         if (baseIndividual == null)
            return false;

         var numberOfIndividuals = population.NumberOfItems;
         if (numberOfIndividuals == 0)
            return false;

         if (!missingParameterPathsIn(population, baseIndividual).Any())
            return false;

         //One clone is varied in place for every individual. Cloning per individual would be prohibitive on large populations
         var currentIndividual = _cloner.Clone(baseIndividual);
         var allCurrentParameters = _containerTask.CacheAllChildren<IParameter>(currentIndividual);

         //Resolved against the clone, so that a path that does not survive cloning cannot produce a short column of values
         var missingParameterPaths = missingParameterPathsIn(population, baseIndividual)
            .Where(x => allCurrentParameters[x] != null)
            .ToList();

         if (!missingParameterPaths.Any())
            return false;

         var allCurrentDistributedParameters = _containerTask.CacheAllChildren<IDistributedParameter>(currentIndividual);
         var allBaseDistributedParameters = _containerTask.CacheAllChildren<IDistributedParameter>(baseIndividual);

         var allGenders = population.AllGenders(_genderRepository);
         var allSpeciesPopulations = population.AllSpeciesPopulations(_populationRepository);
         var valuesByPath = allValuesByPathFor(population);

         var parameterValuesByPath = missingParameterPaths.ToDictionary(x => x, x => new ParameterValues(x));

         for (var individualIndex = 0; individualIndex < numberOfIndividuals; individualIndex++)
         {
            updateIndividualAt(currentIndividual, allCurrentParameters, valuesByPath, allGenders, allSpeciesPopulations, individualIndex);

            _distributedParametersUpdater.UpdateDistributedParameter(allCurrentDistributedParameters, allBaseDistributedParameters, currentIndividual.OriginData);

            missingParameterPaths.Each(path => parameterValuesByPath[path].Add(allCurrentParameters[path].Value));
         }

         //A parameter that cannot be evaluated is left out entirely: a column of NaN would be worse than no column at all
         var parameterValuesToAdd = parameterValuesByPath.Values.Where(x => x.Values.All(v => !double.IsNaN(v))).ToList();
         parameterValuesToAdd.Each(population.IndividualValuesCache.Add);

         return parameterValuesToAdd.Any();
      }

      private IReadOnlyList<string> missingParameterPathsIn(Population population, Individual baseIndividual)
      {
         return baseIndividual.GetAllChildren<IParameter>(x => x.IsChangedByCreateIndividual)
            .Select(_entityPathResolver.PathFor)
            .Distinct()
            .Where(x => !population.IndividualValuesCache.Has(x))
            .ToList();
      }

      private IReadOnlyDictionary<string, IReadOnlyList<double>> allValuesByPathFor(Population population)
      {
         return population.AllVectorialParameters(_entityPathResolver)
            .Select(_entityPathResolver.PathFor)
            .Distinct()
            .ToDictionary(path => path, population.AllValuesFor);
      }

      private void updateIndividualAt(
         Individual individual,
         PathCache<IParameter> allParameters,
         IReadOnlyDictionary<string, IReadOnlyList<double>> valuesByPath,
         IReadOnlyList<Gender> allGenders,
         IReadOnlyList<SpeciesPopulation> allSpeciesPopulations,
         int individualIndex)
      {
         foreach (var pathAndValues in valuesByPath)
         {
            var parameter = allParameters[pathAndValues.Key];
            var values = pathAndValues.Value;
            if (parameter == null || values == null || individualIndex >= values.Count)
               continue;

            parameter.Value = values[individualIndex];
         }

         //The distributions are resolved from the origin data, so it has to reflect the individual we are standing on.
         //A covariate the species does not define keeps the value of the base individual rather than becoming null
         var originData = individual.OriginData;
         var organism = individual.Organism;
         originData.Age = originDataParameterFrom(organism.Parameter(CoreConstants.Parameters.AGE)) ?? originData.Age;
         originData.GestationalAge = originDataParameterFrom(organism.Parameter(Constants.Parameters.GESTATIONAL_AGE)) ?? originData.GestationalAge;
         originData.Height = originDataParameterFrom(organism.Parameter(CoreConstants.Parameters.HEIGHT)) ?? originData.Height;
         originData.BMI = originDataParameterFrom(organism.Parameter(CoreConstants.Parameters.BMI)) ?? originData.BMI;
         originData.Weight = originDataParameterFrom(organism.Parameter(CoreConstants.Parameters.WEIGHT)) ?? originData.Weight;

         if (individualIndex < allGenders.Count)
            originData.Gender = allGenders[individualIndex];

         if (individualIndex < allSpeciesPopulations.Count)
            originData.Population = allSpeciesPopulations[individualIndex];
      }

      private static OriginDataParameter originDataParameterFrom(IParameter parameter) =>
         parameter == null ? null : new OriginDataParameter(parameter.Value, parameter.Dimension.BaseUnit.Name);
   }
}
