using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Maths;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core.Chart;
using PKSim.Core.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using static OSPSuite.Core.Domain.Constants;
using static OSPSuite.Core.Domain.Constants.Dimension;
using static OSPSuite.Core.Domain.Constants.ObservedData;
using static OSPSuite.Core.Domain.Constants.Parameters;
using static OSPSuite.Core.Domain.Constants.PKParameters;
using static PKSim.Assets.PKSimConstants.Warning;
using static PKSim.Core.CoreConstants.Dimension;
using static PKSim.Core.CoreConstants.PKAnalysis;
using static PKSim.Core.CoreConstants.Species;
using static PKSim.Core.CoreConstants.Units;
using Compound = PKSim.Core.Model.Compound;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;
using PKAnalysis = PKSim.Core.Model.PKAnalysis;

namespace PKSim.Core.Services
{
   public interface IPKAnalysesTask : OSPSuite.Core.Domain.Services.IPKAnalysesTask
   {
      /// <summary>
      ///    Calculates the PKAnalyses for the given <paramref name="populationSimulation" />. It does not delete the previous pk
      ///    calculation from the <paramref name="populationSimulation" />
      /// </summary>
      /// <param name="populationSimulation">Population simulation for which pk parameters should be calculated</param>
      /// <param name="compoundPKContext">If there is context such as prior calculations of ratio parameters, they are held here</param>
      /// <returns>The PopulationSimulationPKAnalyses containing all calculated values</returns>
      PopulationSimulationPKAnalyses CalculateFor(PopulationSimulation populationSimulation, CompoundPKContext compoundPKContext = null);

      IEnumerable<PopulationPKAnalysis> CalculateFor(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData, bool firstOnCurves = true);

      /// <summary>
      ///    Calculates the <see cref="IndividualPKAnalysis" /> for the given <paramref name="dataColumns" /> corresponding to
      ///    outputs defined in the <paramref name="simulations" />. If the optional parameter
      ///    <paramref name="globalPKAnalysis" /> is set, some warning may be displayed for single pK-Parameter values
      ///    based on global pK-parameter values such as fraction absorbed
      /// </summary>
      IEnumerable<IndividualPKAnalysis> CalculateFor(IReadOnlyList<Simulation> simulations, IEnumerable<DataColumn> dataColumns, GlobalPKAnalysis globalPKAnalysis = null);

      IndividualPKAnalysis CalculateFor(Simulation simulation, DataColumn dataColumn);

      /// <summary>
      ///    Resolves options and use the mapper to create a PKAnalysis out of the values and a simulation for a given compound
      /// </summary>
      /// <param name="pkValues">values to use</param>
      /// <param name="simulation">the simulation</param>
      /// <param name="compound">the compound containing its name and molweight</param>
      /// <returns></returns>
      PKAnalysis CreatePKAnalysisFromValues(PKValues pkValues, Simulation simulation, Compound compound);

      IReadOnlyList<PopulationPKAnalysis> AggregatePKAnalysis(Simulation populationDataCollector, IEnumerable<QuantityPKParameter> pkParameters, IEnumerable<StatisticalAggregation> selectedStatistics, string captionPrefix);

      GlobalPKAnalysis CalculateGlobalPKAnalysisFor(IEnumerable<Simulation> simulations);

      /// <summary>
      ///    Starts the calculation of the bioavailability for the compound named <paramref name="compoundName" /> by switching
      ///    its application
      ///    protocol with an iv protocol
      /// </summary>
      void CalculateBioavailabilityFor(Simulation simulation, string compoundName);

      /// <summary>
      ///    Starts the calculation of the Auc DDI Ratio for all compounds /> by switching off all other applications
      /// </summary>
      void CalculateDDIRatioFor(Simulation simulation);
   }

   public class PKAnalysesTask : OSPSuite.Core.Domain.Services.PKAnalysesTask, IPKAnalysesTask
   {
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IPKValuesCalculator _pkValuesCalculator;
      private readonly IPKValuesToPKAnalysisMapper _pkMapper;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IPKCalculationOptionsFactory _pkCalculationOptionsFactory;
      private readonly IPKParameterRepository _pkParameterRepository;
      private readonly IStatisticalDataCalculator _statisticalDataCalculator;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IParameterFactory _parameterFactory;
      private readonly IProtocolToSchemaItemsMapper _protocolToSchemaItemsMapper;
      private readonly IProtocolFactory _protocolFactory;
      private readonly IGlobalPKAnalysisRunner _globalPKAnalysisRunner;
      private readonly IVSSCalculator _vssCalculator;
      private readonly IInteractionTask _interactionTask;
      private readonly ICloner _cloner;
      private readonly IEntityPathResolver _entityPathResolver;

      public PKAnalysesTask(ILazyLoadTask lazyLoadTask,
         IPKValuesCalculator pkValuesCalculator,
         IPKParameterRepository pkParameterRepository,
         IPKCalculationOptionsFactory pkCalculationOptionsFactory,
         IPKValuesToPKAnalysisMapper pkMapper,
         IDimensionRepository dimensionRepository,
         IStatisticalDataCalculator statisticalDataCalculator,
         IRepresentationInfoRepository representationInfoRepository,
         IParameterFactory parameterFactory, IProtocolToSchemaItemsMapper protocolToSchemaItemsMapper, IProtocolFactory protocolFactory,
         IGlobalPKAnalysisRunner globalPKAnalysisRunner, IVSSCalculator vssCalculator, IInteractionTask interactionTask, ICloner cloner, IEntityPathResolver entityPathResolver) : 
         base(lazyLoadTask, pkValuesCalculator, pkParameterRepository, pkCalculationOptionsFactory)
      {
         _lazyLoadTask = lazyLoadTask;
         _pkMapper = pkMapper;
         _dimensionRepository = dimensionRepository;
         _pkValuesCalculator = pkValuesCalculator;
         _pkCalculationOptionsFactory = pkCalculationOptionsFactory;
         _pkParameterRepository = pkParameterRepository;
         _statisticalDataCalculator = statisticalDataCalculator;
         _representationInfoRepository = representationInfoRepository;
         _parameterFactory = parameterFactory;
         _protocolToSchemaItemsMapper = protocolToSchemaItemsMapper;
         _protocolFactory = protocolFactory;
         _globalPKAnalysisRunner = globalPKAnalysisRunner;
         _vssCalculator = vssCalculator;
         _interactionTask = interactionTask;
         _cloner = cloner;
         _entityPathResolver = entityPathResolver;
      }

      public PopulationSimulationPKAnalyses CalculateFor(PopulationSimulation populationSimulation, CompoundPKContext compoundPKContext = null)
      {
         _lazyLoadTask.LoadResults(populationSimulation);
         if (!populationSimulation.HasResults)
            return new NullPopulationSimulationPKAnalyses();

         var bodyWeightParameter = populationSimulation.BodyWeight;
         var bodyWeightParameterPath = bodyWeightParameterPathFrom(bodyWeightParameter);
         var allBodyWeights = populationSimulation.AllValuesFor(bodyWeightParameterPath);

         try
         {
            var globalPKAnalysesForIndividuals = new Cache<int, GlobalPKAnalysis>();
            var analyses = base.CalculateFor(populationSimulation, populationSimulation.Results,
               individualId =>
               {
                  updateBodyWeightForIndividual(bodyWeightParameter, allBodyWeights, individualId);
               },
               (individualId, options, compoundName) =>
               {
                  var globalPKAnalysis = globalPkAnalysisForIndividual(globalPKAnalysesForIndividuals, individualId);
                  globalPKAnalysis.Add(calculateGlobalPKParameters(populationSimulation, individualId, compoundName, options, compoundPKContext));
               });

            mapQuantityPKParametersFromIndividualGlobalPKAnalyses(globalPKAnalysesForIndividuals).Each(analyses.AddPKAnalysis);
            return analyses;
         }
         finally
         {
            bodyWeightParameter?.ResetToDefault();
         }
      }

      private string bodyWeightParameterPathFrom(IParameter bodyWeightParameter)
      {
         return bodyWeightParameter != null ? _entityPathResolver.PathFor(bodyWeightParameter) : string.Empty;
      }

      private void updateBodyWeightForIndividual(IParameter bodyWeightParameter, IReadOnlyList<double> allBodyWeights, int individualId)
      {
         if (bodyWeightParameter == null)
            return;

         bodyWeightParameter.Value = allBodyWeights.Count > individualId ? allBodyWeights[individualId] : double.NaN;
      }

      public GlobalPKAnalysis CalculateGlobalPKAnalysisFor(IEnumerable<Simulation> simulations)
      {
         var globalPKAnalysis = new GlobalPKAnalysis();

         var allSimulations = simulations.ToList();
         if (allSimulations.Count != 1)
            return globalPKAnalysis;

         var simulation = allSimulations[0];

         if (simulation == null)
            return globalPKAnalysis;

         // For a population simulation the pk parameters are calculated at run time so we retrieve the global
         // pk parameters from the simulation
         if (simulation is PopulationSimulation populationSimulation)
         {
            // Global PK Parameters are stored in containers with the compound name as the first child of the root container.
            // There will be one container per compound with PK parameters stored within
            var compoundContainers = createCompoundPKParameterContainers(populationSimulation);

            globalPKAnalysis.AddChildren(compoundContainers);

            return globalPKAnalysis;
         }

         // For individual simulations global pk parameters are calculated on the fly
         if (simulation is IndividualSimulation individualSimulation)
         {
            individualSimulation.Compounds.Each(compound =>
            {
               var pkCalculationOptions = _pkCalculationOptionsFactory.CreateFor(individualSimulation, compound.Name);
               var peripheralVenousBloodColumn = individualSimulation.PeripheralVenousBloodColumn(compound.Name);
               var venousBloodColumn = individualSimulation.VenousBloodColumn(compound.Name);
               var fractionAbsorbedColumn = individualSimulation.FabsOral(compound.Name);
               var individualId = individualSimulation.IndividualId;
               var compoundPKContext = createCompoundPKContext(compound, individualSimulation, individualId);
               var compoundContainer = calculateGlobalPKAnalysisFor(individualSimulation, compound, peripheralVenousBloodColumn, venousBloodColumn, fractionAbsorbedColumn, pkCalculationOptions, individualId, compoundPKContext);
               globalPKAnalysis.Add(compoundContainer);
            });
         }

         return globalPKAnalysis;
      }

      private static CompoundPKContext createCompoundPKContext(Compound compound, IndividualSimulation individualSimulation, int individualId)
      {
         var compoundPKContext = new CompoundPKContext();
         var compoundPK = compoundPKContext.CompoundPKFor(compound.Name);
         if (individualSimulation.AucIV.Contains(compound.Name))
            compoundPK.AddBioavailability(individualId, individualSimulation.AucIV[compound.Name]);

         if (individualSimulation.AucDDI.Contains(compound.Name) && individualSimulation.CMaxDDI.Contains(compound.Name))
         {
            compoundPK.AddDDIAucInf(individualId, individualSimulation.AucDDI[compound.Name]);
            compoundPK.AddDDICMax(individualId, individualSimulation.CMaxDDI[compound.Name]);
         }
         return compoundPKContext;
      }

      private IEnumerable<Container> createCompoundPKParameterContainers(PopulationSimulation populationSimulation)
      {
         return populationSimulation.CompoundNames.Select(compoundName => createCompoundContainer(populationSimulation, compoundName));
      }

      private Container createCompoundContainer(PopulationSimulation populationSimulation, string compoundName)
      {
         var compoundContainer = new Container().WithName(compoundName);
         populationSimulation.PKAnalyses.AllPKParametersFor(compoundName).Each(quantityPKParameter =>
         {
            double? defaultValue = new SortedFloatArray(quantityPKParameter.ValuesAsArray, false).Median();
            if (double.IsNaN(defaultValue.Value))
               defaultValue = null;

            var newParameter = createParameter(quantityPKParameter.Name, defaultValue, quantityPKParameter.DimensionName());
            compoundContainer.Add(newParameter);
         });

         return compoundContainer;
      }

      private IContainer calculateGlobalPKParameters(PopulationSimulation simulation, int individualId, string moleculeName, PKCalculationOptions calculationOptions, CompoundPKContext compoundPKContext)
      {
         var peripheralVenousBloodColumn = simulation.PeripheralVenousBloodColumnForIndividual(individualId, moleculeName);
         var venousBloodColumn = simulation.VenousBloodColumnForIndividual(individualId, moleculeName);
         var fractionAbsorbedColumn = simulation.FractionAbsorbedColumnForIndividual(individualId, moleculeName);

         return calculateGlobalPKAnalysisFor(simulation, simulation.Compounds.FindByName(moleculeName), peripheralVenousBloodColumn, venousBloodColumn, fractionAbsorbedColumn, calculationOptions, individualId, compoundPKContext);
      }

      private static GlobalPKAnalysis globalPkAnalysisForIndividual(ICache<int, GlobalPKAnalysis> globalPKAnalysesForIndividuals, int individualId)
      {
         if (!globalPKAnalysesForIndividuals.Keys.Contains(individualId))
         {
            globalPKAnalysesForIndividuals[individualId] = new GlobalPKAnalysis();
         }

         return globalPKAnalysesForIndividuals[individualId];
      }

      private IContainer calculateGlobalPKAnalysisFor(Simulation simulation, Compound compound, DataColumn peripheralVenousBloodCurve, DataColumn venousBloodCurve, DataColumn fractionAbsorbedCurve, PKCalculationOptions options, int individualId, CompoundPKContext compoundPKContext = null)
      {
         //one container per compound

         var compoundName = compound.Name;
         IParameter vssPhysChem = null;
         var container = new Container().WithName(compoundName);

         if (peripheralVenousBloodCurve == null || venousBloodCurve == null)
            return container;

         var venousBloodPlasmaPK = calculatePK(venousBloodCurve, options);

         var compoundPKFromContext = (compoundPKContext ?? new CompoundPKContext()).CompoundPKFor(compoundName);

         var bioAvailability = contextParameterFrom(Bioavailability, compoundPKFromContext.QuantityFor(individualId, Bioavailability), venousBloodPlasmaPK.ValueFor(AUC_inf),
            compoundPKFromContext.BioAvailabilityAucInfFor(individualId));

         compoundPKFromContext.AddBioavailability(individualId, venousBloodPlasmaPK.ValueFor(AUC_inf));

         var bioAvailabilityValue = bioAvailability.Value;

         var bloodCurveForPKAnalysis = bloodCurveForSpecies(peripheralVenousBloodCurve, venousBloodCurve, simulation.Individual);
         var bloodPlasmaPK = calculatePK(bloodCurveForPKAnalysis, options);

         var applicationType = applicationTypeFor(simulation, compound);

         var aucRatio = contextParameterFrom(AUCRatio, compoundPKFromContext.QuantityFor(individualId, AUCRatio),
            bloodPlasmaPK[pkParameterNameForAUCRatio(applicationType)], compoundPKFromContext.DDIAucInfFor(individualId));

         compoundPKFromContext.AddDDIAucInf(individualId, bloodPlasmaPK[pkParameterNameForAUCRatio(applicationType)]);

         var cmaxRatio = contextParameterFrom(C_maxRatio, compoundPKFromContext.QuantityFor(individualId, C_maxRatio),
            bloodPlasmaPK[pkParameterNameForCmaxRatio(applicationType)], compoundPKFromContext.CMaxDDIFor(individualId));

         compoundPKFromContext.AddDDICMax(individualId, bloodPlasmaPK[pkParameterNameForCmaxRatio(applicationType)]);


         var vssPlasma = createParameter(VssPlasma, bloodPlasmaPK[Vss], VolumePerBodyWeight);
         var vssPlasmaOverF = createParameter(VssPlasmaOverF, bloodPlasmaPK[Vss], VolumePerBodyWeight);
         var vdPlasma = createParameter(VdPlasma, bloodPlasmaPK[Vd], VolumePerBodyWeight);
         var vdPlasmaOverF = createParameter(VdPlasmaOverF, bloodPlasmaPK[Vd], VolumePerBodyWeight);
         if(simulation is IndividualSimulation)
            vssPhysChem = createParameter(VssPhysChem, calculateVSSPhysChemFor(simulation, compoundName), VolumePerBodyWeight);

         var totalPlasmaCL = createParameter(TotalPlasmaCL, bloodPlasmaPK[PKParameters.CL], FlowPerWeight);
         var totalPlasmaCLOverF = createParameter(TotalPlasmaCLOverF, bloodPlasmaPK[PKParameters.CL], FlowPerWeight);

         if (_interactionTask.HasInteractionInvolving(compound, simulation))
            container.AddChildren(aucRatio, cmaxRatio);

         var fractionAbsorbed = fractionAbsorbedFor(fractionAbsorbedCurve);

         //multiple application or no application? in that case, only show fraction absorbed for oral
         if (applicationType != ApplicationType.Single)
         {
            if (isMultipleOral(simulation, compound))
               container.Add(fractionAbsorbed);

            return container;
         }

         var pkValues = new List<IParameter>();
         var bioAvailabilityCalculated = !double.IsNaN(bioAvailabilityValue);

         var schemaItem = singleDosingItem(simulation, compound);
         if (isIntravenous(schemaItem))
         {
            container.AddChildren(vssPlasma, vdPlasma, totalPlasmaCL);
            if(vssPhysChem != null)
               container.Add(vssPhysChem);

            return container;
         }

         var fractionAbsorbedWarningParameters = new List<IParameter>();
         if (bioAvailabilityCalculated)
         {
            vssPlasma.Value *= bioAvailabilityValue;
            vdPlasma.Value *= bioAvailabilityValue;
            totalPlasmaCL.Value *= bioAvailabilityValue;
            fractionAbsorbedWarningParameters.AddRange(new[] { vssPlasma, vdPlasma });
            pkValues.AddRange(new[] { vssPlasma, vdPlasma, totalPlasmaCL, bioAvailability });
         }
         else
         {
            fractionAbsorbedWarningParameters.AddRange(new[] { vssPlasmaOverF, vdPlasmaOverF });
            pkValues.AddRange(new[] { vssPlasmaOverF, vdPlasmaOverF, totalPlasmaCLOverF, bioAvailability });
         }


         if (isOral(schemaItem))
         {
            fractionAbsorbed.Rules.Add(fractionAbsorbedRule);
            pkValues.Add(fractionAbsorbed);

            if (bioAvailability.Value > fractionAbsorbed.Value)
               bioAvailability.Rules.Add(bioAvailabilityRule);

            addFractionAbsorbedWarningTo(fractionAbsorbed, fractionAbsorbedWarningParameters);
         }

         container.AddChildren(pkValues);
         return container;
      }

      IParameter contextParameterFrom(string parameterName, double? contextValue, double? numerator, double? denominator)
      {
         if (contextValue.HasValue)
            return createParameter(parameterName, contextValue, DIMENSIONLESS);

         return createRatioParameter(parameterName, numerator, denominator, DIMENSIONLESS);
      }

      private static IEnumerable<QuantityPKParameter> mapQuantityPKParametersFromIndividualGlobalPKAnalyses(ICache<int, GlobalPKAnalysis> globalIndividualPKParameterCache)
      {
         var quantityPKList = new List<QuantityPKParameter>();

         // use the first in series as a template to retrieve from all individual results.
         // The list of parameters should be identical for all the individual global analyses.
         var aPKAnalysis = globalIndividualPKParameterCache.FirstOrDefault();
         aPKAnalysis?.AllPKParameters.GroupBy(moleculeNameFrom).Each(group =>
         {
            group.Each(pKParameter =>
            {
               quantityPKList.Add(quantityPKParameterFor(globalIndividualPKParameterCache, pKParameter, group.Key));
            });
         });

         return quantityPKList;
      }

      private static string moleculeNameFrom(IParameter pkParameter)
      {
         return pkParameter.EntityPath().ToPathArray().MoleculeName();
      }

      private static QuantityPKParameter quantityPKParameterFor(ICache<int, GlobalPKAnalysis> globalIndividualPKParameterCache, IParameter pKParameter, string quantityPath)
      {
         var pKValuesForPKParameter = globalIndividualPKParameterCache.SelectMany(globalPKAnalysisForAnIndividual => globalPKAnalysisForAnIndividual.AllPKParameters.Where(x => pathsEqual(x, pKParameter))).ToList();
         var firstParameter = pKValuesForPKParameter.FirstOrDefault();

         if (firstParameter == null)
            return new QuantityPKParameter();

         var quantityPKParameter = new QuantityPKParameter { Dimension = firstParameter.Dimension, Name = firstParameter.Name, QuantityPath = quantityPath };

         pKValuesForPKParameter.Each(pKValue => { quantityPKParameter.SetValue(pKValuesForPKParameter.IndexOf(pKValue), (float)pKValue.Value); });
         return quantityPKParameter;
      }

      private static bool pathsEqual(IParameter x, IParameter parameter)
      {
         return string.Equals(x.EntityPath(), parameter.EntityPath());
      }

      private void addFractionAbsorbedWarningTo(IParameter fractionAbsorbed, IReadOnlyList<IParameter> pkParameters)
      {
         if (ValueComparer.AreValuesEqual(fractionAbsorbed.Value, 1, CoreConstants.DOUBLE_RELATIVE_EPSILON))
            return;

         pkParameters.Each(p => p.Rules.Add(warningRule(FractionAbsorbedSmallerThanOne)));
      }

      private IParameter fractionAbsorbedFor(DataColumn fractionAbsorbedCurve)
      {
         double? fractionAbsorbedValue = null;
         if (fractionAbsorbedCurve != null)
            fractionAbsorbedValue = fractionAbsorbedCurve.Values.Last();

         return createParameter(FractionAbsorbed, fractionAbsorbedValue, Fraction);
      }

      private static string pkParameterNameForAUCRatio(ApplicationType applicationType)
      {
         return applicationType == ApplicationType.Multiple ? AUC_inf_tDLast : AUC_inf;
      }

      private static string pkParameterNameForCmaxRatio(ApplicationType applicationType)
      {
         return applicationType == ApplicationType.Multiple ? C_max_tDLast_tDEnd : C_max;
      }

      private DataColumn bloodCurveForSpecies(DataColumn peripheralVenousBloodCurve, DataColumn venousBloodCurve, Individual individual)
      {
         if (individual != null && individual.Species.NameIsOneOf(SpeciesUsingVenousBlood))
            return venousBloodCurve;

         return peripheralVenousBloodCurve;
      }

      private double calculateVSSPhysChemFor(Simulation simulation, string compoundName)
      {
         return _vssCalculator.VSSPhysChemFor(simulation, compoundName).Value;
      }

      private IParameter createParameter(string name, double? defaultValue, string dimensionName)
      {
         var parameter = _parameterFactory.CreateFor(name, defaultValue.GetValueOrDefault(double.NaN), dimensionName, PKSimBuildingBlockType.Simulation);

         if (!defaultValue.HasValue)
         {
            var nullParameter = new NullParameter();
            nullParameter.UpdatePropertiesFrom(parameter, _cloner);
            parameter = nullParameter;
         }

         parameter.Rules.Clear();
         return parameter;
      }

      private IParameter createRatioParameter(string name, double? numerator, double? denominator, string dimensionName)
      {
         double? value = null;
         if (numerator.HasValue && denominator.HasValue)
            value = numerator.Value / denominator.Value;

         return createParameter(name, value, dimensionName);
      }

      private IBusinessRule bioAvailabilityRule { get; } = CreateRule.For<IParameter>()
         .Property(item => item.Value)
         .WithRule((param, value) => false)
         .WithError((param, value) => BioAvailabilityAndFractionAbsorbed);

      private IBusinessRule fractionAbsorbedRule { get; } = CreateRule.For<IParameter>()
         .Property(item => item.Value)
         .WithRule((param, value) => value <= 1)
         .WithError((param, value) => FractionAbsorbedAndEHC);

      public void CalculateBioavailabilityFor(Simulation simulation, string compoundName)
      {
         var compound = simulation.Compounds.FindByName(compoundName);

         //BA calculated with a Simple IV with 15 min infusion
         var simpleIvProtocol = _protocolFactory.Create(ProtocolMode.Simple, ApplicationTypes.Intravenous)
            .WithName("Simple IV for Bioavailability")
            .DowncastTo<SimpleProtocol>();
         simpleIvProtocol.Parameter(INFUSION_TIME).Value = 15;

         var simulationSingleDosingItem = singleDosingItem(simulation, compound);
         simpleIvProtocol.Dose.DisplayUnit = simulationSingleDosingItem.Dose.DisplayUnit;
         simpleIvProtocol.Dose.Value = simulationSingleDosingItem.Dose.Value;
         simpleIvProtocol.StartTime.Value = simulationSingleDosingItem.StartTime.Value;

         var ivSimulation = _globalPKAnalysisRunner.RunForBioavailability(simpleIvProtocol, simulation, compound);

         var ivCompoundPKContext = new CompoundPKContext();
         if (ivSimulation is PopulationSimulation ivPopulationSimulation)
         {
            // We know this is a valid cast because it is cloned from simulation
            var populationSimulation = simulation.DowncastTo<PopulationSimulation>();
            CalculateFor(ivPopulationSimulation, ivCompoundPKContext);
            var contextForPopulationSimulation = createContextForPopulationSimulation(ivCompoundPKContext, mapFromBioavailabilityCompoundPK, compoundName, populationSimulation, new []{Bioavailability});
            populationSimulation.PKAnalyses = CalculateFor(populationSimulation, contextForPopulationSimulation);

         }
         else if (ivSimulation is IndividualSimulation ivIndividualSimulation)
         {
            var venousBloodCurve = ivIndividualSimulation.VenousBloodColumn(compoundName);
            var pkVenousBlood = CalculateFor(ivSimulation, venousBloodCurve).PKAnalysis;
            var aucInf = pkParameterValue(pkVenousBlood, AUC_inf);

            // We know this is a valid cast because it is cloned from simulation
            var individualSimulation = simulation.DowncastTo<IndividualSimulation>();
            individualSimulation.AucIV[compound.Name] = aucInf;
         }

         simulation.HasChanged = true;
      }

      private static void removeQuantityPKParameterFromCompound(string compoundName, CompoundPKContext contextForPopulationSimulation, string parameterName)
      {
         if (contextForPopulationSimulation.CompoundPKFor(compoundName).QuantityPKParameters().Contains(parameterName))
            contextForPopulationSimulation.CompoundPKFor(compoundName).QuantityPKParameters().Remove(parameterName);
      }

      public void CalculateDDIRatioFor(Simulation simulation)
      {
         if (simulation == null) return;

         var ddiRatioSimulation = _globalPKAnalysisRunner.RunForDDIRatio(simulation);

         simulation.CompoundNames.Each(compoundName =>
         {
            var compound = simulation.Compounds.FindByName(compoundName);
            var applicationType = applicationTypeFor(simulation, compound);

            var aucPKParameterName = pkParameterNameForAUCRatio(applicationType);
            var cmaxPKParameterName = pkParameterNameForCmaxRatio(applicationType);

            if (simulation is PopulationSimulation populationSimulation)
            {
               // We know this is a valid cast because it is cloned from simulation
               var ddiPopulationSimulation = ddiRatioSimulation.DowncastTo<PopulationSimulation>();

               var ddiCompoundPKContext = new CompoundPKContext();

               // First calculate the compound context. We will know the AucInf and CMax when DDI is turned off
               CalculateFor(ddiPopulationSimulation, ddiCompoundPKContext);
               var parameterNames = new[] { AUCRatio, C_maxRatio };

               // From the ddi context, create the context from the original simulation and the ddi simulation for calculating ddi ratio parameters
               var contextForPopulationSimulation = createContextForPopulationSimulation(ddiCompoundPKContext, mapFromDDISimulationCompoundPK, compoundName, populationSimulation, parameterNames);

               // Calculate all the parameters again, including now - ddi ratio parameters from the context
               populationSimulation.PKAnalyses = CalculateFor(populationSimulation, contextForPopulationSimulation);
            }
            else if (simulation is IndividualSimulation individualSimulation)
            {
               var ddiIndividualRatioSimulation = ddiRatioSimulation.DowncastTo<IndividualSimulation>();
               individualSimulation.AucDDI[compoundName] = pkParameterInBloodCurveFor(ddiIndividualRatioSimulation, compoundName, aucPKParameterName);
               individualSimulation.CMaxDDI[compoundName] = pkParameterInBloodCurveFor(ddiIndividualRatioSimulation, compoundName, cmaxPKParameterName);
            }
         });
         simulation.HasChanged = true;
      }

      private CompoundPKContext createContextForPopulationSimulation(CompoundPKContext contextCompoundPK, Func<CompoundPK, CompoundPK> compoundPKMapper, string compoundName, PopulationSimulation populationSimulation, string[] parameterNames)
      {
         var contextForPopulationSimulation = new CompoundPKContext();
         contextForPopulationSimulation.AddCompoundPK(compoundPKMapper(contextCompoundPK.CompoundPKFor(compoundName)));
         contextForPopulationSimulation.InitializeQuantityPKParametersFrom(populationSimulation);
         parameterNames.Each(parameterName => { removeQuantityPKParameterFromCompound(compoundName, contextForPopulationSimulation, parameterName); });
         return contextForPopulationSimulation;
      }

      private CompoundPK mapFromBioavailabilityCompoundPK(CompoundPK bioAvailabilitySimulationCompoundPK)
      {
         var compoundPK = new CompoundPK { CompoundName = bioAvailabilitySimulationCompoundPK.CompoundName };
         bioAvailabilitySimulationCompoundPK.AllBioAvailabilityAucInf.KeyValues.Each(bioAvailability => { compoundPK.AddBioavailability(bioAvailability.Key, bioAvailability.Value); });
         return compoundPK;
      }

      private CompoundPK mapFromDDISimulationCompoundPK(CompoundPK ddiSimulationCompoundPK)
      {
         var compoundPK = new CompoundPK { CompoundName = ddiSimulationCompoundPK.CompoundName };
         ddiSimulationCompoundPK.AllDDIAucInf.KeyValues.Each(aucInf => compoundPK.AddDDIAucInf(aucInf.Key, aucInf.Value));
         ddiSimulationCompoundPK.AllDDICMax.KeyValues.Each(cMax => compoundPK.AddDDICMax(cMax.Key, cMax.Value));
         return compoundPK;
      }

      private double? pkParameterInBloodCurveFor(IndividualSimulation simulation, string compoundName, string pkParameterName)
      {
         var pkAnalysis = bloodPkAnalysisFor(simulation, compoundName);
         return pkParameterValue(pkAnalysis, pkParameterName);
      }

      private double? pkParameterValue(PKAnalysis pkAnalysis, string pkParameterName)
      {
         var parameter = pkAnalysis.Parameter(pkParameterName);
         return parameter?.Value;
      }

      private PKAnalysis bloodPkAnalysisFor(IndividualSimulation simulation, string compoundName)
      {
         var peripheralVenousBloodCurve = simulation.PeripheralVenousBloodColumn(compoundName);
         var venousBloodCurve = simulation.VenousBloodColumn(compoundName);
         if (peripheralVenousBloodCurve == null || venousBloodCurve == null)
            return new PKAnalysis();

         var bloodCurveForPKAnalysis = bloodCurveForSpecies(peripheralVenousBloodCurve, venousBloodCurve, simulation.Individual);
         return CalculateFor(simulation, bloodCurveForPKAnalysis).PKAnalysis;
      }

      private bool isMultipleOral(Simulation simulation, Compound compound)
      {
         var allSchemaItems = schemaItemsFrom(simulation, compound);
         return allSchemaItems.Any() && allSchemaItems.All(isOral);
      }

      private static bool isOral(ISchemaItem schemaItem)
      {
         if (schemaItem == null)
            return false;

         return schemaItem.ApplicationType == ApplicationTypes.Oral;
      }

      private static bool isIntravenous(ISchemaItem schemaItem)
      {
         if (schemaItem == null)
            return false;

         return schemaItem.ApplicationType == ApplicationTypes.Intravenous || schemaItem.ApplicationType == ApplicationTypes.IntravenousBolus;
      }

      private ApplicationType applicationTypeFor(Simulation simulation, Compound compound)
      {
         var numberOfApplications = simulation.AllApplicationParametersOrderedByStartTimeFor(compound.Name).Count;
         if (numberOfApplications == 1)
            return ApplicationType.Single;

         return numberOfApplications > 1 ? ApplicationType.Multiple : ApplicationType.Empty;
      }

      private ISchemaItem singleDosingItem(Simulation simulation, Compound compound)
      {
         //this may be null for compound created by metabolization process only
         return schemaItemsFrom(simulation, compound).FirstOrDefault();
      }

      private IReadOnlyList<ISchemaItem> schemaItemsFrom(Simulation simulation, Compound compound)
      {
         var protocol = simulation.CompoundPropertiesFor(compound).ProtocolProperties.Protocol;
         if (protocol == null)
            return new List<ISchemaItem>();

         return _protocolToSchemaItemsMapper.MapFrom(protocol);
      }

      private enum ApplicationType
      {
         //One application exactly
         Single,

         //Two or more applications
         Multiple,

         //No application at all
         Empty
      }

      public IEnumerable<PopulationPKAnalysis> CalculateFor(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData, bool firstOnCurves = true)
      {
         var pkAnalyses = new List<PopulationPKAnalysis>();

         if (timeProfileChartData == null)
            return pkAnalyses; // there are no analyses to calculate

         var allColumns = timeProfileChartData.Panes.SelectMany(x => x.Curves).SelectMany(x =>
               columnsFor(x, populationDataCollector).Select(column => new { curveData = x, column }))
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
               var moleculeName = observedDataColumn.Repository.ExtendedPropertyValueFor(MOLECULE);
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

      private PKValues calculatePK(DataColumn column, PKCalculationOptions options)
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
         var umolPerLiterUnit = dimension.UnitOrDefault(MicroMolPerLiter);
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
         var fractionAbsorbed = globalPKAnalysis.PKParameter(moleculeName, FractionAbsorbed);
         if (fractionAbsorbed == null)
            return;

         if (ValueComparer.AreValuesEqual(fractionAbsorbed.Value, 1, CoreConstants.DOUBLE_RELATIVE_EPSILON))
            return;

         addWarningsTo(pkAnalysis, FractionAbsorbedSmallerThanOne, AllParametersInfluencedByFractionAbsorbed);
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
         var splitStrings = text.Split(new[] { "Range" }, StringSplitOptions.None);
         var match = splitStrings.Length == 2;

         if (!match)
            return (text, text);

         var upperAndLowerRange = splitStrings.Last().Split(new[] { "to" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

         return ($"{splitStrings[0]}{upperAndLowerRange[0]}", $"{splitStrings[0]}{upperAndLowerRange[1]}");
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