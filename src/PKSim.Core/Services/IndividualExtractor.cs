using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IIndividualExtractor
   {
      /// <summary>
      ///    Extracts individuals with id in <paramref name="individualIds" /> from <paramref name="population" />
      /// </summary>
      void ExtractIndividualsFrom(Population population, params int[] individualIds);

      /// <summary>
      ///    Extracts individuals with id in <paramref name="individualIds" /> from <paramref name="population" />
      /// </summary>
      void ExtractIndividualsFrom(Population population, IEnumerable<int> individualIds);

      /// <summary>
      ///    Extracts individuals from <paramref name="population" /> using the provided
      ///    <paramref name="individualExtractionOptions" />
      /// </summary>
      void ExtractIndividualsFrom(Population population, IndividualExtractionOptions individualExtractionOptions);

      /// <summary>
      ///    Extracts individuals from <paramref name="population" /> using the provided
      ///    <paramref name="individualExtractionOptions" />
      /// </summary>
      /// <returns>The executed command for the extraction. This command was not added to the history</returns>
      ICommand ExtractIndividualsFromPopulationCommand(Population population, IndividualExtractionOptions individualExtractionOptions);
   }

   public class IndividualExtractor : IIndividualExtractor
   {
      private readonly IExecutionContext _executionContext;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IIndividualTask _individualTask;
      private readonly IContainerTask _containerTask;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public IndividualExtractor(IExecutionContext executionContext, IEntityPathResolver entityPathResolver, IIndividualTask individualTask, IContainerTask containerTask, IBuildingBlockRepository buildingBlockRepository)
      {
         _executionContext = executionContext;
         _entityPathResolver = entityPathResolver;
         _individualTask = individualTask;
         _containerTask = containerTask;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public void ExtractIndividualsFrom(Population population, params int[] individualIds)
      {
         ExtractIndividualsFrom(population, individualIds.ToList());
      }

      public void ExtractIndividualsFrom(Population population, IEnumerable<int> individualIds)
      {
         ExtractIndividualsFrom(population, new IndividualExtractionOptions {IndividualIds = individualIds});
      }

      public void ExtractIndividualsFrom(Population population, IndividualExtractionOptions individualExtractionOptions)
      {
         _executionContext.AddToHistory(ExtractIndividualsFromPopulationCommand(population, individualExtractionOptions));
      }

      public ICommand ExtractIndividualsFromPopulationCommand(Population population, IndividualExtractionOptions individualExtractionOptions)
      {
         if (population.IsAnImplementationOf<MoBiPopulation>())
            throw new PKSimException(PKSimConstants.Error.CannotExtractIndividualFrom(_executionContext.TypeFor(population)));

         var allPopulationParameters = population.AllVectorialParameters(_entityPathResolver).ToList();

         var allDistributedParameters = allPopulationParameters.Where(x => x.Formula.IsDistributed()).ToList();
         var allConstantParameters = allPopulationParameters.Where(x => x.Formula.IsConstant()).ToList();
         var allRemainingParameters = allPopulationParameters.Except(allConstantParameters).Except(allDistributedParameters);

         //Update constant parameters first, then distributed and last all others. This ensures that formula parameters in the extracted individuals are not updated with a constant value
         var allParametersOrderedByFormulaType = allConstantParameters.Union(allDistributedParameters).Union(allRemainingParameters).ToList();

         var addCommands = individualExtractionOptions.IndividualIds.Distinct()
            .Select(individualId => extractIndividualFrom(population, individualId, allParametersOrderedByFormulaType, individualExtractionOptions))
            .ToList();

         var overallCommand = new PKSimMacroCommand
         {
            CommandType = PKSimConstants.Command.CommandTypeAdd,
            ObjectType = PKSimConstants.ObjectTypes.Population,
            Description = PKSimConstants.Command.ExtractingIndividualsDescription(population.Name),
            BuildingBlockName = population.Name,
            BuildingBlockType = PKSimConstants.ObjectTypes.Population
         };

         overallCommand.AddRange(addCommands);

         return overallCommand;
      }

      private IPKSimCommand extractIndividualFrom(Population population, int individualId, IEnumerable<IParameter> allParametersOrderedByFormulaType, IndividualExtractionOptions individualExtractionOptions)
      {
         if (population.NumberOfItems <= individualId)
            return new PKSimEmptyCommand();

         var individual = _executionContext.Clone(population.FirstIndividual)
            .WithName(createIndividualName(population, individualId, individualExtractionOptions));

         var exctractedIndividualParameterCache = new PathCache<IParameter>(_entityPathResolver).For(individual.GetAllChildren<IParameter>());

         foreach (var parameter in allParametersOrderedByFormulaType)
         {
            updateParameterValue(population, individualId, parameter, exctractedIndividualParameterCache);
         }

         var originData = individual.OriginData;
         updateOriginDataFromIndividual(individual, originData);

         originData.Gender = population.AllGenders[individualId];
         originData.SpeciesPopulation = population.AllRaces[individualId];

         return _individualTask.AddToProject(individual, editBuildingBlock: false, addToHistory: false);
      }

      private static void updateOriginDataFromIndividual(Individual individual, OriginData originData)
      {
         var organism = individual.Organism;
         originData.Age = organism.Parameter(CoreConstants.Parameter.AGE)?.Value;
         originData.GestationalAge = organism.Parameter(CoreConstants.Parameter.GESTATIONAL_AGE)?.Value;
         originData.Height = organism.Parameter(CoreConstants.Parameter.HEIGHT)?.Value;
         originData.BMI = organism.Parameter(CoreConstants.Parameter.BMI)?.Value;
         originData.Weight = organism.Parameter(CoreConstants.Parameter.WEIGHT).Value;
      }

      private void updateParameterValue(Population population, int individualId, IParameter parameter, PathCache<IParameter> exctractedIndividualParameterCache)
      {
         var parameterPath = _entityPathResolver.PathFor(parameter);
         var parameterValues = population.AllValuesFor(parameterPath);
         var exactractedParameter = exctractedIndividualParameterCache[parameterPath];
         var value = parameterValues[individualId];

         if (!shouldUpdateParameter(exactractedParameter, value))
            return;

         exactractedParameter.Value = value;
      }

      private bool shouldUpdateParameter(IParameter parameter, double value)
      {
         return !ValueComparer.AreValuesEqual(parameter.Value, value);
      }

      private string createIndividualName(Population population, int individualId, IndividualExtractionOptions options)
      {
         var proposedName = options.GenerateIndividualName(population.Name, individualId);
         return _containerTask.CreateUniqueName(_buildingBlockRepository.All<Individual>(), proposedName, canUseBaseName: true);
      }
   }
}