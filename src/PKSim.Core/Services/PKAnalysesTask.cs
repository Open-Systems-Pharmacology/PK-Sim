using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IPKAnalysesTask : OSPSuite.Core.Domain.Services.IPKAnalysesTask
   {
      /// <summary>
      ///    Calculates the PKAnalyses for the given <paramref name="populationSimulation" />. It does not delete the previous pk
      ///    calculation from the <paramref name="populationSimulation" />
      /// </summary>
      /// <param name="populationSimulation">Population simulation for which pk parameters should be calculated</param>
      /// <returns>The PopulationSimulationPKAnalyses containing all calculated values</returns>
      PopulationSimulationPKAnalyses CalculateFor(PopulationSimulation populationSimulation);

      IEnumerable<PopulationPKAnalysis> CalculateFor(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData);

      /// <summary>
      ///    Calculates the <see cref="IndividualPKAnalysis" /> for the given <paramref name="dataColumns" /> corresponding to
      ///    outputs defined in the <paramref name="simulations" />. If the optional parameter
      ///    <paramref name="globalPKAnalysis" /> is set, some warning may be displayed for single pK-Parameter values
      ///    based on global pK-parameter values such as fraction absorbed
      /// </summary>
      IEnumerable<IndividualPKAnalysis> CalculateFor(IReadOnlyList<Simulation> simulations, IEnumerable<DataColumn> dataColumns, GlobalPKAnalysis globalPKAnalysis = null);

      IndividualPKAnalysis CalculateFor(Simulation simulation, DataColumn dataColumn);
      PKValues CalculatePK(DataColumn column, PKCalculationOptions options);
   }

   public class PKAnalysesTask : OSPSuite.Core.Domain.Services.PKAnalysesTask, IPKAnalysesTask
   {
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IPKValuesCalculator _pkValuesCalculator;
      private readonly IPKValuesToPKAnalysisMapper _pkMapper;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IPKCalculationOptionsFactory _pkCalculationOptionsFactory;
      private readonly IPKParameterRepository _pkParameterRepository;

      public PKAnalysesTask(ILazyLoadTask lazyLoadTask,
         IPKValuesCalculator pkValuesCalculator,
         IPKParameterRepository pkParameterRepository,
         IPKCalculationOptionsFactory pkCalculationOptionsFactory,
         IEntityPathResolver entityPathResolver,
         IPKValuesToPKAnalysisMapper pkMapper,
         IDimensionRepository dimensionRepository) : base(lazyLoadTask, pkValuesCalculator, pkParameterRepository, pkCalculationOptionsFactory)
      {
         _lazyLoadTask = lazyLoadTask;
         _entityPathResolver = entityPathResolver;
         _pkMapper = pkMapper;
         _dimensionRepository = dimensionRepository;
         _pkValuesCalculator = pkValuesCalculator;
         _pkCalculationOptionsFactory = pkCalculationOptionsFactory;
         _pkParameterRepository = pkParameterRepository;
      }

      public PopulationSimulationPKAnalyses CalculateFor(PopulationSimulation populationSimulation)
      {
         _lazyLoadTask.LoadResults(populationSimulation);
         if (!populationSimulation.HasResults)
            return new NullPopulationSimulationPKAnalyses();

         var bodyWeightParameter = populationSimulation.BodyWeight;
         var bodyWeightParameterPath = bodyWeightParameterPathFrom(bodyWeightParameter);
         var allBodyWeights = populationSimulation.AllValuesFor(bodyWeightParameterPath);

         try
         {
            return base.CalculateFor(populationSimulation, populationSimulation.NumberOfItems, populationSimulation.Results, (individualId) => { updateBodyWeightFromCurrentIndividual(bodyWeightParameter, allBodyWeights, individualId); });
         }
         finally
         {
            bodyWeightParameter?.ResetToDefault();
         }
      }

      public IEnumerable<PopulationPKAnalysis> CalculateFor(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData)
      {
         var pkAnalyses = new List<PopulationPKAnalysis>();

         if (timeProfileChartData == null)
            return pkAnalyses; // there are no analyses to calculate

         var allColumns = timeProfileChartData.Panes.SelectMany(x => x.Curves).Select(x =>
               new {curveData = x, column = columnFor(x, populationDataCollector)})
            .Where(c => c.column != null)
            .Where(c => c.column.IsConcentration());

         var columnsByMolecules = allColumns.GroupBy(x => x.column.MoleculeName());
         foreach (var columnsByMolecule in columnsByMolecules)
         {
            var moleculeName = columnsByMolecule.Key;
            var options = _pkCalculationOptionsFactory.CreateFor(populationDataCollector, moleculeName);
            pkAnalyses.AddRange(columnsByMolecule.Select(pkAnalysisData => new PopulationPKAnalysis(pkAnalysisData.curveData, calculatePKFor(pkAnalysisData.column, moleculeName, options).PKAnalysis)));
         }

         return pkAnalyses;
      }

      public IEnumerable<IndividualPKAnalysis> CalculateFor(IReadOnlyList<Simulation> simulations, IEnumerable<DataColumn> dataColumns, GlobalPKAnalysis globalPKAnalysis = null)
      {
         var allPKAnalysis = new List<IndividualPKAnalysis>();
         var allColumns = dataColumns.ToList();
         foreach (var simulation in simulations)
         {
            var columnsByMolecules = allColumns.BelongingTo(simulation).GroupBy(x => x.MoleculeName());
            foreach (var columnsByMolecule in columnsByMolecules)
            {
               var moleculeName = columnsByMolecule.Key;
               var options = _pkCalculationOptionsFactory.CreateFor(simulation, moleculeName);
               allPKAnalysis.AddRange(columnsByMolecule.Select(c => calculatePKFor(c, moleculeName, options, globalPKAnalysis)));
            }
         }

         //last but not least, add observed data that do not belong to any simulation
         allPKAnalysis.AddRange(allColumns.Where(x => x.IsObservedData())
            .Select(obervedDataColumn =>
            {
               var moleculeName = obervedDataColumn.Repository.ExtendedPropertyValueFor(ObservedData.MOLECULE);
               var observedDataPKOptions = _pkCalculationOptionsFactory.CreateForObservedData(simulations, moleculeName);
               return calculatePKFor(obervedDataColumn, moleculeName, observedDataPKOptions);
            }));

         return allPKAnalysis;
      }

      private DataColumn columnFor(CurveData<TimeProfileXValue, TimeProfileYValue> curveData, IPopulationDataCollector populationDataCollector)
      {
         if (curveData.IsRange())
            return null;

         var baseGrid = new BaseGrid("Time", curveData.XAxis.Dimension) {Values = curveData.XValues.Select(x => x.X).ToList()};
         return new DataColumn("Col", curveData.YAxis.Dimension, baseGrid)
         {
            Values = curveData.YValues.Select(y => y.Y).ToList(),
            DataInfo = {MolWeight = populationDataCollector.MolWeightFor(curveData.QuantityPath)},
            QuantityInfo = {Path = curveData.QuantityPath.ToPathArray()}
         };
      }

      public IndividualPKAnalysis CalculateFor(Simulation simulation, DataColumn dataColumn)
      {
         if (dataColumn == null)
            return new NullIndividualPKAnalysis();

         return CalculateFor(new[] {simulation}, new[] {dataColumn}).FirstOrDefault() ?? new NullIndividualPKAnalysis();
      }

      public PKValues CalculatePK(DataColumn column, PKCalculationOptions options)
      {
         return _pkValuesCalculator.CalculatePK(column, options);
      }

      public string DescriptionFor(string pkParameterName)
      {
         var pkParameter = _pkParameterRepository.FindByName(pkParameterName);
         if (pkParameter == null)
            return pkParameterName;

         return pkParameter.Description;
      }

      private IndividualPKAnalysis calculatePKFor(DataColumn dataColumn, string moleculeName, PKCalculationOptions options, GlobalPKAnalysis globalPKAnalysys = null)
      {
         var timeValue = dataColumn.BaseGrid.Values;
         var dimension = _dimensionRepository.MergedDimensionFor(dataColumn);
         var umolPerLiterUnit = dimension.UnitOrDefault(CoreConstants.Units.MicroMolPerLiter);
         var concentrationValueInMolL = dataColumn.Values.Select(v => dimension.BaseUnitValueToUnitValue(umolPerLiterUnit, v)).ToFloatArray();
         var pkAnalysis = _pkMapper.MapFrom(dataColumn, _pkValuesCalculator.CalculatePK(timeValue, concentrationValueInMolL, options), options.PKParameterMode, moleculeName);
         addWarningsTo(pkAnalysis, globalPKAnalysys, moleculeName);
         return new IndividualPKAnalysis(dataColumn, pkAnalysis);
      }

      private void addWarningsTo(PKAnalysis pkAnalysis, GlobalPKAnalysis globalPKAnalysis, string moleculeName)
      {
         if (globalPKAnalysis == null)
            return;

         addFractionAbsorvedWarningTo(pkAnalysis, globalPKAnalysis, moleculeName);
      }

      private void addFractionAbsorvedWarningTo(PKAnalysis pkAnalysis, GlobalPKAnalysis globalPKAnalysis, string moleculeName)
      {
         var fractionAbsorbed = globalPKAnalysis.PKParameter(moleculeName, CoreConstants.PKAnalysis.FractionAbsorbed);
         if (fractionAbsorbed == null)
            return;

         if (ValueComparer.AreValuesEqual(fractionAbsorbed.Value, 1, CoreConstants.DOUBLE_RELATIVE_EPSILON))
            return;

         addWarningsTo(pkAnalysis, PKSimConstants.Warning.FractionAbsorbedSmallerThanOne, CoreConstants.PKAnalysis.AllParametersInfluencedByFractionAbsorbed);
      }

      private void addWarningsTo(PKAnalysis pkAnalysis, string warning, IEnumerable<string> parameterNames)
      {
         parameterNames.Select(pkAnalysis.Parameter)
            .Where(p => p != null)
            .Each(p => p.Rules.Add(warningRule(warning)));
      }

      private IBusinessRule warningRule(string warning)
      {
         return CreateRule.For<IParameter>()
            .Property(item => item.Value)
            .WithRule((param, value) => false)
            .WithError((param, value) => warning);
      }

      private string bodyWeightParameterPathFrom(IParameter bodyWeightParameter)
      {
         return bodyWeightParameter != null ? _entityPathResolver.PathFor(bodyWeightParameter) : string.Empty;
      }

      private void updateBodyWeightFromCurrentIndividual(IParameter bodyWeightParameter, IReadOnlyList<double> allBodyWeights, int individualId)
      {
         if (bodyWeightParameter == null)
            return;

         bodyWeightParameter.Value = allBodyWeights.Count > individualId ? allBodyWeights[individualId] : double.NaN;
      }
   }
}