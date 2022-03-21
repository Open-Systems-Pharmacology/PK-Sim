using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using static OSPSuite.Core.Domain.Constants.Parameters;
using static PKSim.Core.CoreConstants.Organ;
using static PKSim.Core.CoreConstants.Parameters;
using IFormulaFactory = OSPSuite.Core.Domain.Formulas.IFormulaFactory;

namespace PKSim.Core.Services
{
   public class CKDDiseaseStateImplementation : IDiseaseStateImplementation
   {
      private enum CKDStage
      {
         Stage3,
         Stage4,
         Stage5,
      }

      private readonly IValueOriginRepository _valueOriginRepository;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IIndividualFactory _individualFactory;
      private readonly IContainerTask _containerTask;
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IDimension _dimensionForGFR;
      public static readonly string TARGET_GFR = "eGFR";
      private readonly IDimension _ageDimension;
      private const int CKD_VALUE_ORIGIN_ID = 92;
      private const string GFR_UNIT = "ml/min/1.73m²";

      public CKDDiseaseStateImplementation(
         IValueOriginRepository valueOriginRepository,
         IDimensionRepository dimensionRepository,
         IFormulaFactory formulaFactory,
         IIndividualFactory individualFactory,
         IContainerTask containerTask,
         IParameterSetUpdater parameterSetUpdater)
      {
         _valueOriginRepository = valueOriginRepository;
         _formulaFactory = formulaFactory;
         _individualFactory = individualFactory;
         _containerTask = containerTask;
         _parameterSetUpdater = parameterSetUpdater;
         _dimensionForGFR = dimensionRepository.DimensionForUnit(GFR_UNIT);
         _ageDimension = dimensionRepository.AgeInYears;
      }

      public bool IsSatisfiedBy(DiseaseState diseaseState) => diseaseState.IsNamed(CoreConstants.DiseaseStates.CKD);

      private (
         IParameter hct,
         IParameter GFR_spec,
         IParameter kidneyVolume,
         IParameter kidneySpecificBloodFlowRate,
         IParameter fatVolume,
         IParameter plasmaProteinScaleFactorParameter,
         IParameter smallIntestinalTransitTime,
         IParameter gastricEmptyingTime
         ) parametersChangedByCKDAlgorithm(Individual individual)
      {
         var organism = individual.Organism;
         var kidney = organism.Organ(KIDNEY);
         var fat = organism.Organ(FAT);
         var smallIntestine = organism.Organ(SMALL_INTESTINE);
         var hct = organism.Parameter(HCT);
         var GFR_spec = kidney.Parameter(GFR_SPEC);
         var kidneyVolume = kidney.Parameter(VOLUME);
         var kidneySpecificBloodFlowRate = kidney.Parameter(SPECIFIC_BLOOD_FLOW_RATE);
         var fatVolume = fat.Parameter(VOLUME);
         var stomach = organism.EntityAt<IContainer>(LUMEN, STOMACH);
         var plasmaProteinScaleFactorParameter = organism.Parameter(PLASMA_PROTEIN_SCALE_FACTOR);
         var smallIntestinalTransitTime = smallIntestine.Parameter(SMALL_INTESTINAL_TRANSIT_TIME);
         var gastricEmptyingTime = stomach.Parameter(GASTRIC_EMPTYING_TIME);

         return (hct, GFR_spec, kidneyVolume, kidneySpecificBloodFlowRate, fatVolume, plasmaProteinScaleFactorParameter, smallIntestinalTransitTime, gastricEmptyingTime);
      }

      private IReadOnlyList<IParameter> parametersChangedByCKDAlgorithmAsList(Individual individual)
      {
         var p = parametersChangedByCKDAlgorithm(individual);

         return new[] {p.hct, p.GFR_spec, p.kidneyVolume, p.kidneySpecificBloodFlowRate, p.fatVolume, p.plasmaProteinScaleFactorParameter, p.smallIntestinalTransitTime, p.gastricEmptyingTime};
      }

      public bool ApplyTo(Individual individual) => applyTo(individual, updateParameter);

      public bool ApplyForPopulationTo(Individual individual) => applyTo(individual, updateParameterValues);

      private bool applyTo(Individual individual, Action<IParameter, double> updateParameterFunc)
      {
         var targetGFR = individual.OriginData.DiseaseStateParameters.FindByName(TARGET_GFR);

         var (hct, GFR_Spec, kidneyVolume, kidneySpecificBloodFlowRate, fatVolume, plasmaProteinScaleFactorParameter, smallIntestinalTransitTime, gastricEmptyingTime)
            = parametersChangedByCKDAlgorithm(individual);
         var organism = individual.Organism;
         var bsa = organism.Parameter(BSA);


         var GFR_0 = getTargetGFRValue(GFR_Spec.Value * kidneyVolume.Value / bsa.Value);
         var targetGFRValue = getTargetGFRValue(targetGFR.Value);

         //The GFR of our individual is smaller! This individual should be discarded
         if (GFR_0 < targetGFRValue)
            return false;

         //Adjust kidney volume and update fat accordingly
         var healthyKidneyVolume = kidneyVolume.Value;
         updateParameterFunc(kidneyVolume, getKidneyVolumeFactor(targetGFRValue) / getKidneyVolumeFactor(GFR_0) * healthyKidneyVolume);
         updateParameterFunc(fatVolume, fatVolume.Value + healthyKidneyVolume - kidneyVolume.Value);

         //Adjust renal blood flow spec
         updateParameterFunc(kidneySpecificBloodFlowRate, getRenalBloodFlowFactor(targetGFRValue) / getRenalBloodFlowFactor(GFR_0) * kidneySpecificBloodFlowRate.Value);

         //Correct specific GFR
         updateParameterFunc(GFR_Spec, GFR_Spec.Value * targetGFRValue / GFR_0 * healthyKidneyVolume / kidneyVolume.Value);

         var (plasmaProteinScaleFactor, gastricEmptyingTimeFactor, smallIntestinalTransitTimeFactor) = getCategorialFactors(targetGFRValue);

         //Categorial Parameters as constant: We set the value as is as the value will not be reset when creating a population
         updateParameterFunc(plasmaProteinScaleFactorParameter, plasmaProteinScaleFactor);

         //Categorial Parameters distributed: We apply the variation to the default value
         updateParameterFunc(gastricEmptyingTime, gastricEmptyingTime.Value * gastricEmptyingTimeFactor);
         updateParameterFunc(smallIntestinalTransitTime, smallIntestinalTransitTime.Value * smallIntestinalTransitTimeFactor);

         //Special case for Hematocrit
         updateParameterFunc(hct, hct.Value * getHematocritFactor(targetGFRValue, individual.OriginData.Gender));

         //no parameters to lock for now
         lockParametersAfterCKDImplementation();
         return true;
      }

      private void lockParametersAfterCKDImplementation(params IParameter[] parameters)
      {
         parameters.Each(x => x.Editable = false);
      }

      private void updateParameterValues(IParameter parameter, double value)
      {
         parameter.Value = value;
      }

      private void updateParameter(IParameter parameter, double value)
      {
         updateValueOriginsFor(parameter);
         if (parameter is IDistributedParameter distributedParameter)
         {
            distributedParameter.ScaleDistributionBasedOn(value / distributedParameter.Value);
            return;
         }

         //We are using a formula, we override with a constant
         if (parameter.Formula.IsExplicit())
         {
            parameter.Formula = _formulaFactory.ConstantFormula(value, parameter.Dimension);
            return;
         }

         //constant formula
         updateParameterValues(parameter, value);
         parameter.DefaultValue = value;
         parameter.IsFixedValue = false;
      }

      public Individual CreateBaseIndividualForPopulation(Individual originalIndividual)
      {
         //we need to create a new individual WITHOUT CKD and set all percentiles as in the original individuals. Other parameters, value wil be taken as is
         var originData = originalIndividual.OriginData.Clone();

         //remove the disease state to create a healthy Individual
         originData.DiseaseState = null;
         var healthyIndividual = _individualFactory.CreateAndOptimizeFor(originData, originalIndividual.Seed);

         var allCKDParameters = parametersChangedByCKDAlgorithmAsList(healthyIndividual);

         //Make sure we update the flags that might not be set coming from the database
         allCKDParameters.Each(x => x.IsChangedByCreateIndividual = true);

         //do not update parameters changed by CKD algorithm or that are not visible
         var allHealthyParameters = _containerTask.CacheAllChildrenSatisfying<IParameter>(healthyIndividual, x => !allCKDParameters.Contains(x) && x.Visible);
         var allOriginalParameters = _containerTask.CacheAllChildren<IParameter>(originalIndividual);
         _parameterSetUpdater.UpdateValues(allOriginalParameters, allHealthyParameters);

         //we have a healthy individuals based on the CKD individual where all changes were all manual changes were accounted for
         //we now need to add the disease state contributions from the original individual
         originData.DiseaseState = originalIndividual.OriginData.DiseaseState;
         originalIndividual.OriginData.DiseaseStateParameters.Each(x => originData.AddDiseaseStateParameter(x.Clone()));

         return healthyIndividual;
      }

      public void ResetParametersAfterPopulationIteration(Individual individual)
      {
         //ensures that formula parameters are reset so that they can be reused in next iteration
         var allCKDParameters = parametersChangedByCKDAlgorithmAsList(individual).Where(x => x.IsFixedValue);
         allCKDParameters.Each(x => x.ResetToDefault());
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
         var ageInYears = _ageDimension.BaseUnitValueToUnitValue(_ageDimension.Unit(CoreConstants.Units.Years), originData.Age.Value);
         if (ageInYears >= 18)
            return (true, string.Empty);

         return (false, PKSimConstants.Error.CKDOnlyAvailableForAdult);
      }

      public void ApplyTo(IndividualMolecule individualMolecule)
      {
         //nothing to do here
      }

      private double getHematocritFactor(double targetGFR, Gender gender)
      {
         var isFemale = gender.IsNamed(CoreConstants.Gender.FEMALE);
         var healthyValue = isFemale ? 40 : 45.5;
         double diseaseValue;
         if (targetGFR <= 20)
            diseaseValue = isFemale ? 33.5 : 34.3;
         else if (targetGFR <= 30)
            diseaseValue = isFemale ? 37.4 : 41.7;
         else if (targetGFR <= 40)
            diseaseValue = isFemale ? 38.4 : 42.6;
         else if (targetGFR <= 50)
            diseaseValue = isFemale ? 39.7 : 43.3;
         else
            diseaseValue = isFemale ? 39.6 : 44.8;

         return diseaseValue / healthyValue;
      }

      private void updateValueOriginsFor(IParameter parameter)
      {
         parameter.ValueOrigin.UpdateAllFrom(_valueOriginRepository.FindBy(CKD_VALUE_ORIGIN_ID));
         //Make sure we mark this parameter as changed by create individual. It might already be the case but in that case, it does not change anything
         parameter.IsChangedByCreateIndividual = true;
      }

      private double getKidneyVolumeFactor(double gfrValue) => getFactor(gfrValue, -6.3E-5, 0.0149, 4.13);

      private double getRenalBloodFlowFactor(double gfrValue) => getFactor(gfrValue, -3.1E-5, 0.0170, 4.09);

      private double getFactor(double variable, double a, double b, double c)
      {
         var factor = a * Math.Pow(variable, 2) + b * variable + c;
         return Math.Exp(factor);
      }

      private CKDStage getCKDStage(double targetGFR)
      {
         if (targetGFR < 15)
            return CKDStage.Stage5;
         if (targetGFR < 30)
            return CKDStage.Stage4;

         return CKDStage.Stage3;
      }

      private double getTargetGFRValue(double gfrValueInLPerMinPerDm2)
      {
         return _dimensionForGFR.BaseUnitValueToUnitValue(_dimensionForGFR.Unit(GFR_UNIT), gfrValueInLPerMinPerDm2);
      }

      private (double plasmaProteinScaleFactor, double gastricEmptyingTimeFactor, double smallIntestinalTransitTimeFactor) getCategorialFactors(double gfrValue)
      {
         var stage = getCKDStage(gfrValue);
         switch (stage)
         {
            case CKDStage.Stage3:
               return (1.07, 1.0, 1.0);
            case CKDStage.Stage4:
               return (1.16, 1.6, 1.4);
            case CKDStage.Stage5:
               return (1.55, 1.6, 1.4);
            default:
               throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
         }
      }
   }
}