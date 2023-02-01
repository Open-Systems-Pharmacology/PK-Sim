using System;
using System.IO;
using FakeItEasy;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.DTO.Protocols;

namespace PKSim.Core
{
   public static class DomainHelperForSpecs
   {
      public static string ParameterInOrganism = "POrganism";
      private static readonly Random _random = new Random();
      private static Dimension _lengthDimension;
      private static Dimension _concentrationDimension;
      private static Dimension _timeDimension;
      private static Dimension _massConcentrationDimension;
      private static Dimension _fractionDimension;

      private static readonly string PATH_TO_SRC = "..\\..\\..\\..\\..\\src\\";
      private static readonly string PATH_TO_DATA = "..\\..\\..\\Data\\";
      private static readonly string PATH_TO_TEMPLATES = "..\\..\\..\\Templates\\";

      public static string FilePathFor(string fileNameWithExtension)
      {
         return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileNameWithExtension);
      }

      public static string DataFilePathFor(string fileNameWithExtension)
      {
         return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PATH_TO_DATA, fileNameWithExtension);
      }

      public static string PopulationFilePathFor(string fileNameWithoutExtension)
      {
         return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PATH_TO_DATA, "PopulationFiles", fileNameWithoutExtension + ".csv");
      }

      public static string SimulationResultsFilePathFor(string fileNameWithoutExtension)
      {
         return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PATH_TO_DATA, "SimulationResultsFiles", fileNameWithoutExtension + ".csv");
      }

      public static string PKAnalysesFilePathFor(string fileNameWithoutExtension)
      {
         return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PATH_TO_DATA, "PKAnalysesFiles", fileNameWithoutExtension + ".csv");
      }

      public static string TEXTemplateFolder()
      {
         return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PATH_TO_TEMPLATES, "StandardTemplate");
      }

      public static string UserTemplateDatabasePath()
      {
         return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PATH_TO_SRC, "Db\\TemplateDB", "PKSimTemplateDBUser.template");
      }

      public static string SystemTemplateDatabasePath()
      {
         return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PATH_TO_SRC, "Db\\TemplateDB", "PKSimTemplateDBSystem.mdb");
      }

      public static ExpressionProfile CreateExpressionProfile<TMolecule>(string speciesName = "Species", string moleculeName = "CYP3A4", string category = "Healthy") where TMolecule : IndividualMolecule, new()
      {
         var expressionProfile = new ExpressionProfile();
         var individual = CreateIndividual(speciesName);
         individual.AddMolecule(new TMolecule().WithName(moleculeName));
         expressionProfile.Individual = individual;
         expressionProfile.Category = category;
         return expressionProfile;
      }

      public static Individual CreateIndividual(string speciesName = "Species")
      {
         var originData = new OriginData
         {
            SubPopulation = new SubPopulation(),
            Species = new Species {DisplayName = speciesName, Id = speciesName}.WithName(speciesName),
            Gender = new Gender().WithName("Gender"),
            Population = new SpeciesPopulation().WithName("Population"),
         };

         var pvv = new ParameterValueVersion().WithName("PVVName");
         var category = new ParameterValueVersionCategory().WithName("CategoryName");
         category.Add(pvv);
         originData.SubPopulation.AddParameterValueVersion(pvv);
         var organism = new Organism().WithName(Constants.ORGANISM).WithId("OrganismId");
         var organLiver = new Organ().WithName(CoreConstants.Organ.LIVER).WithId("LiverId");
         organLiver.OrganType = OrganType.TissueOrgansNotInGiTract;
         var periportal = new Compartment().WithName(CoreConstants.Compartment.PERIPORTAL).WithId("PeriportalId");
         periportal.Add(new Container().WithName(CoreConstants.Compartment.INTRACELLULAR).WithId("PeriportalIntracellular"));
         var pericentral = new Compartment().WithName(CoreConstants.Compartment.PERICENTRAL).WithId("PericentralId");
         pericentral.Add(new Container().WithName(CoreConstants.Compartment.INTRACELLULAR).WithId("PericentralIntracellular"));
         organLiver.Add(periportal);
         organLiver.Add(pericentral);

         var organKidney = new Organ().WithName(CoreConstants.Organ.KIDNEY).WithId("KidneyId");
         organKidney.OrganType = OrganType.TissueOrgansNotInGiTract;
         organKidney.Add(new Compartment {Visible = true}.WithName(CoreConstants.Compartment.PLASMA).WithId("KidneyPlasma"));
         var lumen = new Organ().WithName("GiTract").WithId("GiTract");
         lumen.OrganType = OrganType.Lumen;

         var duodenum = new Compartment().WithName("Duodenum").WithId("Duodenum");
         var caecum = new Compartment().WithName("Caecum").WithId("Caecum");
         var dimension = Constants.Dimension.NO_DIMENSION;

         var parameterLiver = new PKSimParameter().WithName("PLiver").WithId("PLiverId").WithFormula(new ExplicitFormula("1").WithId("1")).WithDimension(dimension);
         parameterLiver.Info = new ParameterValueMetaData {DefaultValue = 1};
         var parameterKidney = new PKSimParameter().WithName("PKidney").WithId("PKidneyId").WithFormula(new ExplicitFormula("2").WithId("2")).WithDimension(dimension);
         parameterKidney.Info = new ParameterValueMetaData {DefaultValue = 2};
         var parameterOrganism = new PKSimParameter().WithName(ParameterInOrganism).WithId("POrganismId").WithFormula(new ExplicitFormula("3").WithId("3")).WithDimension(dimension);
         parameterOrganism.Info = new ParameterValueMetaData {DefaultValue = 3};
         organLiver.Add(parameterLiver);
         organKidney.Add(parameterKidney);
         organism.Add(organLiver);
         organism.Add(organKidney);
         organism.Add(lumen);
         organism.Add(parameterOrganism);
         lumen.Add(duodenum);
         lumen.Add(caecum);

         var individual = new Individual {OriginData = originData}.WithName("Individual").WithId("IndividualId");
         individual.Add(organism);

         return individual;
      }

      public static IParameter ConstantParameterWithValue(double value = 10, bool isDefault = false, bool visible = true)
      {
         var parameter = new PKSimParameter().WithFormula(new ConstantFormula(value).WithId("constantFormulaId"));
         parameter.Visible = visible;
         addDimensionTo(parameter);
         parameter.IsFixedValue = true;
         parameter.IsDefault = isDefault;
         return parameter;
      }

      public static IDistributedParameter NormalDistributedParameter(double defaultMean = 0, double defaultDeviation = 1, double defaultPercentile = 0.5, bool isDefault = false, bool distributionParameterIsDefault = true)
      {
         var parameter = new PKSimDistributedParameter().WithId("P1");
         parameter.IsDefault = isDefault;
         var pathFactory = new ObjectPathFactoryForSpecs();
         var meanParameter = new PKSimParameter {Name = Constants.Distribution.MEAN}.WithFormula(new ConstantFormula(defaultMean).WithId("MeanFormula")).WithId("Mean");
         meanParameter.IsDefault = distributionParameterIsDefault;
         addDimensionTo(meanParameter);
         var stdParameter = new PKSimParameter {Name = Constants.Distribution.DEVIATION}.WithFormula(new ConstantFormula(defaultDeviation).WithId("DeviationFormula")).WithId("Deviation");
         stdParameter.IsDefault = distributionParameterIsDefault;
         addDimensionTo(stdParameter);
         var percentileParameter = new PKSimParameter {Name = Constants.Distribution.PERCENTILE}.WithFormula(new ConstantFormula(defaultPercentile).WithId("PercentileFormula")).WithId("Percentile");
         percentileParameter.IsDefault = distributionParameterIsDefault;
         addDimensionTo(percentileParameter);
         parameter.Add(meanParameter);
         parameter.Add(stdParameter);
         parameter.Add(percentileParameter);
         parameter.Formula = new NormalDistributionFormula().WithId("NormalDistributionFormula");
         parameter.Formula.AddObjectPath(pathFactory.CreateRelativeFormulaUsablePath(parameter, meanParameter));
         parameter.Formula.AddObjectPath(pathFactory.CreateRelativeFormulaUsablePath(parameter, stdParameter));
         addDimensionTo(parameter);
         return parameter;
      }

      private static void addDimensionTo(IParameter parameter)
      {
         var dimension = LengthDimensionForSpecs();
         parameter.Dimension = dimension;
      }

      public static IDimension LengthDimensionForSpecs()
      {
         if (_lengthDimension == null)
         {
            _lengthDimension = new Dimension(new BaseDimensionRepresentation {LengthExponent = 1}, "Length", "m");
            _lengthDimension.AddUnit(new Unit("cm", 0.01, 0));
            _lengthDimension.AddUnit(new Unit("mm", 0.001, 0));
         }

         return _lengthDimension;
      }

      public static IDimension TimeDimensionForSpecs()
      {
         if (_timeDimension == null)
         {
            _timeDimension = new Dimension(new BaseDimensionRepresentation {TimeExponent = 1}, Constants.Dimension.TIME, "min");
            _timeDimension.AddUnit(new Unit("h", 60, 0));
            _timeDimension.AddUnit(new Unit("day(s)", 60 * 24, 0));
         }

         return _timeDimension;
      }

      public static IDimension ConcentrationDimensionForSpecs()
      {
         if (_concentrationDimension == null)
         {
            _concentrationDimension = new Dimension(new BaseDimensionRepresentation {AmountExponent = 3, LengthExponent = -1}, Constants.Dimension.MOLAR_CONCENTRATION, "µmol/l");
            _concentrationDimension.AddUnit(new Unit("mol/l", 1E6, 0));
         }

         return _concentrationDimension;
      }

      public static IDimension MassConcentrationDimensionForSpecs()
      {
         if (_massConcentrationDimension == null)
         {
            _massConcentrationDimension = new Dimension(new BaseDimensionRepresentation {MassExponent = 3, LengthExponent = -1}, Constants.Dimension.MASS_CONCENTRATION, "µg/l");
            _massConcentrationDimension.AddUnit(new Unit("g/l", 1E6, 0));
         }

         return _massConcentrationDimension;
      }

      public static IDimension FractionDimensionForSpecs()
      {
         if (_fractionDimension == null)
         {
            _fractionDimension = new Dimension(new BaseDimensionRepresentation(), Constants.Dimension.FRACTION, "");
            _fractionDimension.AddUnit(new Unit("%", 1e-2, 0));
         }

         return _fractionDimension;
      }

      public static IDimension NoDimension()
      {
         return Constants.Dimension.NO_DIMENSION;
      }

      public static SimulationResults CreateSimulationResults(int numberOfIndividuals = 10, int numberOfPaths = 5, int numberOfPoints = 100)
      {
         var populationResults = new SimulationResults {Time = createTimeArray(numberOfPoints)};

         for (int i = 0; i < numberOfIndividuals; i++)
         {
            var results = newIndividualResults(i, numberOfPaths, numberOfPoints);
            populationResults.Add(results);
            results.Time = populationResults.Time;
            results.UpdateQuantityTimeReference();
         }

         return populationResults;
      }

      public static DataRepository ObservedData(string id = "TestData")
      {
         var observedData = new DataRepository(id);
         var baseGrid = new BaseGrid("Time", TimeDimensionForSpecs())
         {
            Values = new[] {1.0f, 2.0f, 3.0f}
         };
         observedData.Add(baseGrid);

         var data = new DataColumn("Col", ConcentrationDimensionForSpecs(), baseGrid)
         {
            Values = new[] {10f, 20f, 30f},
            DataInfo = {Origin = ColumnOrigins.Observation}
         };
         observedData.Add(data);

         return observedData;
      }

      public static IdentificationParameter IdentificationParameter(string name = "IdentificationParameter", double min = 0, double max = 10, double startValue = 5)
      {
         return new IdentificationParameter
         {
            ConstantParameterWithValue(min).WithName(Constants.Parameters.MIN_VALUE),
            ConstantParameterWithValue(startValue).WithName(Constants.Parameters.START_VALUE),
            ConstantParameterWithValue(max).WithName(Constants.Parameters.MAX_VALUE)
         }.WithName(name);
      }

      public static SensitivityParameter SensitivityParameter(string name = "SensitivityParameter", double range = 0.1, double steps = 2)
      {
         return new SensitivityParameter
         {
            ConstantParameterWithValue(range).WithName(Constants.Parameters.VARIATION_RANGE),
            ConstantParameterWithValue(steps).WithName(Constants.Parameters.NUMBER_OF_STEPS),
         }.WithName(name);
      }

      public static DataRepository IndividualSimulationDataRepositoryFor(string simulationName)
      {
         var simulationResults = new DataRepository();
         var baseGrid = new BaseGrid("Time", TimeDimensionForSpecs())
         {
            Values = new[] {1.0f, 2.0f, 3.0f}
         };
         simulationResults.Add(baseGrid);

         var data = new DataColumn("Col", ConcentrationDimensionForSpecs(), baseGrid)
         {
            Values = new[] {10f, 20f, 30f},
            DataInfo = {Origin = ColumnOrigins.Calculation},
            QuantityInfo = new QuantityInfo(new[] {simulationName, "Comp", "Liver", "Cell", "Concentration"}, QuantityType.Drug)
         };

         simulationResults.Add(data);

         return simulationResults;
      }

      private static IndividualResults newIndividualResults(int indIndex, int numberOfPaths, int numberOfPoints)
      {
         var ind = new IndividualResults {IndividualId = indIndex};
         for (int i = 0; i < numberOfPaths; i++)
         {
            ind.Add(createValuesArray(i, numberOfPoints));
         }

         return ind;
      }

      private static QuantityValues createValuesArray(int i, int numberOfPoints)
      {
         return createArray("Path " + i, LengthDimensionForSpecs(), numberOfPoints);
      }

      private static QuantityValues createTimeArray(int numberOfPoints)
      {
         return createArray("Time", TimeDimensionForSpecs(), numberOfPoints);
      }

      private static QuantityValues createArray(string path, IDimension dimension, int numberOfPoints)
      {
         var quantityValues = new QuantityValues
         {
            ColumnId = ShortGuid.NewGuid(),
            QuantityPath = path
         };

         var values = new float[numberOfPoints];
         for (int i = 0; i < values.Length; i++)
         {
            values[i] = (float) (_random.NextDouble() * 100);
         }

         quantityValues.Values = values;
         return quantityValues;
      }

      public static SchemaItemDTO SchemaItemDTO(ApplicationType applicationType, Unit doseDisplayUnit = null, double? doseValue = null, double? startTimeValue = null)
      {
         var schemaItemDTO = new SchemaItemDTO(new SchemaItem {ApplicationType = applicationType})
         {
            DoseParameter = new ParameterDTO(ConstantParameterWithValue(doseValue.GetValueOrDefault(1)).WithName(CoreConstants.Parameters.INPUT_DOSE)),
            StartTimeParameter = new ParameterDTO(ConstantParameterWithValue(startTimeValue.GetValueOrDefault(0)).WithName(Constants.Parameters.START_TIME))
         };

         if (doseDisplayUnit != null)
            schemaItemDTO.DoseParameter.Parameter.DisplayUnit = doseDisplayUnit;

         return schemaItemDTO;
      }

      public static IndividualMolecule DefaultIndividualMolecule()
      {
         return new IndividualEnzyme
         {
            ConstantParameterWithValue(10).WithName(CoreConstants.Parameters.REFERENCE_CONCENTRATION),
            ConstantParameterWithValue(20).WithName(CoreConstants.Parameters.HALF_LIFE_LIVER),
            ConstantParameterWithValue(30).WithName(CoreConstants.Parameters.HALF_LIFE_INTESTINE)
         };
      }
   }

   public class ObjectPathFactoryForSpecs : ObjectPathFactory
   {
      public ObjectPathFactoryForSpecs() : base(new AliasCreator())
      {
      }
   }

   public class EntityPathResolverForSpecs : EntityPathResolver
   {
      public EntityPathResolverForSpecs() : base(new ObjectPathFactoryForSpecs())
      {
      }
   }

   public class PathCacheForSpecs<T> : PathCache<T> where T : class, IEntity
   {
      public PathCacheForSpecs() : base(new EntityPathResolverForSpecs())
      {
      }
   }

   public class ContainerTaskForSpecs : ContainerTask
   {
      public ContainerTaskForSpecs() : base(A.Fake<IObjectBaseFactory>(), new EntityPathResolverForSpecs())
      {
      }
   }
}