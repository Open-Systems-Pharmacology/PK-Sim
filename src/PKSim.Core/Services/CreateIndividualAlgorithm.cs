using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Maths.Optimization.NelderMead;
using OSPSuite.Core.Maths.Random;
using PKSim.Core.Model;
using PKSim.Core.Reporting;

namespace PKSim.Core.Services
{
   public interface ICreateIndividualAlgorithm
   {
      /// <summary>
      ///    Apply the create individual to the given individual based on its origin data
      /// </summary>
      /// <param name="individual">Individual for which the create individual will be applied</param>
      void Optimize(Individual individual);

      /// <summary>
      ///    Randomize the distributed parameter of the individual based on the origin data.
      ///    the generated weight should be in the interval [<paramref name="minWeight" />, [<paramref name="maxWeight" />]
      /// </summary>
      /// <param name="individual">Individual to perturbate</param>
      /// <param name="weightParameter">Weight parameter for the given individual</param>
      /// <param name="minWeight">minimal weight allowed for the randomized individual. Null means not constrained</param>
      /// <param name="maxWeight">maximal weight allowed for the randomized individual. Null means not constrained</param>
      /// <param name="allIndividualParameters">All individual parameters defined in the individual (optimization purpose only)</param>
      /// <param name="randomGenerator">Random generator used to create values</param>
      void Randomize(Individual individual, IParameter weightParameter, double? minWeight, double? maxWeight, IEnumerable<IParameter> allIndividualParameters, RandomGenerator randomGenerator);
   }

   public class CreateIndividualAlgorithm : ICreateIndividualAlgorithm
   {
      private const double CONVERGENCE_TOLERANCE = 1e-6;
      private const int MAX_NUMBER_OF_OPTIMIZATION_ITERATIONS = 10000;
      private const int MAX_CREATION_TRIES = 100;

      private readonly IContainerTask _containerTask;
      private readonly IReportGenerator _reportGenerator;
      private IList<IMuSigma> _muSigmas;
      private double _hrel;
      private double _meanBodyWeight;

      private double[] _organDensity;

      private int _skinIndex;
      private int _fatIndex;

      //parameter only used during the optimization
      private double _objectiveWeight;

      //parameters only used during randomization
      private double? _minWeight;

      private double? _maxWeight;
      private IParameter _weightParameter;

      public CreateIndividualAlgorithm(IContainerTask containerTask, IReportGenerator reportGenerator)
      {
         _containerTask = containerTask;
         _reportGenerator = reportGenerator;
      }

      public void Optimize(Individual individual)
      {
         var randomGenerator = new RandomGenerator(individual.Seed);
         if (individual.IsAgeDependent)
            distributeParameterFor(individual, randomGenerator, optimizeVolumes);
         else
            scaleVolume(individual);
      }

      public void Randomize(Individual individual, IParameter weightParameter, double? minWeight, double? maxWeight, IEnumerable<IParameter> allIndividualParameters, RandomGenerator randomGenerator)
      {
         _minWeight = minWeight;
         _maxWeight = maxWeight;
         _weightParameter = weightParameter;

         try
         {
            if (individual.IsAgeDependent)
               distributeParameterFor(individual, randomGenerator, randomizeAgeDependentVolumes);
            else
               distributeParameterFor(individual, randomGenerator, randomizeWeightAndScaleVolumes);

            randomizeDistributedParameterIn(individual, randomGenerator);
         }
         finally
         {
            _weightParameter = null;
         }
      }

      private void scaleVolume(Individual individual)
      {
         _meanBodyWeight = individual.MeanWeight;
         _objectiveWeight = individual.InputWeight;
         double weightRatio = _objectiveWeight / _meanBodyWeight;
         double[] volumes = createDefaultOrgansVolumesFrom(individual);

         for (int i = 0; i < volumes.Length; i++)
         {
            volumes[i] = volumes[i] * weightRatio;
         }
         setOrganVolumesTo(individual, volumes);
      }

      private void randomizeDistributedParameterIn(Individual individual, RandomGenerator randomGenerator)
      {
         //all distribued parameters in individual that are not standard parameters
         var allDistributedParameters = _containerTask.CacheAllChildrenSatisfying<IDistributedParameter>(
            individual, p => !CoreConstants.Parameter.StandardCreateIndividualParameters.Contains(p.Name));

         foreach (var parameter in allDistributedParameters)
         {
            parameter.Value = parameter.RandomDeviateIn(randomGenerator);
         }
      }

      private void distributeParameterFor(Individual individual, RandomGenerator randomGenerator, Action<Individual, RandomGenerator> action)
      {
         _muSigmas = new List<IMuSigma>();

         try
         {
            _meanBodyWeight = individual.MeanWeight;
            double meanHeight = individual.MeanHeight;

            //relative height of the individual we wish to define
            if (individual.Population.IsHeightDependent)
               _hrel = individual.InputHeight / meanHeight;
            else
               _hrel = 1;

            initializeMusAndSigmas(individual);

            action(individual, randomGenerator);
         }
         finally
         {
            _muSigmas.Clear();
         }
      }

      private void optimizeVolumes(Individual individual, RandomGenerator randomGenerator)
      {
         //Initialization for optimizer
         double p = 0;
         double[] organWeightsInit = null;

         uint numberOfTry = 0;
         _objectiveWeight = individual.InputWeight;

         while (p == 0)
         {
            //Random organ weight
            organWeightsInit = organWeightsFrom(createRandomOrgansVolumesFrom(individual, randomGenerator, getOrganVolumeForIndividual));

            //skale skin
            organWeightsInit[_skinIndex] = getDefaultSkinWeight(_muSigmas[_skinIndex]);

            //compensate with fat to get our target weight
            updateFatWeight(organWeightsInit);

            p = probabilityOrgans(organWeightsInit);
            numberOfTry++;
            if (numberOfTry > MAX_CREATION_TRIES)
               throw new CannotCreateIndividualWithConstraintsException(_reportGenerator.StringReportFor(individual.OriginData));
         }

         //Start optimization
         var result = NelderMeadSimplex.Regress(simplexConstantFrom(organWeightsNonFatFrom(organWeightsInit)), CONVERGENCE_TOLERANCE, MAX_NUMBER_OF_OPTIMIZATION_ITERATIONS, probabilityOrgansWrapper);

         //Retrieve results and scale fat volume to match objective bodyweight
         var organWeights = organWeightsWithFatFrom(transformedWeights(result.Constants));
         setOrganVolumesTo(individual, organVolumesFrom(organWeights));
      }

      private void randomizeWeightAndScaleVolumes(Individual individual, RandomGenerator randomGenerator)
      {
         //default volumes
         var organVolumes = createRandomOrgansVolumesFrom(individual, randomGenerator, getOrganVolumeForPopulation);
         var organWeights = organWeightsFrom(organVolumes);
         double currentWeight = organWeights.Sum();
         double randomWeight = _weightParameter.RandomDeviateIn(randomGenerator, _minWeight, _maxWeight);
         var scaleFactor = randomWeight / currentWeight;
         for (int i = 0; i < organWeights.Length; i++)
         {
            organWeights[i] *= scaleFactor;
         }

         setOrganVolumesTo(individual, organVolumesFrom(organWeights));
      }

      private void randomizeAgeDependentVolumes(Individual individual, RandomGenerator randomGenerator)
      {
         bool success = false;
         uint numberOfTry = 0;
         do
         {
            numberOfTry++;
            var organVolumes = createRandomOrgansVolumesFrom(individual, randomGenerator, getOrganVolumeForPopulation);
            var organWeights = organWeightsFrom(organVolumes);
            var currentWeight = organWeights.Sum();
            if (!currentWeightCloseToTargetWeight(currentWeight)) continue;

            //individual should be kept. Adjust fat weight if required
            organWeights = adjustFatWeightIfRequired(organWeights);
            setOrganVolumesTo(individual, organVolumesFrom(organWeights));
            success = true;
         } while (!success && numberOfTry < MAX_CREATION_TRIES);

         if (numberOfTry >= MAX_CREATION_TRIES)
            throw new CannotCreateIndividualWithConstraintsException(_reportGenerator.StringReportFor(individual.OriginData));
      }

      private double[] adjustFatWeightIfRequired(double[] organWeights)
      {
         if (_minWeight != _maxWeight) return organWeights;
         if (_minWeight == null) return organWeights;

         //configuration where one specific weight is being targeted
         //remove fat: 
         organWeights[_fatIndex] = 0;
         organWeights[_fatIndex] = _minWeight.Value - (organWeights.Sum());

         return organWeights;
      }

      private bool currentWeightCloseToTargetWeight(double currentWeight)
      {
         //no min max specified, weight is valid
         if (!_minWeight.HasValue && !_maxWeight.HasValue)
            return true;

         //min and max specified
         //if the min and max have the same value, return true if the current weight is close to the desired weight
         if (_minWeight.HasValue && _maxWeight.HasValue)
         {
            if (_minWeight == _maxWeight)
               return currentWeight.EqualsByTolerance(_minWeight.Value, _minWeight.Value * 0.05);

            //not equal, true if in the interval
            return currentWeight >= _minWeight && currentWeight <= _maxWeight;
         }

         if (_maxWeight.HasValue)
            return currentWeight <= _maxWeight;

         //min weight as a value
         return currentWeight >= _minWeight;
      }

      private void setOrganVolumesTo(Individual individual, double[] organVolumes)
      {
         int organIndex = 0;
         foreach (var organ in individual.AllOrgans())
         {
            var volumeParameter = organ.Parameter(Constants.Parameters.VOLUME);
            volumeParameter.Value = organVolumes[organIndex];
            volumeParameter.DefaultValue = volumeParameter.Value;
            volumeParameter.IsFixedValue = false;
            organIndex++;
         }
      }

      private double probabilityOrgansWrapper(double[] organWeightsNonFat)
      {
         // double[] transformedWeights = TransformedWeights(organWeights);
         //we have to reset the value of fat so that the sum is still accurate
         var organWeights = organWeightsWithFatFrom(transformedWeights(organWeightsNonFat));

         //return negative value since we want to maximize the function
         return -probabilityOrgans(organWeights);
      }

      private double[] transformedWeights(double[] organWeightsNonFat)
      {
         //     return organWeightsNonFat;
         double[] transformedWeights = new double[organWeightsNonFat.Length];
         for (int i = 0; i < transformedWeights.Count(); i++)
         {
            transformedWeights[i] = Math.Pow(10, organWeightsNonFat[i]);
         }
         return transformedWeights;
      }

      private double[] organWeightsWithFatFrom(double[] organWeightsNonFat)
      {
         var organWeightsWithFat = new double[organWeightsNonFat.Length + 1];

         //Copy all values up to the fat index into the new array
         Array.Copy(organWeightsNonFat, 0, organWeightsWithFat, 0, _fatIndex);

         //add fatvalue
         organWeightsWithFat[_fatIndex] = _objectiveWeight - (organWeightsNonFat.Sum());

         //add rest of values after the fat index
         Array.Copy(organWeightsNonFat, _fatIndex, organWeightsWithFat, _fatIndex + 1, organWeightsNonFat.Length - _fatIndex);
         return organWeightsWithFat;
      }

      private double[] organWeightsNonFatFrom(double[] organWeightsWithFat)
      {
         var organWeightsNonFat = new double[organWeightsWithFat.Length - 1];

         //Copy all values up to the fat index into the new array
         Array.Copy(organWeightsWithFat, 0, organWeightsNonFat, 0, _fatIndex);

         //skip fat and copy rest
         Array.Copy(organWeightsWithFat, _fatIndex + 1, organWeightsNonFat, _fatIndex, organWeightsWithFat.Length - _fatIndex - 1);
         return organWeightsNonFat;
      }

      private void updateFatWeight(double[] organWeightsWithFat)
      {
         organWeightsWithFat[_fatIndex] = _objectiveWeight - (organWeightsWithFat.Sum() - organWeightsWithFat[_fatIndex]);
      }

      private SimplexConstant[] simplexConstantFrom(double[] values)
      {
         var constants = new SimplexConstant[values.Length];
         for (int i = 0; i < values.Length; i++)
         {
            constants[i] = new SimplexConstant(Math.Log10(values[i]), 1);
         }
         return constants;
      }

      private double probabilityOrgans(double[] organWeights)
      {
         double[] organVolumes = organVolumesFrom(organWeights);
         double[] props = new double[organWeights.Length];
         var skinFactor = getSkinScaleFactor(organWeights.Sum());
         for (int i = 0; i < _muSigmas.Count; i++)
         {
            double mu = _muSigmas[i].Mean;
            double sigma = _muSigmas[i].Deviation;
            //skin scaling
            if (i == _skinIndex)
            {
               _muSigmas[i].Mean *= skinFactor;
               _muSigmas[i].Deviation *= skinFactor;
            }

            props[i] = _muSigmas[i].ProbabilityFor(organVolumes[i]);

            //reset skin scaling 
            if (i == _skinIndex)
            {
               _muSigmas[i].Mean = mu;
               _muSigmas[i].Deviation = sigma;
            }
         }

         double p = 1;
         for (int i = 0; i < props.Count(); i++)
         {
            p *= props[i];
         }
         return p;
      }

      private double[] createRandomOrgansVolumesFrom(Individual individual, RandomGenerator randomGenerator, Func<IMuSigma, RandomGenerator, double> generateVolumeFunction)
      {
         var allOrgans = individual.AllOrgans().ToList();
         var organVolumes = new double[allOrgans.Count()];
         _organDensity = new double[allOrgans.Count()];

         int iOrganIndex = 0;
         foreach (var organ in allOrgans)
         {
            var muSigma = _muSigmas[iOrganIndex];
            _organDensity[iOrganIndex] = organ.Parameter(CoreConstants.Parameter.DENSITY).Value;
            organVolumes[iOrganIndex] = generateVolumeFunction(muSigma, randomGenerator);

            if (organ.IsNamed(CoreConstants.Organ.Skin))
               _skinIndex = iOrganIndex;

            if (organ.IsNamed(CoreConstants.Organ.Fat))
               _fatIndex = iOrganIndex;

            iOrganIndex++;
         }

         //Scale skin
         organVolumes[_skinIndex] = getSkinVolume(_muSigmas[_skinIndex], organVolumes, randomGenerator, generateVolumeFunction);

         return organVolumes;
      }

      private double[] createDefaultOrgansVolumesFrom(Individual individual)
      {
         var allOrgans = individual.AllOrgans().ToList();
         var organVolumes = new double[allOrgans.Count()];
         _organDensity = new double[allOrgans.Count()];

         int iOrganIndex = 0;
         foreach (var organ in allOrgans)
         {
            _organDensity[iOrganIndex] = organ.Parameter(CoreConstants.Parameter.DENSITY).Value;
            organVolumes[iOrganIndex] = organ.Parameter(Constants.Parameters.VOLUME).Value;


            if (organ.IsNamed(CoreConstants.Organ.Skin))
               _skinIndex = iOrganIndex;

            if (organ.IsNamed(CoreConstants.Organ.Fat))
               _fatIndex = iOrganIndex;

            iOrganIndex++;
         }

         return organVolumes;
      }

      private void initializeMusAndSigmas(Individual individual)
      {
         //retrieve disitribution parameters for volumes parameters
         foreach (var organ in individual.AllOrgans())
         {
            var volumeParameter = organ.Parameter(Constants.Parameters.VOLUME);
            var allometricScaleFactor = volumeParameter.ParentContainer.Parameter(CoreConstants.Parameter.ALLOMETRIC_SCALE_FACTOR).Value;
            var musSigma = MuSigma.From(volumeParameter);
            musSigma.ScaleWith(_hrel, allometricScaleFactor);
            _muSigmas.Add(musSigma);
         }
      }

      private double[] organWeightsFrom(double[] organVolumes)
      {
         double[] organWeights = new double[organVolumes.Length];
         for (int i = 0; i < organWeights.Count(); i++)
         {
            organWeights[i] = organVolumes[i] * _organDensity[i];
         }

         return organWeights;
      }

      private double[] organVolumesFrom(double[] organWeights)
      {
         double[] organVolumes = new double[organWeights.Length];
         for (int i = 0; i < organVolumes.Count(); i++)
         {
            organVolumes[i] = organWeights[i] / _organDensity[i];
         }
         return organVolumes;
      }

      private double getDefaultSkinWeight(IMuSigma muSigmaSkin)
      {
         return muSigmaSkin.DefaultValue * getSkinScaleFactor(_objectiveWeight);
      }

      private double getSkinScaleFactor(double currentWeight)
      {
         return Math.Pow(currentWeight / (_meanBodyWeight * Math.Pow(_hrel, 2)), 0.5);
      }

      private double getSkinVolume(IMuSigma muSigmaSkin, double[] organVolumes, RandomGenerator randomGenerator, Func<IMuSigma, RandomGenerator, double> generateVolumeFunction)
      {
         double skinScaleFactor = getSkinScaleFactor(organWeightsFrom(organVolumes).Sum());
         double mu = muSigmaSkin.Mean;
         double sigma = muSigmaSkin.Deviation;

         try
         {
            muSigmaSkin.Mean *= skinScaleFactor;
            muSigmaSkin.Deviation *= skinScaleFactor;
            return generateVolumeFunction(muSigmaSkin, randomGenerator);
         }
         finally
         {
            muSigmaSkin.Mean = mu;
            muSigmaSkin.Deviation = sigma;
         }
      }

      private double getOrganVolumeForPopulation(IMuSigma muSigma, RandomGenerator randomGenerator)
      {
         return muSigma.GenerateRandomValueForPopulation(randomGenerator);
      }

      private double getOrganVolumeForIndividual(IMuSigma muSigma, RandomGenerator randomGenerator)
      {
         return muSigma.GenerateRandomValueForIndividual(randomGenerator);
      }
   }
}