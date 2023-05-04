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
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public class HIDiseaseStateImplementation : AbstractDiseaseStateImplementation
   {
      private readonly IParameterFactory _parameterFactory;

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
      private static readonly HIFactors _hepaticFlowScalingFactor = createFactors(1.3, 2.3, 3.4);
      private static readonly HIFactors _hepaticVolumeScalingFactor = createFactors(0.69, 0.55, 0.28);
      private static readonly HIFactors _renalFlowScalingFactor = createFactors(0.88, 0.65, 0.65);
      private static readonly HIFactors _otherOrgansFlowScalingFactor = createFactors(1.31, 1.84, 2.27);
      private static readonly HIFactors _gfrScalingFactor = createFactors(1, 0.7, 0.36);
      private static readonly HIFactors _albuminFactor = createFactors(0.81, 0.68, 0.5);
      private static readonly HIFactors _agpFactor = createFactors(0.6, 0.56, 0.3);
      private static readonly HIFactors _hematocritScalingFactor = createFactors(0.92, 0.88, 0.83);

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

      public const string CHILD_PUGH_SCORE = "Child-Pugh score";

      private const int HI_EDGINTON_VALUE_ORIGIN_ID = 93;
      private const int HI_JOHNSON_VALUE_ORIGIN_ID = 94;

      private readonly IDimension _ageDimension;

      public HIDiseaseStateImplementation(IDimensionRepository dimensionRepository,
         IParameterFactory parameterFactory,
         IValueOriginRepository valueOriginRepository,
         IFormulaFactory formulaFactory,
         IIndividualFactory individualFactory,
         IContainerTask containerTask,
         IParameterSetUpdater parameterSetUpdater) : base(valueOriginRepository, formulaFactory, individualFactory, containerTask, parameterSetUpdater, CoreConstants.DiseaseStates.HI)
      {
         _parameterFactory = parameterFactory;
         _ageDimension = dimensionRepository.AgeInYears;
      }

      public override bool ApplyTo(Individual individual) => applyTo(individual, UpdateParameter(HI_EDGINTON_VALUE_ORIGIN_ID));

      public override bool ApplyForPopulationTo(Individual individual) => applyTo(individual, UpdateParameterValue);

      private bool applyTo(Individual individual, Action<IParameter, double, bool> updateParameterFunc)
      {
         updateBloodFlows(individual, updateParameterFunc);
         updateVolumes(individual, updateParameterFunc);
         updateGFR(individual, updateParameterFunc);
         updateOntogenyFactory(individual, updateParameterFunc);
         updateHematocrit(individual, updateParameterFunc);

         addChildPughScoreToLiver(individual);
         return true;
      }

      private void addChildPughScoreToLiver(Individual individual)
      {
         var organism = individual.Organism;
         var childPughScore = organism.Parameter(CHILD_PUGH_SCORE);
         if (childPughScore == null)
         {
            childPughScore = createChildPughScore();
            organism.Add(childPughScore);
         }

         childPughScore.Value = childPughScoreFor(individual);
      }

      private IParameter createChildPughScore()
      {
         var childPughScore = _parameterFactory.CreateFor(CHILD_PUGH_SCORE, PKSimBuildingBlockType.Individual)
            .WithGroup(CoreConstants.Groups.INDIVIDUAL_CHARACTERISTICS);
         childPughScore.DefaultValue = null;
         childPughScore.Editable = false;
         return childPughScore;
      }

      protected override IReadOnlyList<IParameter> ParameterChangedByDiseaseStateAsList(Individual individual)
      {
         var organism = individual.Organism;
         var kidney = organism.Organ(KIDNEY);
         var liver = organism.Organ(LIVER);

         var organsBloodFlow = new[] {STOMACH, SMALL_INTESTINE, LARGE_INTESTINE, SPLEEN, PANCREAS, LIVER, KIDNEY, BONE, FAT, GONADS, HEART, MUSCLE, SKIN};
         var bloodFlows = organsBloodFlow.Select(x => organism.Organ(x).Parameter(SPECIFIC_BLOOD_FLOW_RATE)).ToList();

         return new[]
         {
            organism.Parameter(HCT),
            organism.Parameter(CHILD_PUGH_SCORE),
            organism.Parameter(CHILD_PUGH_SCORE),
            organism.Parameter(ONTOGENY_FACTOR_ALBUMIN),
            organism.Parameter(ONTOGENY_FACTOR_AGP),
            kidney.Parameter(GFR_SPEC),
            liver.Parameter(VOLUME),
         }.Union(bloodFlows).ToList();
      }

      private void updateOntogenyFactory(Individual individual, Action<IParameter, double, bool> updateParameterFunc)
      {
         var score = childPughScoreFor(individual);
         var organism = individual.Organism;
         updateParameterFunc(organism.Parameter(ONTOGENY_FACTOR_ALBUMIN), _albuminFactor[score], true);
         updateParameterFunc(organism.Parameter(ONTOGENY_FACTOR_AGP), _agpFactor[score], true);
      }

      private void updateGFR(Individual individual, Action<IParameter, double, bool> updateParameterFunc)
      {
         var score = childPughScoreFor(individual);
         var kidney = individual.Organism.Organ(KIDNEY);
         var GFR_spec = kidney.Parameter(GFR_SPEC);
         updateParameterFunc(GFR_spec, _gfrScalingFactor[score], true);
      }

      private void updateHematocrit(Individual individual, Action<IParameter, double, bool> updateParameterFunc)
      {
         var score = childPughScoreFor(individual);
         var hct = individual.Organism.Parameter(HCT);
         updateParameterFunc(hct, _hematocritScalingFactor[score], true);
      }

      private void updateBloodFlows(Individual individual, Action<IParameter, double, bool> updateParameterFunc)
      {
         var score = childPughScoreFor(individual);
         var organism = individual.Organism;
         var updateBloodFlow = updateBloodFlowDef(updateParameterFunc, organism);

         //PortalGF
         var portalOrgans = new[] {STOMACH, SMALL_INTESTINE, LARGE_INTESTINE, SPLEEN, PANCREAS};
         portalOrgans.Each(x => updateBloodFlow(x, _portalFlowScalingFactor[score]));

         //Hepatic
         updateBloodFlow(LIVER, _hepaticFlowScalingFactor[score]);

         //Renal
         updateBloodFlow(KIDNEY, _renalFlowScalingFactor[score]);

         //other organs
         var otherOrgans = new[] {BONE, FAT, GONADS, HEART, MUSCLE, SKIN};
         otherOrgans.Each(x => updateBloodFlow(x, _otherOrgansFlowScalingFactor[score]));
      }

      private void updateVolumes(Individual individual, Action<IParameter, double, bool> updateParameterFunc)
      {
         var score = childPughScoreFor(individual);
         var organism = individual.Organism;
         var parameter = organism.Container(LIVER).Parameter(VOLUME);
         updateParameterFunc(parameter, _hepaticVolumeScalingFactor[score], true);
      }

      private Action<string, double> updateBloodFlowDef(Action<IParameter, double, bool> updateParameterFunc, IContainer organism) => (organName, factor) =>
      {
         var parameter = organism.Container(organName).Parameter(SPECIFIC_BLOOD_FLOW_RATE);
         updateParameterFunc(parameter, factor, true);
      };

      private void updateReferenceConcentration(Individual individual, IndividualMolecule individualMolecule, HIFactors factors, int valueOriginId)
      {
         var score = childPughScoreFor(individual);
         UpdateParameter(valueOriginId)(individualMolecule.ReferenceConcentration, factors[score], true);
      }

      private double childPughScoreFor(Individual individual) => individual.OriginData.DiseaseStateParameters.FindByName(CHILD_PUGH_SCORE).Value;

      public override (bool isValid, string error) IsValid(OriginData originData)
      {
         var ageInYears = _ageDimension.BaseUnitValueToUnitValue(_ageDimension.Unit(Years), originData.Age.Value);
         if (ageInYears >= 18)
            return (true, string.Empty);

         return (false, PKSimConstants.Error.HIOnlyAvailableForAdult);
      }

      public override void ApplyTo(Individual individual, IndividualMolecule individualMolecule)
      {
         var moleculeName = individualMolecule.Name.ToUpper();
         if (_moleculeScalingFactorEdginton.Contains(moleculeName))
            updateReferenceConcentration(individual, individualMolecule, _moleculeScalingFactorEdginton[moleculeName], HI_EDGINTON_VALUE_ORIGIN_ID);

         if (_moleculeScalingFactorJohnson.Contains(moleculeName))
            updateReferenceConcentration(individual, individualMolecule, _moleculeScalingFactorJohnson[moleculeName], HI_JOHNSON_VALUE_ORIGIN_ID);
      }
   }
}