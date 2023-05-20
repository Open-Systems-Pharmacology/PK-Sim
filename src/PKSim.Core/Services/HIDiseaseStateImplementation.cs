using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using static OSPSuite.Core.Domain.Constants.Parameters;
using static PKSim.Core.CoreConstants.Units;
using static PKSim.Core.CoreConstants.Organ;
using static PKSim.Core.CoreConstants.Parameters;
using HIFactors = OSPSuite.Utility.Collections.Cache<double, double>;
using IFormulaFactory = OSPSuite.Core.Domain.Formulas.IFormulaFactory;

namespace PKSim.Core.Services
{
   public class HIDiseaseStateImplementation : AbstractDiseaseStateImplementation
   {
      public static class ChildPughScore
      {
         public static double A = 0;
         public static double B = 1;
         public static double C = 2;
      }

      private static HIFactors createFactors(double forA, double forB, double forC) =>
         new()
         {
            {ChildPughScore.A, forA},
            {ChildPughScore.B, forB},
            {ChildPughScore.C, forC},
         };

      private static readonly HIFactors _portalFlowScalingFactor = createFactors(0.4, 0.36, 0.04);
      private static readonly HIFactors _hepaticFlowScalingFactor = createFactors(1.61, 2.34, 5.29);
      private static readonly HIFactors _hepaticVolumeScalingFactor = createFactors(0.69, 0.55, 0.28);
      private static readonly HIFactors _renalFlowScalingFactor = createFactors(0.88, 0.65, 0.48);
      private static readonly HIFactors _cardiacIndexScalingFactor = createFactors(1.11, 1.27, 1.36);
      private static readonly HIFactors _gfrScalingFactor = createFactors(1, 0.7, 0.36);
      private static readonly HIFactors _albuminScalingFactor = createFactors(0.81, 0.68, 0.5);
      private static readonly HIFactors _agpScalingFactor = createFactors(0.6, 0.56, 0.3);
      private static readonly HIFactors _hematocritScalingFactor = createFactors(0.866, 0.822, 0.778);

      private static readonly Cache<string, HIFactors> _moleculeScalingFactorEdginton = new()
      {
         {"CYP3A4", createFactors(1, 0.4, 0.4)},
         {"CYP1A2", createFactors(1, 0.1, 0.1)},
         {"CYP2E1", createFactors(1, 0.83, 0.83)},
      };

      private static readonly Cache<string, HIFactors> _moleculeScalingFactorJohnson = new()
      {
         {"CYP2A6", createFactors(0.89, 0.62, 0.32)},
         {"CYP2B6", createFactors(1, 0.9, 0.8)},
         {"CYP2C8", createFactors(0.69, 0.52, 0.33)},
         {"CYP2C9", createFactors(0.69, 0.51, 0.33)},
         {"CYP2C18", createFactors(0.32, 0.26, 0.12)},
         {"CYP2C19", createFactors(0.32, 0.26, 0.12)},
         {"CYP2D6", createFactors(0.76, 0.33, 0.11)},
      };

      private const int HI_EDGINTON_VALUE_ORIGIN_ID = 93;
      private const int HI_JOHNSON_VALUE_ORIGIN_ID = 94;

      private readonly IDimension _ageDimension;

      public HIDiseaseStateImplementation(IDimensionRepository dimensionRepository,
         IValueOriginRepository valueOriginRepository,
         IFormulaFactory formulaFactory,
         IIndividualFactory individualFactory,
         IContainerTask containerTask,
         IParameterSetUpdater parameterSetUpdater) : base(valueOriginRepository, formulaFactory, individualFactory, containerTask, parameterSetUpdater, CoreConstants.DiseaseStates.HI)
      {
         _ageDimension = dimensionRepository.AgeInYears;
      }

      public override bool ApplyTo(Individual individual) => applyTo(individual, UpdateParameter);

      public override bool ApplyForPopulationTo(Individual individual) => applyTo(individual, x=>UpdateParameterValue);

      private bool applyTo(Individual individual, Func<int, Action<ParameterUpdate>> updateParameterFunc)
      {
         updateBloodFlowsAndVolumes(individual, updateParameterFunc(HI_JOHNSON_VALUE_ORIGIN_ID));
         updateGFR(individual, updateParameterFunc(HI_EDGINTON_VALUE_ORIGIN_ID));
         updateOntogenyFactory(individual, updateParameterFunc(HI_EDGINTON_VALUE_ORIGIN_ID));
         updateHematocrit(individual, updateParameterFunc(HI_EDGINTON_VALUE_ORIGIN_ID));
         return true;
      }

      protected override IReadOnlyList<IParameter> ParameterChangedByDiseaseStateAsList(Individual individual)
      {
         var organism = individual.Organism;
         var kidney = organism.Organ(KIDNEY);
         var liver = organism.Organ(LIVER);

         //Oddly enough the brain blood flow is unaffected within hepatic disease, largely due to compensatory mechanisms from some of the decreased blood flow to other organs.
         var organsBloodFlow = new[] {BONE, FAT, GONADS, HEART, KIDNEY, LIVER, MUSCLE, PANCREAS, LARGE_INTESTINE, SKIN, SMALL_INTESTINE, SPLEEN, STOMACH};
         var bloodFlows = organsBloodFlow.Select(x => organism.Organ(x).Parameter(SPECIFIC_BLOOD_FLOW_RATE)).ToList();

         return new[]
         {
            organism.Parameter(HCT),
            organism.Parameter(ONTOGENY_FACTOR_ALBUMIN),
            organism.Parameter(ONTOGENY_FACTOR_AGP),
            kidney.Parameter(GFR_SPEC),
            liver.Parameter(VOLUME),
         }.Union(bloodFlows).ToList();
      }

      private void updateOntogenyFactory(Individual individual, Action<ParameterUpdate> updateParameterFunc)
      {
         var score = childPughScoreFor(individual);
         var organism = individual.Organism;
         updateParameterFunc(new(organism.Parameter(ONTOGENY_FACTOR_ALBUMIN), _albuminScalingFactor[score]));
         updateParameterFunc(new(organism.Parameter(ONTOGENY_FACTOR_AGP), _agpScalingFactor[score]));
      }

      private void updateGFR(Individual individual, Action<ParameterUpdate> updateParameterFunc)
      {
         var score = childPughScoreFor(individual);
         var kidney = individual.Organism.Organ(KIDNEY);
         var GFR_spec = kidney.Parameter(GFR_SPEC);
         updateParameterFunc(new(GFR_spec, _gfrScalingFactor[score]));
      }

      private void updateHematocrit(Individual individual, Action<ParameterUpdate> updateParameterFunc)
      {
         var score = childPughScoreFor(individual);
         var hct = individual.Organism.Parameter(HCT);
         updateParameterFunc(new(hct, _hematocritScalingFactor[score]));
      }

      private void updateBloodFlowsAndVolumes(Individual individual, Action<ParameterUpdate> updateParameterFunc)
      {
         var score = childPughScoreFor(individual);
         var organism = individual.Organism;
         var updateBloodFlowSpec = updateBloodFlowSpecDef(updateParameterFunc, organism);
         var bloodFlow = bloodFlowDef(organism);
         var portalOrgans = new[] {STOMACH, SMALL_INTESTINE, LARGE_INTESTINE, SPLEEN, PANCREAS};
         var otherOrgans = new[] {BONE, FAT, GONADS, HEART, MUSCLE, SKIN};
         var volumeLiver = organism.Container(LIVER).Parameter(VOLUME);

         //Sum of all healthy portal blood flow + Liver and kidney
         var Q_portal = portalOrgans.Sum(bloodFlow);
         var Q_other = otherOrgans.Sum(bloodFlow);
         var Q_brain = bloodFlow(BRAIN);
         var Q_kidney = bloodFlow(KIDNEY);
         var Q_liver = bloodFlow(LIVER);

         var portal_factor = _portalFlowScalingFactor[score];
         var liver_factor = _hepaticFlowScalingFactor[score];
         var kidney_factor = _renalFlowScalingFactor[score];
         var ci_factor = _cardiacIndexScalingFactor[score];

         //update liver volume so that we get the correct diseases blood flow as Q_liver_HI = f(V_liver)
         updateParameterFunc(new(volumeLiver, _hepaticVolumeScalingFactor[score]));

         var Q_liver_HI = bloodFlow(LIVER);

         //update all blood flows specs
         portalOrgans.Each(x => updateBloodFlowSpec(x, portal_factor));
         updateBloodFlowSpec(LIVER, liver_factor);
         updateBloodFlowSpec(KIDNEY, kidney_factor);

         //retrieve the scaling factor based on publication and github entry
         //see https://github.com/Open-Systems-Pharmacology/Forum/discussions/1341
         var otherOrganDiseaseFactor = (ci_factor * (Q_other + Q_portal + Q_kidney + Q_liver + Q_brain) - (Q_brain + Q_portal * portal_factor + Q_liver_HI * liver_factor + Q_kidney * kidney_factor)) / Q_other;
         otherOrgans.Each(x => updateBloodFlowSpec(x, otherOrganDiseaseFactor));
      }

      private Action<string, double> updateBloodFlowSpecDef(Action<ParameterUpdate> updateParameterFunc, IContainer organism) => (organName, factor) =>
      {
         var parameter = organism.Container(organName).Parameter(SPECIFIC_BLOOD_FLOW_RATE);
         updateParameterFunc(new(parameter, factor));
      };

      private Func<string, double> bloodFlowDef(IContainer organism) => (organName) => organism.Container(organName).Parameter(BLOOD_FLOW).Value;

      private void updateReferenceConcentration(Individual individual, IndividualMolecule individualMolecule, HIFactors factors, int valueOriginId)
      {
         var score = childPughScoreFor(individual);
         UpdateParameter(valueOriginId)(new(individualMolecule.ReferenceConcentration, factors[score]));
      }

      private double childPughScoreFor(Individual individual) => individual.OriginData.DiseaseStateParameters.FindByName(CHILD_PUGH_SCORE).Value;

      public override (bool isValid, string error) IsValid(OriginData originData)
      {
         var ageInYears = _ageDimension.BaseUnitValueToUnitValue(_ageDimension.Unit(Years), originData.Age.Value);
         if (ageInYears >= 18)
            return (true, string.Empty);

         return (false, PKSimConstants.Error.HIOnlyAvailableForAdult);
      }

      public override void ApplyTo(ExpressionProfile expressionProfile, string moleculeName)
      {
         var (molecule, individual) = expressionProfile;
         var moleculeNameToUse = moleculeName.ToUpper();
         if (_moleculeScalingFactorEdginton.Contains(moleculeNameToUse))
            updateReferenceConcentration(individual, molecule, _moleculeScalingFactorEdginton[moleculeNameToUse], HI_EDGINTON_VALUE_ORIGIN_ID);

         if (_moleculeScalingFactorJohnson.Contains(moleculeNameToUse))
            updateReferenceConcentration(individual, molecule, _moleculeScalingFactorJohnson[moleculeNameToUse], HI_JOHNSON_VALUE_ORIGIN_ID);
      }

      public override bool CanBeAppliedToExpressionProfile(QuantityType moleculeType)
      {
         return moleculeType.Is(QuantityType.Enzyme);
      }
   }
}