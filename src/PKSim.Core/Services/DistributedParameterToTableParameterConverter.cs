using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Maths.Interpolations;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Core.Maths.Statistics;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IDistributedParameterToTableParameterConverter
   {
      /// <summary>
      ///    Create a table parameter for each distributed parameter defined in the simulation subject of the simulation if the
      ///    simulation is aging
      /// </summary>
      /// <param name="buildConfiguration"> spatialStructure that will be use to create the simulation </param>
      /// <param name="simulation"> Model less simulation that whose spatial structure will be created </param>
      /// <param name="createAgindDataInSimulation"></param>
      void UpdateBuildConfigurationForAging(IBuildConfiguration buildConfiguration, Simulation simulation, bool createAgindDataInSimulation);
   }

   public class DistributedParameterToTableParameterConverter : IDistributedParameterToTableParameterConverter
   {
      private readonly IFormulaFactory _formulaFactory;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IParameterFactory _parameterFactory;
      private readonly ICloneManager _cloneManager;
      private readonly IParameterQuery _parameterQuery;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly IFullPathDisplayResolver _fullPathDisplayResolver;
      private readonly IInterpolation _interpolation;
      private readonly IParameterStartValuesCreator _parameterStartValuesCreator;
      private readonly IObjectPathFactory _objectPathFactory;
      private readonly IDimension _timeDimension;
      private readonly Unit _yearUnit;
      private Simulation _simulation;
      private Individual _baseIndividual;
      private OriginData _baseOriginData;
      private IReadOnlyList<ParameterDistributionMetaData> _allHeightDistributionMaleParameters;
      private IReadOnlyList<ParameterDistributionMetaData> _allHeightDistributionFemaleParameters;
      private bool _createAgindDataInSimulation;

      public DistributedParameterToTableParameterConverter(IFormulaFactory formulaFactory, IEntityPathResolver entityPathResolver, IParameterFactory parameterFactory,
         ICloneManager cloneManager, IParameterQuery parameterQuery, IDimensionRepository dimensionRepository, IOntogenyRepository ontogenyRepository,
         IFullPathDisplayResolver fullPathDisplayResolver, IInterpolation interpolation, IParameterStartValuesCreator parameterStartValuesCreator, IObjectPathFactory objectPathFactory)
      {
         _formulaFactory = formulaFactory;
         _entityPathResolver = entityPathResolver;
         _parameterFactory = parameterFactory;
         _cloneManager = cloneManager;
         _parameterQuery = parameterQuery;
         _dimensionRepository = dimensionRepository;
         _ontogenyRepository = ontogenyRepository;
         _fullPathDisplayResolver = fullPathDisplayResolver;
         _interpolation = interpolation;
         _parameterStartValuesCreator = parameterStartValuesCreator;
         _objectPathFactory = objectPathFactory;
         _timeDimension = dimensionRepository.Time;
         _yearUnit = _timeDimension.Unit(dimensionRepository.AgeInYears.BaseUnit.Name);
      }

      public void UpdateBuildConfigurationForAging(IBuildConfiguration buildConfiguration, Simulation simulation, bool createAgindDataInSimulation)
      {
         if (!simulation.AllowAging)
            return;

         try
         {
            _simulation = simulation;
            _createAgindDataInSimulation = createAgindDataInSimulation;
            _baseIndividual = simulation.Individual;
            _baseOriginData = _baseIndividual.OriginData;
            var allHeightDistributionParameters = _parameterQuery.ParameterDistributionsFor(_baseIndividual.Organism, _baseOriginData.SpeciesPopulation, _baseOriginData.SubPopulation, CoreConstants.Parameter.MEAN_HEIGHT);
            _allHeightDistributionMaleParameters = allHeightDistributionParameters.Where(p => p.Gender == CoreConstants.Gender.Male).ToList();
            _allHeightDistributionFemaleParameters = allHeightDistributionParameters.Where(p => p.Gender == CoreConstants.Gender.Female).ToList();
            createSpatialStructureTableParameters(buildConfiguration);
            createOntogenyTableParameters(buildConfiguration);

            updateAgeParameter(buildConfiguration);
         }
         finally
         {
            _simulation = null;
            _baseIndividual = null;
            _baseOriginData = null;
         }
      }

      private void updateAgeParameter(IBuildConfiguration buildConfiguration)
      {
         var spatialStructure = buildConfiguration.SpatialStructure;
         var organism = spatialStructure.TopContainers.FindByName(Constants.ORGANISM);
         var ageParameter = organism.Parameter(CoreConstants.Parameter.AGE);
         var minToYearFactor = _timeDimension.BaseUnitValueToUnitValue(_yearUnit, 1);
         var age0Parameter = _parameterFactory.CreateFor(CoreConstants.Parameter.AGE_0, ageParameter.Value, ageParameter.Dimension.Name, PKSimBuildingBlockType.Simulation);
         age0Parameter.DisplayUnit = ageParameter.DisplayUnit;
         age0Parameter.Visible = false;

         var minToYearFactorParameter = _parameterFactory.CreateFor(CoreConstants.Parameter.MIN_TO_YEAR_FACTOR, minToYearFactor, PKSimBuildingBlockType.Simulation);
         minToYearFactorParameter.Visible = false;

         organism.Add(age0Parameter);
         organism.Add(minToYearFactorParameter);

         var formula = _formulaFactory.AgeFormulaFor(age0Parameter, minToYearFactorParameter);
         updateConstantParameterToFormula(ageParameter, formula, buildConfiguration);
         addNewParametersToParameterStartValues(buildConfiguration, age0Parameter, minToYearFactorParameter);
      }

      private void addNewParametersToParameterStartValues(IBuildConfiguration buildConfiguration, IParameter age0Parameter, IParameter minToYearFactorParameter)
      {
         var psv = buildConfiguration.ParameterStartValues;
         addParameterToParameterStartValues(psv, age0Parameter);
         addParameterToParameterStartValues(psv, minToYearFactorParameter);
      }

      private void addParameterToParameterStartValues(IParameterStartValuesBuildingBlock parameterStartValuesBuildingBlock, IParameter parameter)
      {
         var path = _objectPathFactory.CreateAbsoluteObjectPath(parameter);
         var psv =_parameterStartValuesCreator.CreateParameterStartValue(path, parameter);
         parameterStartValuesBuildingBlock[psv.Path] = psv;
      }

      private void createSpatialStructureTableParameters(IBuildConfiguration buildConfiguration)
      {
         var spatialStructure = buildConfiguration.SpatialStructure;
         var allBaseIndividualDistributedParameters = new PathCache<IDistributedParameter>(_entityPathResolver).For(_baseIndividual.GetAllChildren<IDistributedParameter>(parameterShouldBeDefinedAsTable));
         if (!allBaseIndividualDistributedParameters.Any())
            return;

         var simulationPopulation = _simulation as PopulationSimulation;

         //Some special cache to optimize speed
         var allContainerParameters = new PathCache<IDistributedParameter>(_entityPathResolver).For(spatialStructure.TopContainers.SelectMany(x => x.GetAllChildren<IDistributedParameter>()));
         var allNeighborhoodParameters = new PathCache<IDistributedParameter>(_entityPathResolver).For(spatialStructure.Neighborhoods.SelectMany(x => x.GetAllChildren<IDistributedParameter>()));
         foreach (var baseIndividualParameter in allBaseIndividualDistributedParameters.KeyValues)
         {
            var structureParameter = allContainerParameters[baseIndividualParameter.Key] ?? allNeighborhoodParameters[baseIndividualParameter.Key];
            if (structureParameter == null) continue;

            //cache all distributions for this parameter defined for the population and sub population.
            var allDistributionsForParameter = _parameterQuery.ParameterDistributionsFor(baseIndividualParameter.Value.ParentContainer, _baseOriginData.SpeciesPopulation, _baseOriginData.SubPopulation, baseIndividualParameter.Value.Name);
            var allDistributionsForMaleParameter = allDistributionsForParameter.Where(p => p.Gender == CoreConstants.Gender.Male).ToList();
            var allDistributionsForFemaleParameter = allDistributionsForParameter.Where(p => p.Gender == CoreConstants.Gender.Female).ToList();

            createSpatialStructureTableParameter(structureParameter, baseIndividualParameter.Value, allDistributionsForMaleParameter, allDistributionsForFemaleParameter, buildConfiguration);

            createPopulationTableParameter(baseIndividualParameter, simulationPopulation, allDistributionsForMaleParameter, allDistributionsForFemaleParameter);
         }
      }

      private void createOntogenyTableParameters(IBuildConfiguration buildConfiguration)
      {
         var simulationPopulation = _simulation as PopulationSimulation;

         foreach (var molecule in _baseIndividual.AllMolecules().Where(m => m.Ontogeny.IsDefined()))
         {
            var ontogenyFactorPath = _entityPathResolver.ObjectPathFor(molecule.OntogenyFactorParameter);
            var ontogenyFactorGIPath = _entityPathResolver.ObjectPathFor(molecule.OntogenyFactorGIParameter);
            createParameterValueVersionOntogenyTableParameter(molecule.OntogenyFactorParameter, buildConfiguration, ontogenyFactorPath, molecule);
            createParameterValueVersionOntogenyTableParameter(molecule.OntogenyFactorGIParameter, buildConfiguration, ontogenyFactorGIPath, molecule);

            createPopulationOntogenyTableParameter(molecule.OntogenyFactorParameter, ontogenyFactorPath, molecule, simulationPopulation);
            createPopulationOntogenyTableParameter(molecule.OntogenyFactorGIParameter, ontogenyFactorGIPath, molecule, simulationPopulation);
         }

         createPlasmaProteinOntogenyTable(buildConfiguration);
      }

      private void createPlasmaProteinOntogenyTable(IBuildConfiguration buildConfiguration)
      {
         var spatialStructure = buildConfiguration.SpatialStructure;
         var organism = spatialStructure.TopContainers.FindByName(Constants.ORGANISM);
         foreach (var ontogenyParameterName in CoreConstants.Parameter.AllPlasmaProteinOntogenyFactors)
         {
            var parameter = organism.Parameter(ontogenyParameterName);
            var formula = createPlasmaProteinOntogenyTableFormulaFrom(parameter, _baseOriginData);
            if (formula == null) continue;

            updateConstantParameterToFormula(parameter, formula, buildConfiguration);
            createPopulationPlasmaProteinOntogenyTableParameter(parameter, _simulation as PopulationSimulation);
         }
      }

      private void createParameterValueVersionOntogenyTableParameter(IParameter ontogenyFactorParameter,
         IBuildConfiguration buildConfiguration, IObjectPath ontogenyFactorPath, IndividualMolecule molecule)
      {
         var psv = buildConfiguration.ParameterStartValues;
         var parameterStartValue = psv[ontogenyFactorPath];

         parameterStartValue.Formula = createOntogenyTableFormulaFrom(ontogenyFactorParameter, molecule.Ontogeny, _baseOriginData);
         if (parameterStartValue.Formula != null)
         {
            parameterStartValue.StartValue = null;
            psv.FormulaCache.Add(parameterStartValue.Formula);
         }
      }

      private TableFormula createMoleculeOntogenyTableFormula(IParameter ontogenyFactor, OriginData originData, IReadOnlyList<Sample> allOntogenies)
      {
         //null is ok here. It's the default value for formula in ParameterStartValue
         if (allOntogenies.Count == 0)
            return null;

         var tableFormula = _formulaFactory.CreateTableFormula();
         updateTableFormulaFrom(tableFormula, ontogenyFactor);

         //0 because of the offset with age
         tableFormula.AddPoint(0, ontogenyFactor.Value);

         foreach (var ontogenyForAge in allOntogenies)
         {
            var age = ageWithOffsetInMin(ontogenyForAge.X, originData.Age.Value);
            tableFormula.AddPoint(age, ontogenyForAge.Y);
         }

         tableFormula.UseDerivedValues = false;

         return tableFormula;
      }

      private TableFormula createOntogenyTableFormulaFrom(IParameter ontogenyFactor, Ontogeny ontogeny, OriginData originData, RandomGenerator randomize = null)
      {
         var containerName = containerNameForOntogenyFactor(ontogenyFactor);

         var allOntogenies = _ontogenyRepository.AllOntogenyFactorForStrictBiggerThanPMA(ontogeny, originData, containerName, randomize).ToList();
         return createMoleculeOntogenyTableFormula(ontogenyFactor, originData, allOntogenies);
      }

      private static string containerNameForOntogenyFactor(IParameter ontogenyFactor)
      {
         if (ontogenyFactor.IsNamed(CoreConstants.Parameter.ONTOGENY_FACTOR_GI))
            return CoreConstants.Groups.ONTOGENY_DUODENUM;

         return CoreConstants.Groups.ONTOGENY_LIVER;
      }

      private TableFormula createPlasmaProteinOntogenyTableFormulaFrom(IParameter ontogenyFactor, OriginData originData, RandomGenerator randomize = null)
      {
         var allOntogenies = _ontogenyRepository.AllPlasmaProteinOntogenyFactorForStrictBiggerThanPMA(ontogenyFactor.Name, originData, randomize).ToList();
         return createMoleculeOntogenyTableFormula(ontogenyFactor, originData, allOntogenies);
      }

      private bool parameterShouldBeDefinedAsTable(IDistributedParameter parameter)
      {
         return !parameter.NameIsOneOf(CoreConstants.Parameter.MEAN_HEIGHT, CoreConstants.Parameter.MEAN_WEIGHT);
      }

      private void createPopulationTableParameter(KeyValuePair<string, IDistributedParameter> individualParameter, PopulationSimulation populationSimulation,
         IEnumerable<ParameterDistributionMetaData> allDistributionsForMaleParameter, IEnumerable<ParameterDistributionMetaData> allDistributionsForFemaleParameter)
      {
         if (populationSimulation == null) return;
         var parameter = individualParameter.Value;

         addAgingDataToPopulationSimulation(populationSimulation, individualParameter.Key, parameter,
            p =>
            {
               var distributions = allDistributionsWithAgeStrictBiggerThanOriginData(allDistributionsForMaleParameter, allDistributionsForFemaleParameter, p.OriginData).ToList();
               return createTableFormulaFrom(p, distributions);
            });
      }

      private void createPopulationOntogenyTableParameter(IParameter ontogenyFactorParameter, IObjectPath ontogenyFactorPath, IndividualMolecule molecule, PopulationSimulation populationSimulation)
      {
         addAgingDataToPopulationSimulation(populationSimulation, ontogenyFactorPath.ToString(), ontogenyFactorParameter,
            p => createOntogenyTableFormulaFrom(p.Parameter, molecule.Ontogeny, p.OriginData, populationSimulation.RandomGenerator));
      }

      private void createPopulationPlasmaProteinOntogenyTableParameter(IParameter ontogenyFactorParameter, PopulationSimulation populationSimulation)
      {
         if (populationSimulation == null) return;

         var parameterPath = _entityPathResolver.PathFor(ontogenyFactorParameter);
         addAgingDataToPopulationSimulation(populationSimulation, parameterPath, ontogenyFactorParameter,
            p => createPlasmaProteinOntogenyTableFormulaFrom(ontogenyFactorParameter, _baseOriginData, populationSimulation.RandomGenerator));
      }

      private void addAgingDataToPopulationSimulation<TParameter>(PopulationSimulation populationSimulation, string parameterPath, TParameter parameter,
         Func<TableFormulaParameter<TParameter>, TableFormula> tableFormulaRetriever) where TParameter : IParameter
      {
         if (populationSimulation == null || !_createAgindDataInSimulation) return;

         var originData = _baseOriginData.Clone();
         var allAges = populationSimulation.AllOrganismValuesFor(CoreConstants.Parameter.AGE, _entityPathResolver);
         var allGAs = populationSimulation.AllOrganismValuesFor(CoreConstants.Parameter.GESTATIONAL_AGE, _entityPathResolver);
         var allHeights = populationSimulation.AllOrganismValuesFor(CoreConstants.Parameter.HEIGHT, _entityPathResolver);
         var allGender = populationSimulation.AllGenders.ToList();
         var allValues = populationSimulation.AllValuesFor(parameterPath).ToList();
         var allPercentiles = populationSimulation.AllPercentilesFor(parameterPath)
            .Select(x => x.CorrectedPercentileValue()).ToList();

         var tableFormulaParameter = new TableFormulaParameter<TParameter> {OriginData = originData, Parameter = parameter};
         for (int individualIndex = 0; individualIndex < populationSimulation.NumberOfItems; individualIndex++)
         {
            //create orgin data for individual i
            originData.Age = allAges[individualIndex];
            originData.GestationalAge = allGAs[individualIndex];
            originData.Height = allHeights[individualIndex];
            originData.Gender = allGender[individualIndex];
            tableFormulaParameter.Value = allValues[individualIndex];
            tableFormulaParameter.Percentile = allPercentiles[individualIndex];

            var tableFormula = tableFormulaRetriever(tableFormulaParameter);
            if (tableFormula == null)
               continue;

            populationSimulation.AddAgingTableFormula(parameterPath, individualIndex, tableFormula);
         }
      }

      private void createSpatialStructureTableParameter(IDistributedParameter structureParameter, IDistributedParameter baseIndividualParameter,
         IEnumerable<ParameterDistributionMetaData> distributionsForMale, IEnumerable<ParameterDistributionMetaData> distributionsForFemale, IBuildConfiguration buildConfiguration)
      {
         var allDistributionsForParameter = allDistributionsWithAgeStrictBiggerThanOriginData(distributionsForMale, distributionsForFemale, _baseIndividual.OriginData).ToList();
         if (allDistributionsForParameter.Count == 0)
            return;

         //remove the parameter from the parent container and add a new one that will contain the table formula
         //retrieve the table formula corresponding to the individual values
         var newParameter = _parameterFactory.CreateFor(structureParameter.Name, structureParameter.BuildingBlockType);
         newParameter.UpdatePropertiesFrom(structureParameter, _cloneManager);
         var parentContainer = structureParameter.ParentContainer;
         parentContainer.RemoveChild(structureParameter);
         parentContainer.Add(newParameter);
         newParameter.Editable = false;
         var formula = createTableFormulaFrom(baseIndividualParameter, allDistributionsForParameter);
         updateConstantParameterToFormula(newParameter, formula, buildConfiguration);
      }

      private IEnumerable<ParameterDistributionMetaData> allDistributionsWithAgeStrictBiggerThanOriginData(
         IEnumerable<ParameterDistributionMetaData> distributionsForMale, IEnumerable<ParameterDistributionMetaData> distributionsForFemale, OriginData originData)
      {
         var distributions = distributionsForMale;
         if (originData.Gender.Name == CoreConstants.Gender.Female)
            distributions = distributionsForFemale;

         return distributions
            .Where(d => d.Age > originData.Age)
            .DefinedFor(originData);
      }

      private IEnumerable<ParameterDistributionMetaData> allDistributionsFor(
         IEnumerable<ParameterDistributionMetaData> distributionsForMale,
         IEnumerable<ParameterDistributionMetaData> distributionsForFemale, OriginData originData)
      {
         var distributions = distributionsForMale;
         if (originData.Gender.Name == CoreConstants.Gender.Female)
            distributions = distributionsForFemale;

         return distributions.DefinedFor(originData);
      }

      private TableFormula createTableFormulaFrom(IDistributedParameter individualParameter, IEnumerable<ParameterDistributionMetaData> allDistributionsWithAgeStrictBiggerThanOriginData)
      {
         var parameter = new TableFormulaParameter<IDistributedParameter>
         {
            OriginData = _baseOriginData,
            Parameter = individualParameter,
            Value = individualParameter.Value,
            Percentile = individualParameter.Percentile
         };

         return createTableFormulaFrom(parameter, allDistributionsWithAgeStrictBiggerThanOriginData.ToList());
      }

      private TableFormula createTableFormulaFrom(TableFormulaParameter<IDistributedParameter> parameter, List<ParameterDistributionMetaData> allDistributionsWithAgeStrictBiggerThanOriginData)
      {
         if (allDistributionsWithAgeStrictBiggerThanOriginData.Count == 0)
            return null;

         var tableFormula = _formulaFactory.CreateDistributedTableFormula();
         updateTableFormulaFrom(tableFormula, parameter.Parameter);
         tableFormula.Percentile = parameter.Percentile;

         if (parameter.PercentileIsInvalid)
            throw new PKSimException(PKSimConstants.Error.CannotCreateAgingSimulationWithInvalidPercentile(_fullPathDisplayResolver.FullPathFor(parameter.Parameter), parameter.Percentile));

         //0 because of the offset with age
         tableFormula.AddPoint(0, parameter.Value, DistributionMetaData.From(parameter.Parameter));

         foreach (var scaledDistribution in scaleDistributions(parameter, allDistributionsWithAgeStrictBiggerThanOriginData))
         {
            var age = ageWithOffsetInMin(scaledDistribution.Age, parameter.OriginData.Age.Value);
            var value = valueFrom(scaledDistribution, parameter.Percentile);
            tableFormula.AddPoint(age, value, DistributionMetaData.From(scaledDistribution));
         }

         return tableFormula;
      }

      private IEnumerable<ParameterDistributionMetaData> scaleDistributions(TableFormulaParameter<IDistributedParameter> parameter, IEnumerable<ParameterDistributionMetaData> distributionsToScale)
      {
         //Retrieve the mean height parameter for the given origin data
         var originData = parameter.OriginData;
         var individualParameter = parameter.Parameter;

         bool needHeightScaling = needScaling(originData, individualParameter);
         if (!needHeightScaling)
            return distributionsToScale;

         //retrieve the height distribution for the original individual 
         var heightDistributions = allDistributionsFor(_allHeightDistributionMaleParameters, _allHeightDistributionFemaleParameters, originData).ToList();

         var heightDistributionParameters = distributionParametersFor(heightDistributions, originData);

         double meanHeight = heightDistributionParameters.Item1;
         double deviation = heightDistributionParameters.Item2;
         var heigthDistributionFormula = distributionFrom(DistributionTypes.Normal, meanHeight, deviation);

         double currentHeight = originData.Height.GetValueOrDefault(meanHeight);

         //same height, not need to scale
         if (ValueComparer.AreValuesEqual(meanHeight, currentHeight))
            return distributionsToScale;

         //is used in order to retrieve the percentile 
         double currentPercentile = heigthDistributionFormula.CalculatePercentileForValue(currentHeight);
         double alpha = individualParameter.ParentContainer.Parameter(CoreConstants.Parameter.ALLOMETRIC_SCALE_FACTOR).Value;

         var currentOriginData = originData.Clone();
         var scaledParameterDistributionMetaData = new List<ParameterDistributionMetaData>();
         foreach (var originDistributionMetaData in distributionsToScale)
         {
            var distributionMetaData = ParameterDistributionMetaData.From(originDistributionMetaData);
            currentOriginData.Age = originDistributionMetaData.Age;
            double hrelForAge = calculateRelativeHeightForAge(heightDistributions, currentOriginData, currentPercentile);

            scaleDistributionMetaData(distributionMetaData, hrelForAge, alpha);
            scaledParameterDistributionMetaData.Add(distributionMetaData);
         }

         return scaledParameterDistributionMetaData;
      }

      private double calculateRelativeHeightForAge(IEnumerable<ParameterDistributionMetaData> heightDistribution, OriginData originData, double startHeightPercentile)
      {
         var parameters = distributionParametersFor(heightDistribution, originData);
         var mean = parameters.Item1;
         var deviation = parameters.Item2;
         double currentHeight = valueFrom(DistributionTypes.Normal, mean, deviation, startHeightPercentile);
         return currentHeight / mean;
      }

      private Tuple<double, double> distributionParametersFor(IEnumerable<ParameterDistributionMetaData> distributions, OriginData originData)
      {
         var knownSamples = from distribution in distributions
            select new
            {
               Mean = new Sample(distribution.Age, distribution.Mean),
               Std = new Sample(distribution.Age, distribution.Deviation),
            };

         knownSamples = knownSamples.ToList();

         double mean = _interpolation.Interpolate(knownSamples.Select(item => item.Mean), originData.Age.Value);
         double deviation = _interpolation.Interpolate(knownSamples.Select(item => item.Std), originData.Age.Value);

         return new Tuple<double, double>(mean, deviation);
      }

      private static bool needScaling(OriginData originData, IDistributedParameter individualParameter)
      {
         if (!originData.SpeciesPopulation.IsHeightDependent)
            return false;

         if (!individualParameter.IsNamed(Constants.Parameters.VOLUME))
            return false;

         if (!individualParameter.ParentContainer.IsAnImplementationOf<Organ>())
            return false;

         //Volume in organ
         return true;
      }

      private IDistribution distributionFrom(DistributionType distributionType, double mean, double deviation)
      {
         if (distributionType == DistributionTypes.LogNormal)
            return new LogNormalDistribution(Math.Log(mean), Math.Log(deviation));

         if (distributionType == DistributionTypes.Normal)
            return new NormalDistribution(mean, deviation);

         return new UniformDistribution(mean, mean);
      }

      private double valueFrom(ParameterDistributionMetaData distributionMetaData, double percentile)
      {
         return valueFrom(distributionMetaData.Distribution, distributionMetaData.Mean, distributionMetaData.Deviation, percentile);
      }

      private double valueFrom(DistributionType distributionType, double mean, double deviation, double percentile)
      {
         return distributionFrom(distributionType, mean, deviation).CalculateValueFromPercentile(percentile);
      }

      private void scaleDistributionMetaData(ParameterDistributionMetaData parameterDistributionMeta, double hrel, double alpha)
      {
         var scale = Math.Pow(hrel, alpha);
         parameterDistributionMeta.Mean *= scale;
         if (parameterDistributionMeta.DistributionType == CoreConstants.Distribution.Normal)
         {
            parameterDistributionMeta.Deviation *= scale;
         }
      }

      private void updateTableFormulaFrom(TableFormula tableFormula, IParameter parameter)
      {
         tableFormula.Name = _fullPathDisplayResolver.FullPathFor(parameter);
         tableFormula.InitializedWith(PKSimConstants.UI.SimulationTime, parameter.Name, _dimensionRepository.Time, parameter.Dimension);
         tableFormula.XDisplayUnit = _yearUnit;
      }

      private double ageWithOffsetInMin(double ageInYears, double originDataAgeInYears)
      {
         var futureAge = ageInYears - originDataAgeInYears;
         return _timeDimension.UnitValueToBaseUnitValue(_yearUnit, futureAge);
      }

      private void updateConstantParameterToFormula(IParameter parameter, IFormula formula, IBuildConfiguration buildConfiguration)
      {
         parameter.Formula = formula;
         buildConfiguration.SpatialStructure.AddFormula(formula);
         removeParameterStartValueFor(parameter, buildConfiguration.ParameterStartValues);
      }

      private void removeParameterStartValueFor(IParameter parameter, IParameterStartValuesBuildingBlock parameterStartValuesBuildingBlock)
      {
         parameterStartValuesBuildingBlock.Remove(_entityPathResolver.ObjectPathFor(parameter));
      }

      /// <summary>
      ///    Help class used to collect parameters required to create a table parametters
      /// </summary>
      private class TableFormulaParameter<TParameter> where TParameter : IParameter
      {
         /// <summary>
         ///    Origin Data for which the table should be created. This is not necessarily the origin data from the individual
         ///    when used in a population
         /// </summary>
         public OriginData OriginData { get; set; }

         /// <summary>
         ///    The actual parameter for which a table should be generated
         /// </summary>
         public TParameter Parameter { get; set; }

         /// <summary>
         ///    The value of the parameter. This is not necessarily the value of the parameter when used in a population
         /// </summary>
         public double Value { get; set; }

         /// <summary>
         ///    The percentile in the distribution for the Value; This is not necessarily the percentile of the parameter when used
         ///    in a population
         ///    This is also only used if the parameter is distributed
         /// </summary>
         public double Percentile { get; set; }

         public bool PercentileIsInvalid => !Percentile.IsValidPercentile();
      }
   }
}