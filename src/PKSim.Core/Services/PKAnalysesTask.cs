﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
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

      IEnumerable<PopulationPKAnalysis> CalculateFor(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData, bool firstOnCurves = true);

      /// <summary>
      ///    Calculates the <see cref="IndividualPKAnalysis" /> for the given <paramref name="dataColumns" /> corresponding to
      ///    outputs defined in the <paramref name="simulations" />. If the optional parameter
      ///    <paramref name="globalPKAnalysis" /> is set, some warning may be displayed for single pK-Parameter values
      ///    based on global pK-parameter values such as fraction absorbed
      /// </summary>
      IEnumerable<IndividualPKAnalysis> CalculateFor(IReadOnlyList<Simulation> simulations, IEnumerable<DataColumn> dataColumns, GlobalPKAnalysis globalPKAnalysis = null);

      IndividualPKAnalysis CalculateFor(Simulation simulation, DataColumn dataColumn);
      PKValues CalculatePK(DataColumn column, PKCalculationOptions options);

      /// <summary>
      ///    Resolves options and use the mapper to create a PKAnalysis out of the values and a simulation for a given compound
      /// </summary>
      /// <param name="pkValues">values to use</param>
      /// <param name="simulation">the simulation</param>
      /// <param name="compound">the compound containing its name and molweight</param>
      /// <returns></returns>
      PKAnalysis CreatePKAnalysisFromValues(PKValues pkValues, Simulation simulation, Compound compound);

      IReadOnlyList<PopulationPKAnalysis> AggregatePKAnalysis(Simulation populationDataCollector, IEnumerable<QuantityPKParameter> pkParameters, IEnumerable<StatisticalAggregation> selectedStatistics, string captionPrefix);
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
      private readonly Regex _rangeRegex = new Regex(@"^(.*)Range (\d*)% to (\d*)%");
      private readonly IStatisticalDataCalculator _statisticalDataCalculator;
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public PKAnalysesTask(ILazyLoadTask lazyLoadTask,
         IPKValuesCalculator pkValuesCalculator,
         IPKParameterRepository pkParameterRepository,
         IPKCalculationOptionsFactory pkCalculationOptionsFactory,
         IEntityPathResolver entityPathResolver,
         IPKValuesToPKAnalysisMapper pkMapper,
         IDimensionRepository dimensionRepository,
         IStatisticalDataCalculator statisticalDataCalculator,
         IRepresentationInfoRepository representationInfoRepository) : base(lazyLoadTask, pkValuesCalculator, pkParameterRepository, pkCalculationOptionsFactory)
      {
         _lazyLoadTask = lazyLoadTask;
         _entityPathResolver = entityPathResolver;
         _pkMapper = pkMapper;
         _dimensionRepository = dimensionRepository;
         _pkValuesCalculator = pkValuesCalculator;
         _pkCalculationOptionsFactory = pkCalculationOptionsFactory;
         _pkParameterRepository = pkParameterRepository;
         _statisticalDataCalculator = statisticalDataCalculator;
         _representationInfoRepository = representationInfoRepository;
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
            return base.CalculateFor(populationSimulation, populationSimulation.Results, (individualId) => { updateBodyWeightFromCurrentIndividual(bodyWeightParameter, allBodyWeights, individualId); });
         }
         finally
         {
            bodyWeightParameter?.ResetToDefault();
         }
      }

      public IEnumerable<PopulationPKAnalysis> CalculateFor(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData, bool firstOnCurves = true)
      {
         var pkAnalyses = new List<PopulationPKAnalysis>();

         if (timeProfileChartData == null)
            return pkAnalyses; // there are no analyses to calculate

         var allColumns = timeProfileChartData.Panes.SelectMany(x => x.Curves).SelectMany(x =>
               columnsFor(x, populationDataCollector).Select(column => new { curveData = x, column = column }))
            .Where(c => c.column.IsConcentration());

         var columnsByMolecules = allColumns.GroupBy(x => x.column.MoleculeName());
         foreach (var columnsByMolecule in columnsByMolecules)
         {
            var moleculeName = columnsByMolecule.Key;
            var options = _pkCalculationOptionsFactory.CreateFor(populationDataCollector, moleculeName);
            pkAnalyses.AddRange(columnsByMolecule.Select(pkAnalysisData => new PopulationPKAnalysis(pkAnalysisData.curveData, calculatePKFor(pkAnalysisData.column, moleculeName, options).PKAnalysis, pkAnalysisData.column.Name)));
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
         allPKAnalysis.AddRange(allColumns.Where(x => x.IsObservation())
            .Select(observedDataColumn =>
            {
               var moleculeName = observedDataColumn.Repository.ExtendedPropertyValueFor(Constants.ObservedData.MOLECULE);
               var observedDataPKOptions = _pkCalculationOptionsFactory.CreateForObservedData(simulations, moleculeName);
               return calculatePKFor(observedDataColumn, moleculeName, observedDataPKOptions);
            }));

         return allPKAnalysis;
      }

      private IEnumerable<DataColumn> columnsFor(CurveData<TimeProfileXValue, TimeProfileYValue> curveData, IPopulationDataCollector populationDataCollector)
      {
         var baseGrid = new BaseGrid(Constants.TIME, curveData.XAxis.Dimension) { Values = curveData.XValues.Select(x => x.X).ToList() };

         if (curveData.IsRange())
         {
            var (lowerRange, upperRange) = rangeDescriptions(curveData.Caption);
            return new[]
            {
               new DataColumn(lowerRange, curveData.YAxis.Dimension, baseGrid)
               {
                  Values = curveData.YValues.Select(y => y.LowerValue).ToList(),
                  DataInfo = { MolWeight = populationDataCollector.MolWeightFor(curveData.QuantityPath) },
                  QuantityInfo = { Path = curveData.QuantityPath.ToPathArray() }
               },
               new DataColumn(upperRange, curveData.YAxis.Dimension, baseGrid)
               {
                  Values = curveData.YValues.Select(y => y.UpperValue).ToList(),
                  DataInfo = { MolWeight = populationDataCollector.MolWeightFor(curveData.QuantityPath) },
                  QuantityInfo = { Path = curveData.QuantityPath.ToPathArray() }
               }
            };
         }

         return new[]
         {
            new DataColumn(curveData.Caption, curveData.YAxis.Dimension, baseGrid)
            {
               Values = curveData.YValues.Select(y => y.Y).ToList(),
               DataInfo = {MolWeight = populationDataCollector.MolWeightFor(curveData.QuantityPath)},
               QuantityInfo = {Path = curveData.QuantityPath.ToPathArray()}
            }
         };
      }

      public IndividualPKAnalysis CalculateFor(Simulation simulation, DataColumn dataColumn)
      {
         if (dataColumn == null)
            return new NullIndividualPKAnalysis();

         return CalculateFor(new[] { simulation }, new[] { dataColumn }).FirstOrDefault() ?? new NullIndividualPKAnalysis();
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

      private IndividualPKAnalysis calculatePKFor(DataColumn dataColumn, string moleculeName, PKCalculationOptions options, GlobalPKAnalysis globalPKAnalysis = null)
      {
         var timeValue = dataColumn.BaseGrid.Values;
         var dimension = _dimensionRepository.MergedDimensionFor(dataColumn);
         var umolPerLiterUnit = dimension.UnitOrDefault(CoreConstants.Units.MicroMolPerLiter);
         var concentrationValueInMolL = dataColumn.Values.Select(v => dimension.BaseUnitValueToUnitValue(umolPerLiterUnit, v)).ToArray();
         var pkAnalysis = _pkMapper.MapFrom(dataColumn, _pkValuesCalculator.CalculatePK(timeValue, concentrationValueInMolL, options), options.PKParameterMode, moleculeName);
         addWarningsTo(pkAnalysis, globalPKAnalysis, moleculeName);
         return new IndividualPKAnalysis(dataColumn, pkAnalysis);
      }

      private void addWarningsTo(PKAnalysis pkAnalysis, GlobalPKAnalysis globalPKAnalysis, string moleculeName)
      {
         if (globalPKAnalysis == null)
            return;

         addFractionAbsorbedWarningTo(pkAnalysis, globalPKAnalysis, moleculeName);
      }

      private void addFractionAbsorbedWarningTo(PKAnalysis pkAnalysis, GlobalPKAnalysis globalPKAnalysis, string moleculeName)
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

      private IBusinessRule warningRule(string warning) =>
         CreateRule.For<IParameter>()
            .Property(item => item.Value)
            .WithRule((param, value) => false)
            .WithError((param, value) => warning);

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

      public PKAnalysis CreatePKAnalysisFromValues(PKValues pkValues, Simulation simulation, Compound compound)
      {
         var options = _pkCalculationOptionsFactory.CreateFor(simulation, compound.Name);
         return _pkMapper.MapFrom(compound.MolWeight, pkValues, options.PKParameterMode, compound.Name);
      }

      /// <summary>
      /// Returns the range strings when the <paramref name="text"/> contains 'Range 2.5% to 97.5%' language
      /// </summary>
      /// <param name="text">The text being split</param>
      /// <returns>The individual range descriptions as a tuple containing low range and high range.
      /// If the string cannot be split on 'Range', returns the original text in both members of the tuple</returns>
      private (string lowerRange, string upperRange) rangeDescriptions(string text)
      {
         var splitStrings = text.Split(new[] { "Range" }, StringSplitOptions.RemoveEmptyEntries);
         var match = splitStrings.Length == 2;

         if (!match)
            return ( text, text );

         var upperAndLowerRange = splitStrings.Last().Split(new[] { "to" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

         return ($"{splitStrings[0]}{upperAndLowerRange[0]}", $"{splitStrings[0]}{upperAndLowerRange[1]}" );
      }

      public IReadOnlyList<PopulationPKAnalysis> AggregatePKAnalysis(Simulation simulation, IEnumerable<QuantityPKParameter> pkParameters, IEnumerable<StatisticalAggregation> selectedStatistics, string captionPrefix)
      {
         var pkParametersList = pkParameters.ToList();
         var matrix = new FloatMatrix();
         var names = pkParametersList.Select(x => x.Name).Distinct().ToList();
         pkParametersList.Each(pkParameter => matrix.AddValuesAndSort(pkParameter.ValuesAsArray));

         var results = new List<PopulationPKAnalysis>();
         selectedStatistics.Each(statisticalAnalysis =>
         {
            var aggregated = _statisticalDataCalculator.StatisticalDataFor(matrix, statisticalAnalysis).ToList();
            aggregated.Each((agg, index) =>
            {
               var name = correctNameFromMetric(_representationInfoRepository.DisplayNameFor(statisticalAnalysis), aggregated.Count > 1, index == 0, captionPrefix);
               var pkAnalysis = buildPopulationPKAnalysis(buildCurveData(pkParametersList[index], name), agg, names, simulation);
               results.Add(pkAnalysis);
            });
         });
         return results;
      }

      private string correctNameFromMetric(string originalText, bool multipleValues, bool isLowerValue, string captionPrefix)
      {
         var suffix = originalText;
         //For those metrics returning two values, the first is the lower value and the second
         //is the upper value so depending on the index we use lower or upper suffix.
         if (multipleValues)
         {
            var (lowerRange, upperRange) = rangeDescriptions(suffix);
            suffix = isLowerValue ? lowerRange : upperRange;
         }

         return (new[] { captionPrefix, suffix }).ToCaption();
      }

      private CurveData<TimeProfileXValue, TimeProfileYValue> buildCurveData(QuantityPKParameter quantityPKParameter, string caption)
      {
         return new CurveData<TimeProfileXValue, TimeProfileYValue>()
         {
            Id = quantityPKParameter.Id,
            Caption = caption,
            YDimension = quantityPKParameter.Dimension,
            QuantityPath = quantityPKParameter.QuantityPath,
         };
      }

      private PopulationPKAnalysis buildPopulationPKAnalysis(CurveData<TimeProfileXValue, TimeProfileYValue> curveData, float[] values, IReadOnlyList<string> names, Simulation simulation)
      {
         var pkValues = new PKValues();
         for (var i = 0; i < names.Count; i++)
         {
            pkValues.AddValue(names[i], values[i]);
         }

         var compound = simulation.Compounds.First(x => simulation.Model.MoleculeNameFor(curveData.QuantityPath) == x.Name);
         return new PopulationPKAnalysis(curveData, CreatePKAnalysisFromValues(pkValues, simulation, compound));
      }
   }
}