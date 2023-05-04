using System;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using static PKSim.Core.CoreConstants.Units;
using static PKSim.Core.CoreConstants.Organ;
using static PKSim.Core.CoreConstants.Parameters;
using HIFactors = OSPSuite.Utility.Collections.Cache<double, double>;
using IFormulaFactory = OSPSuite.Core.Domain.Formulas.IFormulaFactory;

namespace PKSim.Core.Services
{
   public class HIDiseaseStateImplementation : IDiseaseStateImplementation
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
      private static readonly HIFactors _hepaticFlowScalingFactor = createFactors(1.3, 2.3, 3.4);
      private static readonly HIFactors _hepaticVolumeScalingFactor = createFactors(0.69, 0.55, 0.28);
      private static readonly HIFactors _renalFlowScalingFactor = createFactors(0.88, 0.65, 0.65);
      private static readonly HIFactors _otherOrgansFlowScalingFactor = createFactors(1.31, 1.84, 2.27);
      private static readonly HIFactors _gfrScalingFactor = createFactors(1, 0.7, 0.36);
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
      private readonly IValueOriginRepository _valueOriginRepository;
      private readonly IFormulaFactory _formulaFactory;

      public HIDiseaseStateImplementation(IDimensionRepository dimensionRepository, IValueOriginRepository valueOriginRepository, IFormulaFactory formulaFactory)
      {
         _valueOriginRepository = valueOriginRepository;
         _formulaFactory = formulaFactory;
         _ageDimension = dimensionRepository.AgeInYears;
      }

      public bool IsSatisfiedBy(DiseaseState diseaseState) => diseaseState.IsNamed(CoreConstants.DiseaseStates.HI);

      public bool ApplyTo(Individual individual)
      {
         var updateParameterEdginton = updateParameter(HI_EDGINTON_VALUE_ORIGIN_ID);
         updateBloodFlows(individual, updateParameterEdginton);
         updateVolumes(individual, updateParameterEdginton);
         updateGFR(individual, updateParameterEdginton);
         updateHematocrit(individual, updateParameterEdginton);

         return true;
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

      private void updateReferenceConcentration(Individual individual, IndividualMolecule individualMolecule, HIFactors factors, int valueOriginId)
      {
         var score = childPughScoreFor(individual);
         updateParameter(valueOriginId)(individualMolecule.ReferenceConcentration, factors[score], true);
      }

      private void updateVolumes(Individual individual, Action<IParameter, double, bool> updateParameterFunc)
      {
         var score = childPughScoreFor(individual);
         var organism = individual.Organism;
         var parameter = organism.Container(LIVER).Parameter(Constants.Parameters.VOLUME);
         updateParameterFunc(parameter, _hepaticVolumeScalingFactor[score], true);
      }

      private Action<string, double> updateBloodFlowDef(Action<IParameter, double, bool> updateParameterFunc, IContainer organism) => (organName, factor) =>
      {
         var parameter = organism.Container(organName).Parameter(SPECIFIC_BLOOD_FLOW_RATE);
         updateParameterFunc(parameter, factor, true);
      };

      private double childPughScoreFor(Individual individual)
      {
         return individual.OriginData.DiseaseStateParameters.FindByName(CHILD_PUGH_SCORE).Value;
      }

      public bool ApplyForPopulationTo(Individual individual)
      {
         throw new NotImplementedException();
      }

      public Individual CreateBaseIndividualForPopulation(Individual originalIndividual)
      {
         throw new NotImplementedException();
      }

      public void ResetParametersAfterPopulationIteration(Individual individual)
      {
         throw new NotImplementedException();
      }

      private void updateParameterValue(IParameter parameter, double value, bool useAsFactor)
      {
         var valueToUse = useAsFactor ? parameter.Value * value : parameter.Value;
         parameter.Value = valueToUse;
      }

      private Action<IParameter, double, bool> updateParameter(int valueOriginId) => (parameter, value, useAsFactor) =>
      {
         var valueToUse = useAsFactor ? parameter.Value * value : parameter.Value;
         updateValueOriginsFor(parameter, valueOriginId);
         if (parameter is IDistributedParameter distributedParameter)
         {
            distributedParameter.ScaleDistributionBasedOn(valueToUse / distributedParameter.Value);
            return;
         }

         //We are using a formula, we override with a constant
         if (parameter.Formula.IsExplicit())
         {
            parameter.Formula = _formulaFactory.ConstantFormula(valueToUse, parameter.Dimension);
            //Make sure the formula is indeed used in case the value was overwritten before as fixed value
            parameter.IsFixedValue = false;
            return;
         }

         //constant formula
         updateParameterValue(parameter, value, useAsFactor);
         parameter.DefaultValue = valueToUse;
         parameter.IsFixedValue = false;
      };

      private void updateValueOriginsFor(IParameter parameter, int valueOriginId)
      {
         parameter.ValueOrigin.UpdateAllFrom(_valueOriginRepository.FindBy(valueOriginId));
         //Make sure we mark this parameter as changed by create individual. It might already be the case but in that case, it does not change anything
         parameter.IsChangedByCreateIndividual = true;
      }

      public void Validate(OriginData originData)
      {
         var (valid, error) = IsValid(originData);
         if (valid)
            return;

         throw new OSPSuiteException(error);
      }

      public (bool isValid, string error) IsValid(OriginData originData)
      {
         var ageInYears = _ageDimension.BaseUnitValueToUnitValue(_ageDimension.Unit(Years), originData.Age.Value);
         if (ageInYears >= 18)
            return (true, string.Empty);

         return (false, PKSimConstants.Error.HIOnlyAvailableForAdult);
      }

      public void ApplyTo(Individual individual, IndividualMolecule individualMolecule)
      {
         var moleculeName = individualMolecule.Name.ToUpper();
         if (_moleculeScalingFactorEdginton.Contains(moleculeName))
            updateReferenceConcentration(individual, individualMolecule, _moleculeScalingFactorEdginton[moleculeName], HI_EDGINTON_VALUE_ORIGIN_ID);

         if (_moleculeScalingFactorJohnson.Contains(moleculeName))
            updateReferenceConcentration(individual, individualMolecule, _moleculeScalingFactorJohnson[moleculeName], HI_JOHNSON_VALUE_ORIGIN_ID);
      }
   }
}