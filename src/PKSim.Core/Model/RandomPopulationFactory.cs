using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Core.Maths.Statistics;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Reporting;
using PKSim.Core.Services;

namespace PKSim.Core.Model
{
   public interface IRandomPopulationFactory
   {
      /// <summary>
      ///    Creates a random population using the specified <paramref name="populationSettings" />.
      ///    The <paramref name="cancellationToken" /> allows the caller to cancel the task. If the <paramref name="seed" /> is
      ///    provided, it will be set in the created population to generate random values
      /// </summary>
      /// <param name="populationSettings">Population settings used to create the population</param>
      /// <param name="cancellationToken">Allows the called to cancel the task</param>
      /// <param name="seed">If provided, the seed will be used to generate random values and will overwrite the seed created in the population</param>
      /// <param name="addMoleculeParametersVariability">If set to <c>true</c> (default), default parameter variability will be created for all molecules for which information is available in the database </param>
      Task<RandomPopulation> CreateFor(RandomPopulationSettings populationSettings, CancellationToken cancellationToken, int? seed = null, bool addMoleculeParametersVariability = true);
   }

   public class RandomPopulationFactory : IRandomPopulationFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IProgressManager _progressManager;
      private readonly IIndividualModelTask _individualModelTask;
      private readonly ICreateIndividualAlgorithm _createIndividualAlgorithm;
      private readonly IIndividualToIndividualPropertiesMapper _individualPropertiesMapper;
      private readonly IContainerTask _containerTask;
      private readonly ICloner _cloner;
      private readonly IDistributedParametersUpdater _distributedParametersUpdater;
      private readonly IReportGenerator _reportGenerator;
      private readonly IMoleculeParameterVariabilityCreator _moleculeParameterVariabilityCreator;
      private readonly IMoleculeOntogenyVariabilityUpdater _moleculeOntogenyVariabilityUpdater;
      private Queue<Gender> _genderQueue;
      private const int _maxIterations = 100;

      public RandomPopulationFactory(IObjectBaseFactory objectBaseFactory,
         IProgressManager progressManager, IIndividualModelTask individualModelTask,
         ICreateIndividualAlgorithm createIndividualAlgorithm, IIndividualToIndividualPropertiesMapper individualPropertiesMapper,
         IContainerTask containerTask, ICloner cloner, IDistributedParametersUpdater distributedParametersUpdater,
         IReportGenerator reportGenerator, IMoleculeParameterVariabilityCreator moleculeParameterVariabilityCreator, IMoleculeOntogenyVariabilityUpdater moleculeOntogenyVariabilityUpdater)
      {
         _objectBaseFactory = objectBaseFactory;
         _progressManager = progressManager;
         _individualModelTask = individualModelTask;
         _createIndividualAlgorithm = createIndividualAlgorithm;
         _individualPropertiesMapper = individualPropertiesMapper;
         _containerTask = containerTask;
         _cloner = cloner;
         _distributedParametersUpdater = distributedParametersUpdater;
         _reportGenerator = reportGenerator;
         _moleculeParameterVariabilityCreator = moleculeParameterVariabilityCreator;
         _moleculeOntogenyVariabilityUpdater = moleculeOntogenyVariabilityUpdater;
      }

      public Task<RandomPopulation> CreateFor(RandomPopulationSettings populationSettings, CancellationToken cancellationToken, int? seed = null, bool addMoleculeParametersVariability = true)
      {
         return Task.Run(() =>
         {
            using (var progressUpdater = _progressManager.Create())
            {
               var randomPopulation = createPopulationFor(populationSettings, seed);

               fllUpGenderQueueBasedOn(populationSettings);
               progressUpdater.Initialize(populationSettings.NumberOfIndividuals, PKSimConstants.UI.CreatingPopulation);

               //the base indiviudal is used to retrieve the default values. 
               var baseIndividual = populationSettings.BaseIndividual;

               //current individual defined as a clone of the based individual. The current individual will be the one varying 
               var currentIndividual = _cloner.Clone(populationSettings.BaseIndividual);

               //cache containing all parameters changed by the create individual from the current individual. This will be used just as reference to the current parameters
               var allChangedByCreatedIndividualParameters = getAllCreateIndividualParametersFrom(currentIndividual);
               //all distributed parameters. This will be used to udpate the distribution for the current individual
               var allDistributedParameters = getAllDistributedParametersFrom(currentIndividual);
               var allBaseDistributedParamters = getAllDistributedParametersFrom(baseIndividual);

               //all individual parameters. Just an optimiztion to avoid call GetAllChildren for each individual
               var allIndividualParameters = currentIndividual.GetAllChildren<IParameter>().ToList();

               int maxTotalIterations = populationSettings.NumberOfIndividuals * _maxIterations;
               uint numberOfTry = 0;

               var currentGender = _genderQueue.Dequeue();
               do
               {
                  cancellationToken.ThrowIfCancellationRequested();

                  numberOfTry++;

                  //could not create one item in max Iteration try=>exit
                  if (numberOfTry > _maxIterations && randomPopulation.NumberOfItems == 0)
                     throw new CannotCreatePopulationWithConstraintsException(_reportGenerator.StringReportFor(populationSettings));

                  //create a new individual based on population settings defined by the user
                  updateCurrentIndividualFromSettings(populationSettings, currentIndividual, allDistributedParameters, allBaseDistributedParamters, currentGender, randomPopulation.RandomGenerator);

                  bool success = tryRandomize(currentIndividual, populationSettings, allIndividualParameters, randomPopulation.RandomGenerator);
                  if (!success) continue;

                  randomPopulation.AddIndividualProperties(_individualPropertiesMapper.MapFrom(currentIndividual, allChangedByCreatedIndividualParameters));

                  currentGender = _genderQueue.Dequeue();
                  progressUpdater.IncrementProgress(PKSimConstants.UI.CreatingIndividualInPopulation(randomPopulation.NumberOfItems, populationSettings.NumberOfIndividuals));
               } while (randomPopulation.NumberOfItems < populationSettings.NumberOfIndividuals && numberOfTry < maxTotalIterations);

               if (numberOfTry >= maxTotalIterations)
                  throw new CannotCreatePopulationWithConstraintsException(_reportGenerator.StringReportFor(populationSettings));

               if(addMoleculeParametersVariability)
                  _moleculeParameterVariabilityCreator.AddVariabilityTo(randomPopulation);

               _moleculeOntogenyVariabilityUpdater.UpdateAllOntogenies(randomPopulation);

               randomPopulation.IsLoaded = true;
               return randomPopulation;
            }
         }, cancellationToken);
      }

 
      private RandomPopulation createPopulationFor(RandomPopulationSettings populationSettings, int? seed)
      {
         var randomPopulation = _objectBaseFactory.Create<RandomPopulation>();
         randomPopulation.Root = _objectBaseFactory.Create<IRootContainer>();
         randomPopulation.Settings = populationSettings;
         randomPopulation.SetAdvancedParameters(_objectBaseFactory.Create<AdvancedParameterCollection>());

         if (seed != null)
            randomPopulation.Seed = seed.Value;

         return randomPopulation;
      }

      private bool tryRandomize(Individual individual, RandomPopulationSettings populationSettings, IEnumerable<IParameter> allIndividualParameters, RandomGenerator randomGenerator)
      {
         try
         {
            var bodyWeightRange = populationSettings.ParameterRange(CoreConstants.Parameter.MEAN_WEIGHT);
            var bodyWeightParameter = _individualModelTask.MeanOrganismParameter(individual.OriginData, CoreConstants.Parameter.MEAN_WEIGHT);
            _createIndividualAlgorithm.Randomize(individual, bodyWeightParameter, bodyWeightRange.MinValue, bodyWeightRange.MaxValue, allIndividualParameters, randomGenerator);

            if (!individual.OriginData.SpeciesPopulation.IsHeightDependent)
               return true;

            //last: Check if the value for the bmi is in the interval
            var bmi = individual.Organism.Parameter(CoreConstants.Parameter.BMI).Value;
            var bmiRange = populationSettings.ParameterRange(CoreConstants.Parameter.BMI);
            return bmiRange.IsValueInRange(bmi);
         }
         catch (CannotCreateIndividualWithConstraintsException)
         {
            return false;
         }
      }

      private void updateCurrentIndividualFromSettings(RandomPopulationSettings populationSettings, Individual currentIndividual,
         PathCache<IDistributedParameter> allCurrentDistribuedParameters, 
         PathCache<IDistributedParameter> allBaseDistributedParamters, 
         Gender currentGender,
         RandomGenerator randomGenerator)
      {
         //clone of the original origin data
         currentIndividual.OriginData = populationSettings.BaseIndividual.OriginData.Clone();
         currentIndividual.OriginData.Gender = currentGender;

         //perform random varation of the origin data according to the settings defined for the population
         perturbate(currentIndividual, populationSettings, randomGenerator);

         //Update the distribution according to the new origin data
         _distributedParametersUpdater.UpdateDistributedParameter(allCurrentDistribuedParameters, allBaseDistributedParamters, currentIndividual.OriginData);
      }

      private PathCache<IDistributedParameter> getAllDistributedParametersFrom(Individual individual)
      {
         return _containerTask.CacheAllChildren<IDistributedParameter>(individual);
      }

      private PathCache<IParameter> getAllCreateIndividualParametersFrom(Individual individual)
      {
         return _containerTask.CacheAllChildrenSatisfying<IParameter>(individual, p => p.IsChangedByCreateIndividual);
      }

      private void fllUpGenderQueueBasedOn(RandomPopulationSettings populationSettings)
      {
         _genderQueue = new Queue<Gender>();
         foreach (var genderRatio in populationSettings.GenderRatios)
         {
            var numberOfGender = genderRatio.Ratio / 100.0 * populationSettings.NumberOfIndividuals;
            for (int i = 0; i < numberOfGender; i++)
            {
               _genderQueue.Enqueue(genderRatio.Gender);
            }
         }

         //add possible items missing because of rounding artefacts
         //+1 is because we need to add one element more in any case in order to be able to dequeue properly
         for (int i = 0; i < populationSettings.NumberOfIndividuals + 1 - _genderQueue.Count; i++)
         {
            _genderQueue.Enqueue(populationSettings.BaseIndividual.OriginData.Gender);
         }
      }

      private void perturbate(Individual currentIndividual, RandomPopulationSettings populationSettings, RandomGenerator randomGenerator)
      {
         bool success = true;
         var originData = currentIndividual.OriginData;
         uint numberOfTry = 0;
         do
         {
            numberOfTry++;

            //first create a new age value if necessary
            if (originData.SpeciesPopulation.IsAgeDependent)
            {
               originData.Age = createRandomValueFor(originData, populationSettings, CoreConstants.Parameter.AGE, randomGenerator, out success);
               currentIndividual.Organism.Parameter(CoreConstants.Parameter.AGE).Value = originData.Age.Value;
               if (!success) continue;
            }

            if (originData.SpeciesPopulation.IsPreterm)
            {
               originData.GestationalAge = createDiscreteRandomValueFor(populationSettings, CoreConstants.Parameter.GESTATIONAL_AGE, randomGenerator, out success);
               currentIndividual.Organism.Parameter(CoreConstants.Parameter.GESTATIONAL_AGE).Value = originData.GestationalAge.Value;
               if (!success) continue;
            }

            //Then define gender depending on selecting proportions
            if (originData.SpeciesPopulation.IsHeightDependent)
            {
               originData.Height = createRandomValueFor(originData, populationSettings, CoreConstants.Parameter.MEAN_HEIGHT, randomGenerator, out success);
               currentIndividual.Organism.Parameter(CoreConstants.Parameter.HEIGHT).Value = originData.Height.Value;
            }
         } while (!success && numberOfTry < _maxIterations);

         if (numberOfTry >= _maxIterations)
            throw new CannotCreatePopulationWithConstraintsException(_reportGenerator.StringReportFor(populationSettings));
      }

      private double createRandomValueFor(OriginData originData, RandomPopulationSettings populationSettings, string parameterName, RandomGenerator randomGenerator, out bool success)
      {
         var parameterRange = populationSettings.ParameterRange(parameterName);
         var parameter = _individualModelTask.MeanOrganismParameter(originData, parameterName);
         return tryCreateRandomValueFor(parameterRange, parameter, randomGenerator, out success);
      }

      private double createDiscreteRandomValueFor(RandomPopulationSettings populationSettings, string parameterName, RandomGenerator randomGenerator, out bool success)
      {
         var parameterRange = populationSettings.ParameterRange(parameterName) as DiscreteParameterRange;
         success = true;

         if (parameterRange == null)
         {
            success = false;
            return 0;
         }

         if (parameterRange.IsConstant)
            return parameterRange.MinValue.Value;

         return randomGenerator.NextInteger(parameterRange.MinValue.ConvertedTo<int>(), parameterRange.MaxValue.ConvertedTo<int>());
      }

      private double tryCreateRandomValueFor(ParameterRange parameterRange, IParameter baseParameter, RandomGenerator randomGenerator, out bool success)
      {
         try
         {
            success = true;
            if (parameterRange.IsConstant)
               return parameterRange.MinValue.Value;

            return baseParameter.RandomDeviateIn(randomGenerator, parameterRange.MinValue, parameterRange.MaxValue);
         }
         catch (DistributionException)
         {
            success = false;
            return baseParameter.Value;
         }
      }
   }
}