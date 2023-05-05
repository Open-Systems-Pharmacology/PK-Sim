using System;
using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using static OSPSuite.Core.Domain.Constants.Parameters;
using static PKSim.Core.CoreConstants.Organ;
using static PKSim.Core.CoreConstants.Parameters;
using IFormulaFactory = OSPSuite.Core.Domain.Formulas.IFormulaFactory;

namespace PKSim.Core.Services
{
   public class CKDDiseaseStateImplementation : AbstractDiseaseStateImplementation
   {
      private enum CKDStage
      {
         Stage3,
         Stage4,
         Stage5,
      }

      private readonly IDimension _dimensionForGFR;
      public const string TARGET_GFR = "eGFR";
      private readonly IDimension _ageDimension;
      private const int CKD_VALUE_ORIGIN_ID = 92;
      public const string GFR_UNIT = "ml/min/1.73m²";

      public CKDDiseaseStateImplementation(
         IDimensionRepository dimensionRepository,
         IValueOriginRepository valueOriginRepository,
         IFormulaFactory formulaFactory,
         IIndividualFactory individualFactory,
         IContainerTask containerTask,
         IParameterSetUpdater parameterSetUpdater) : base(valueOriginRepository, formulaFactory, individualFactory, containerTask, parameterSetUpdater, CoreConstants.DiseaseStates.CKD)
      {
         _dimensionForGFR = dimensionRepository.DimensionForUnit(GFR_UNIT);
         _ageDimension = dimensionRepository.AgeInYears;
      }

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

      public override bool ApplyTo(Individual individual) => applyTo(individual, UpdateParameter(CKD_VALUE_ORIGIN_ID));

      public override bool ApplyForPopulationTo(Individual individual) => applyTo(individual, UpdateParameterValue);

      private bool applyTo(Individual individual, Action<ParameterUpdate> updateParameterFunc)
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
         updateParameterFunc(new(kidneyVolume, getKidneyVolumeFactor(targetGFRValue) / getKidneyVolumeFactor(GFR_0)));
         updateParameterFunc(new(fatVolume, fatVolume.Value + healthyKidneyVolume - kidneyVolume.Value, ParameterUpdateMode.Value));

         //Adjust renal blood flow spec
         updateParameterFunc(new(kidneySpecificBloodFlowRate, getRenalBloodFlowFactor(targetGFRValue) / getRenalBloodFlowFactor(GFR_0)));

         //Correct specific GFR
         updateParameterFunc(new(GFR_Spec, targetGFRValue / GFR_0 * healthyKidneyVolume / kidneyVolume.Value));

         var (plasmaProteinScaleFactor, gastricEmptyingTimeFactor, smallIntestinalTransitTimeFactor) = getCategorialFactors(targetGFRValue);

         //Categorial Parameters as constant: We set the value as is as the value will not be reset when creating a population
         updateParameterFunc(new(plasmaProteinScaleFactorParameter, plasmaProteinScaleFactor, ParameterUpdateMode.Value));

         //Categorial Parameters distributed: We apply the variation to the default value
         updateParameterFunc(new(gastricEmptyingTime, gastricEmptyingTimeFactor));
         updateParameterFunc(new(smallIntestinalTransitTime, smallIntestinalTransitTimeFactor));

         //Special case for Hematocrit
         updateParameterFunc(new(hct, getHematocritFactor(targetGFRValue, individual.OriginData.Gender)));

         return true;
      }

      public override (bool isValid, string error) IsValid(OriginData originData)
      {
         var ageInYears = _ageDimension.BaseUnitValueToUnitValue(_ageDimension.Unit(CoreConstants.Units.Years), originData.Age.Value);
         if (ageInYears >= 18)
            return (true, string.Empty);

         return (false, PKSimConstants.Error.CKDOnlyAvailableForAdult);
      }

      public override void ApplyTo(Individual individual, IndividualMolecule individualMolecule)
      {
         //nothing to do here
      }

      protected override IReadOnlyList<IParameter> ParameterChangedByDiseaseStateAsList(Individual individual)
      {
         var (hct, gfrSpec, kidneyVolume, kidneySpecificBloodFlowRate, fatVolume, plasmaProteinScaleFactorParameter, smallIntestinalTransitTime, gastricEmptyingTime) = parametersChangedByCKDAlgorithm(individual);
         return new[] {hct, gfrSpec, kidneyVolume, kidneySpecificBloodFlowRate, fatVolume, plasmaProteinScaleFactorParameter, smallIntestinalTransitTime, gastricEmptyingTime};
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