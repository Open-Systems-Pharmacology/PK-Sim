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
using IFormulaFactory = OSPSuite.Core.Domain.Formulas.IFormulaFactory;

namespace PKSim.Core.Services;

public class HIDiseaseStateImplementation : IDiseaseStateImplementation
{
   public static class ChildPughScore
   {
      public static double A = 0;
      public static double B = 1;
      public static double C = 2;
   }

   private static readonly Cache<double, double> _portalFlowScalingFactor = new()
   {
      {ChildPughScore.A, 0.4},
      {ChildPughScore.B, 0.36},
      {ChildPughScore.C, 0.04},
   };

   private static readonly Cache<double, double> _hepaticFlowScalingFactor = new()
   {
      {ChildPughScore.A, 1.3},
      {ChildPughScore.B, 2.3},
      {ChildPughScore.C, 3.4},
   };

   private static readonly Cache<double, double> _hepaticVolumeScalingFactor = new()
   {
      {ChildPughScore.A, 0.69},
      {ChildPughScore.B, 0.55},
      {ChildPughScore.C, 0.28},
   };

   private static readonly Cache<double, double> _renalFlowScalingFactor = new()
   {
      {ChildPughScore.A, 0.88},
      {ChildPughScore.B, 0.65},
      {ChildPughScore.C, 0.65},
   };

   private static readonly Cache<double, double> _otherOrgansFlowScalingFactor = new()
   {
      {ChildPughScore.A, 1.31},
      {ChildPughScore.B, 1.84},
      {ChildPughScore.C, 2.27},
   };

   private static readonly Cache<double, double> _gfrScalingFactor = new()
   {
      {ChildPughScore.A, 1},
      {ChildPughScore.B, 0.7},
      {ChildPughScore.C, 0.36},
   };

   public const string CHILD_PUGH_SCORE = "Child-Pugh score";

   //TODO
   private const int HI_VALUE_ORIGIN_ID = 92;

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
      updateBloodFlows(individual, updateParameter);
      updateVolumes(individual, updateParameter);
      updateGFR(individual, updateParameter);

      return true;
   }

   private void updateGFR(Individual individual, Action<IParameter, double, bool> updateParameterFunc)
   {
      var score = childPughScoreFor(individual);
      var kidney = individual.Organism.Organ(KIDNEY);
      var GFR_spec = kidney.Parameter(GFR_SPEC);
      updateParameterFunc(GFR_spec, _gfrScalingFactor[score], true);
   }

   private void updateBloodFlows(Individual individual, Action<IParameter, double, bool> updateParameterFunc)
   {
      var score = childPughScoreFor(individual);
      var organism = individual.Organism;
      var updateBloodFlow = updateBloodFlowDef(updateParameterFunc, organism);

      //PortalGF
      var portalOrgans = new[] { STOMACH, SMALL_INTESTINE, LARGE_INTESTINE, SPLEEN, PANCREAS };
      portalOrgans.Each(x => updateBloodFlow(x, _portalFlowScalingFactor[score]));

      //Hepatic
      updateBloodFlow(LIVER, _hepaticFlowScalingFactor[score]);

      //Renal
      updateBloodFlow(KIDNEY, _renalFlowScalingFactor[score]);

      //other organs
      var otherOrgans = new[] { BONE, FAT, GONADS, HEART, MUSCLE, SKIN };
      otherOrgans.Each(x => updateBloodFlow(x, _otherOrgansFlowScalingFactor[score]));
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

   private void updateParameter(IParameter parameter, double value, bool useAsFactor)
   {
      var valueToUse = useAsFactor ? parameter.Value * value : parameter.Value;
      updateValueOriginsFor(parameter);
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
   }

   private void updateValueOriginsFor(IParameter parameter)
   {
      parameter.ValueOrigin.UpdateAllFrom(_valueOriginRepository.FindBy(HI_VALUE_ORIGIN_ID));
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

   public void ApplyTo(IndividualMolecule individualMolecule)
   {
      //nothing to do here
   }
}