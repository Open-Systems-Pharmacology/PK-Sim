using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IGlobalPKAnalysisTask
   {
      GlobalPKAnalysis CalculateGlobalPKAnalysisFor(IEnumerable<Simulation> simulations);

      /// <summary>
      ///    Starts the calculation of the bioavailability for the compound named <paramref name="compoundName" /> by switching
      ///    its application
      ///    protocol with an iv protocol
      /// </summary>
      void CalculateBioavailabilityFor(Simulation simulation, string compoundName);

      /// <summary>
      ///    Starts the calculation of the Auc DDI Ratio for the compound named <paramref name="compoundName" /> by switching off
      ///    all other applications
      /// </summary>
      void CalculateDDIRatioFor(Simulation simulation, string compoundName);
   }

   public class GlobalPKAnalysisTask : IGlobalPKAnalysisTask
   {
      private readonly IParameterFactory _parameterFactory;
      private readonly IProtocolToSchemaItemsMapper _protocolToSchemaItemsMapper;
      private readonly IProtocolFactory _protocolFactory;
      private readonly IGlobalPKAnalysisRunner _globalPKAnalysisRunner;
      private readonly IPKAnalysesTask _pkAnalysisTask;
      private readonly IPKCalculationOptionsFactory _pkCalculationOptionsFactory;
      private readonly IVSSCalculator _vssCalculator;
      private readonly IInteractionTask _interactionTask;
      private readonly ICloner _cloner;

      public GlobalPKAnalysisTask(IParameterFactory parameterFactory, IProtocolToSchemaItemsMapper protocolToSchemaItemsMapper,
         IProtocolFactory protocolFactory, IGlobalPKAnalysisRunner globalPKAnalysisRunner, IPKAnalysesTask pkAnalysesTask,
         IPKCalculationOptionsFactory pkCalculationOptionsFactory, IVSSCalculator vssCalculator, IInteractionTask interactionTask, ICloner cloner)
      {
         _parameterFactory = parameterFactory;
         _protocolToSchemaItemsMapper = protocolToSchemaItemsMapper;
         _protocolFactory = protocolFactory;
         _globalPKAnalysisRunner = globalPKAnalysisRunner;
         _pkAnalysisTask = pkAnalysesTask;
         _pkCalculationOptionsFactory = pkCalculationOptionsFactory;
         _vssCalculator = vssCalculator;
         _interactionTask = interactionTask;
         _cloner = cloner;
      }

      public GlobalPKAnalysis CalculateGlobalPKAnalysisFor(IEnumerable<Simulation> simulations)
      {
         var globalPKAnalysis = new GlobalPKAnalysis();

         var allSimulations = simulations.ToList();
         if (allSimulations.Count != 1)
            return globalPKAnalysis;

         var simulation = allSimulations[0] as IndividualSimulation;
         if (simulation == null)
            return globalPKAnalysis;

         //one container per compound
         foreach (var compound in simulation.Compounds)
         {
            var compoundName = compound.Name;
            var container = new Container().WithName(compoundName);
            globalPKAnalysis.Add(container);

            var peripheralVenousBloodCurve = simulation.DataRepository.PeripheralVenousBloodColumn(compoundName);
            var venousBloodCurve = simulation.DataRepository.VenousBloodColumn(compoundName);

            if (peripheralVenousBloodCurve == null || venousBloodCurve == null)
               continue;

            var options = _pkCalculationOptionsFactory.CreateFor(simulation, compoundName);
            var venousBloodPlasmaPK = _pkAnalysisTask.CalculatePK(venousBloodCurve, options);

            var aucIV = simulation.AucIVFor(compoundName);
            var aucDDI = simulation.AucDDIFor(compoundName);
            var cmaxDDI = simulation.CmaxDDIFor(compoundName);

            var bioAvailability = createRatioParameter(CoreConstants.PKAnalysis.Bioavailability, venousBloodPlasmaPK[Constants.PKParameters.AUC_inf], aucIV, Constants.Dimension.DIMENSIONLESS);
            var bioAvailabilityValue = bioAvailability.Value;

            var bloodCurveForPKAnalysis = bloodCurveForSpecies(peripheralVenousBloodCurve, venousBloodCurve, simulation.Individual);
            var bloodPlasmaPK = _pkAnalysisTask.CalculatePK(bloodCurveForPKAnalysis, options);

            var applicationType = applicationTypeFor(simulation, compound);

            var aucRatio = createRatioParameter(CoreConstants.PKAnalysis.AUCRatio, bloodPlasmaPK[pkParameterNameForAUCRatio(applicationType)], aucDDI, Constants.Dimension.DIMENSIONLESS);
            var cmaxRatio = createRatioParameter(CoreConstants.PKAnalysis.C_maxRatio, bloodPlasmaPK[pkParameterNameForCmaxRatio(applicationType)], cmaxDDI, Constants.Dimension.DIMENSIONLESS);

            var vssPlasma = createParameter(CoreConstants.PKAnalysis.VssPlasma, bloodPlasmaPK[Constants.PKParameters.Vss], CoreConstants.Dimension.VolumePerBodyWeight);
            var vssPlasmaOverF = createParameter(CoreConstants.PKAnalysis.VssPlasmaOverF, bloodPlasmaPK[Constants.PKParameters.Vss], CoreConstants.Dimension.VolumePerBodyWeight);
            var vdPlasma = createParameter(CoreConstants.PKAnalysis.VdPlasma, bloodPlasmaPK[Constants.PKParameters.Vd], CoreConstants.Dimension.VolumePerBodyWeight);
            var vdPlasmaOverF = createParameter(CoreConstants.PKAnalysis.VdPlasmaOverF, bloodPlasmaPK[Constants.PKParameters.Vd], CoreConstants.Dimension.VolumePerBodyWeight);
            var vssPhysChem = createParameter(CoreConstants.PKAnalysis.VssPhysChem, calculateVSSPhysChemFor(simulation, compoundName), CoreConstants.Dimension.VolumePerBodyWeight);
            var totalPlasmaCL = createParameter(CoreConstants.PKAnalysis.TotalPlasmaCL, bloodPlasmaPK[Constants.PKParameters.CL], CoreConstants.Dimension.FlowPerWeight);
            var totalPlasmaCLOverF = createParameter(CoreConstants.PKAnalysis.TotalPlasmaCLOverF, bloodPlasmaPK[Constants.PKParameters.CL], CoreConstants.Dimension.FlowPerWeight);

            if (_interactionTask.HasInteractionInvolving(compound, simulation))
               container.AddChildren(aucRatio, cmaxRatio);

            var fractionAbsorbed = fractionAbsorbedFor(simulation, compoundName);

            //multiple application or no application? in that case, only show fraction absorbed for oral
            if (applicationType != ApplicationType.Single)
            {
               if (isMultipleOral(simulation, compound))
                  container.Add(fractionAbsorbed);

               continue;
            }

            var schemaItem = singleDosingItem(simulation, compound);
            var pkValues = new List<IParameter>();
            var bioAvailabilityCalculated = !double.IsNaN(bioAvailabilityValue);

            if (isIntravenous(schemaItem))
            {
               container.AddChildren(vssPlasma, vdPlasma, vssPhysChem, totalPlasmaCL);
               continue;
            }

            var fractionAbsorbedWarningParameters = new List<IParameter>();
            if (bioAvailabilityCalculated)
            {
               vssPlasma.Value *= bioAvailabilityValue;
               vdPlasma.Value *= bioAvailabilityValue;
               totalPlasmaCL.Value *= bioAvailabilityValue;
               fractionAbsorbedWarningParameters.AddRange(new[] {vssPlasma, vdPlasma});
               pkValues.AddRange(new[] {vssPlasma, vdPlasma, totalPlasmaCL, bioAvailability});
            }
            else
            {
               fractionAbsorbedWarningParameters.AddRange(new[] {vssPlasmaOverF, vdPlasmaOverF});
               pkValues.AddRange(new[] {vssPlasmaOverF, vdPlasmaOverF, totalPlasmaCLOverF, bioAvailability});
            }


            if (isOral(schemaItem))
            {
               fractionAbsorbed.Rules.Add(fractionAbsorbedRule);
               pkValues.Add(fractionAbsorbed);

               if (bioAvailability.Value > fractionAbsorbed.Value)
                  bioAvailability.Rules.Add(bioAvailabilityRule);

               addFractionAbsorvedWarningTo(fractionAbsorbed, fractionAbsorbedWarningParameters);
            }

            container.AddChildren(pkValues);
         }

         return globalPKAnalysis;
      }

      private void addFractionAbsorvedWarningTo(IParameter fractionAbsorbed, IReadOnlyList<IParameter> pkParameters)
      {
         if (ValueComparer.AreValuesEqual(fractionAbsorbed.Value, 1, CoreConstants.DOUBLE_RELATIVE_EPSILON))
            return;

         pkParameters.Each(p => p.Rules.Add(warningRule(PKSimConstants.Warning.FractionAbsorbedSmallerThanOne)));
      }

      private IBusinessRule warningRule(string warning)
      {
         return CreateRule.For<IParameter>()
            .Property(item => item.Value)
            .WithRule((param, value) => false)
            .WithError((param, value) => warning);
      }

      private IParameter fractionAbsorbedFor(IndividualSimulation simulation, string compoundName)
      {
         var fabsOralObserver = simulation.DataRepository.FabsOral(compoundName);
         double? fractionAbsorbedValue = null;
         if (fabsOralObserver != null)
            fractionAbsorbedValue = fabsOralObserver.Values.Last();

         return createParameter(CoreConstants.PKAnalysis.FractionAbsorbed, fractionAbsorbedValue, CoreConstants.Dimension.Fraction);
      }

      private static string pkParameterNameForAUCRatio(ApplicationType applicationType)
      {
         return applicationType == ApplicationType.Multiple ? Constants.PKParameters.AUC_inf_tLast : Constants.PKParameters.AUC_inf;
      }

      private static string pkParameterNameForCmaxRatio(ApplicationType applicationType)
      {
         return applicationType == ApplicationType.Multiple ? Constants.PKParameters.C_max_tLast_tEnd : Constants.PKParameters.C_max;
      }

      private DataColumn bloodCurveForSpecies(DataColumn peripheralVenousBloodCurve, DataColumn venousBloodCurve, Individual individual)
      {
         if (individual != null && individual.Species.NameIsOneOf(CoreConstants.Species.SpeciesUsingVenousBlood))
            return venousBloodCurve;

         return peripheralVenousBloodCurve;
      }

      private double calculateVSSPhysChemFor(IndividualSimulation simulation, string compoundName)
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

      private IBusinessRule bioAvailabilityRule
      {
         get
         {
            return CreateRule.For<IParameter>()
               .Property(item => item.Value)
               .WithRule((param, value) => false)
               .WithError((param, value) => PKSimConstants.Warning.BioAvailabilityAndFractionAbsorbed);
         }
      }

      private IBusinessRule fractionAbsorbedRule
      {
         get
         {
            return CreateRule.For<IParameter>()
               .Property(item => item.Value)
               .WithRule((param, value) => value <= 1)
               .WithError((param, value) => PKSimConstants.Warning.FractionAbsorbedAndEHC);
         }
      }

      public void CalculateBioavailabilityFor(Simulation simulation, string compoundName)
      {
         var individualSimulation = simulation as IndividualSimulation;
         if (individualSimulation == null) return;

         var compound = simulation.Compounds.FindByName(compoundName);

         //BA calculated with a Simple IV with 15 min infusion
         var simpleIvProtocol = _protocolFactory.Create(ProtocolMode.Simple, ApplicationTypes.Intravenous)
            .WithName("Simple IV for Bioavailability")
            .DowncastTo<SimpleProtocol>();
         simpleIvProtocol.Parameter(Constants.Parameters.INFUSION_TIME).Value = 15;

         var simulationSingleDosingItem = singleDosingItem(simulation, compound);
         simpleIvProtocol.Dose.DisplayUnit = simulationSingleDosingItem.Dose.DisplayUnit;
         simpleIvProtocol.Dose.Value = simulationSingleDosingItem.Dose.Value;
         simpleIvProtocol.StartTime.Value = simulationSingleDosingItem.StartTime.Value;

         var ivSimulation = _globalPKAnalysisRunner.RunForBioavailability(simpleIvProtocol, individualSimulation, compound);
         var venousBloodCurve = ivSimulation.DataRepository.VenousBloodColumn(compoundName);
         var pkVenousBlood = _pkAnalysisTask.CalculateFor(ivSimulation, venousBloodCurve).PKAnalysis;
         individualSimulation.CompoundPKFor(compoundName).AucIV = pkParameterValue(pkVenousBlood, Constants.PKParameters.AUC_inf);
      }

      public void CalculateDDIRatioFor(Simulation simulation, string compoundName)
      {
         var individualSimulation = simulation as IndividualSimulation;
         if (individualSimulation == null) return;

         var compound = simulation.Compounds.FindByName(compoundName);
         var applicationType = applicationTypeFor(simulation, compound);

         var aucPKParameterName = pkParameterNameForAUCRatio(applicationType);
         var cmaxPKParameterName = pkParameterNameForCmaxRatio(applicationType);
         var ddiRatioSimulation = _globalPKAnalysisRunner.RunForDDIRatio(individualSimulation);
         individualSimulation.CompoundPKFor(compoundName).AucDDI = pkParameterInBloodCurveFor(ddiRatioSimulation, compoundName, aucPKParameterName);
         individualSimulation.CompoundPKFor(compoundName).CmaxDDI = pkParameterInBloodCurveFor(ddiRatioSimulation, compoundName, cmaxPKParameterName);
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
         var peripheralVenousBloodCurve = simulation.DataRepository.PeripheralVenousBloodColumn(compoundName);
         var venousBloodCurve = simulation.DataRepository.VenousBloodColumn(compoundName);
         if (peripheralVenousBloodCurve == null || venousBloodCurve == null)
            return new PKAnalysis();

         var bloodCurveForPKAnalysis = bloodCurveForSpecies(peripheralVenousBloodCurve, venousBloodCurve, simulation.Individual);
         return _pkAnalysisTask.CalculateFor(simulation, bloodCurveForPKAnalysis).PKAnalysis;
      }

      private bool isMultipleOral(Simulation simulation, Compound compound)
      {
         var allSchemaItems = schemaItemsFrom(simulation, compound);
         return allSchemaItems.Any() && allSchemaItems.All(isOral);
      }

      private static bool isOral(ISchemaItem schemaItem)
      {
         return schemaItem.ApplicationType == ApplicationTypes.Oral;
      }

      private static bool isIntravenous(ISchemaItem schemaItem)
      {
         return schemaItem.ApplicationType == ApplicationTypes.Intravenous || schemaItem.ApplicationType == ApplicationTypes.IntravenousBolus;
      }

      private ApplicationType applicationTypeFor(Simulation simulation, Compound compound)
      {
         var numberOfApplications = schemaItemsFrom(simulation, compound).Count;
         if (numberOfApplications == 1)
            return ApplicationType.Single;

         return numberOfApplications > 1 ? ApplicationType.Multiple : ApplicationType.Empty;
      }

      private ISchemaItem singleDosingItem(Simulation simulation, Compound compound)
      {
         return schemaItemsFrom(simulation, compound)[0];
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
   }
}