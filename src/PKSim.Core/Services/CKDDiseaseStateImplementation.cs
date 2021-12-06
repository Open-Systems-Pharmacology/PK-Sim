using System;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using static OSPSuite.Core.Domain.Constants.Parameters;
using static PKSim.Core.CoreConstants.Organ;
using static PKSim.Core.CoreConstants.Parameters;

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
      private readonly IDimensionRepository _dimensionRepository;
      private const string TARGET_GFR = "TargetGFR";
      private const int CKD_VALUE_ORIGIN_ID = 92;

      public CKDDiseaseStateImplementation(
         IValueOriginRepository valueOriginRepository,
         IDimensionRepository dimensionRepository)
      {
         _valueOriginRepository = valueOriginRepository;
         _dimensionRepository = dimensionRepository;
      }

      public bool IsSatisfiedBy(DiseaseState diseaseState) => diseaseState.IsNamed(CoreConstants.DiseaseStates.CKD);

      public void ApplyTo(Individual individual)
      {
         var targetGFR = individual.OriginData.DiseaseStateParameters.FindByName(TARGET_GFR);

         var organism = individual.Organism;
         var kidney = organism.Organ(KIDNEY);
         var fat = organism.Organ(FAT);
         var smallIntestine = organism.Organ(SMALL_INTESTINE);
         var bsa = organism.Parameter(BSA);
         var hct = organism.Parameter(HCT);
         var GFR_spec = kidney.Parameter(GFR_SPEC);
         var kidneyVolume = kidney.Parameter(VOLUME);
         var kidneySpecificBloodFlowRate = kidney.Parameter(SPECIFIC_BLOOD_FLOW_RATE);
         var fatVolume = fat.Parameter(VOLUME);
         var stomach = organism.EntityAt<IContainer>(LUMEN, STOMACH);
         var plasmaProteinScaleFactorParameter = organism.Parameter(PLASMA_PROTEIN_SCALE_FACTOR);
         var smallIntestinalTransitTime = smallIntestine.Parameter(SMALL_INTESTINAL_TRANSIT_TIME);
         var gastricEmptyingTime = stomach.Parameter(GASTRIC_EMPTYING_TIME);

         var bsaInm2 = bsa.ConvertToUnit(bsa.Value, CoreConstants.Units.m2);
         var GFR_0 = GFR_spec.Value * kidneyVolume.Value * 1.73 / bsaInm2;
         if (GFR_0 < targetGFR.Value)
            throw new OSPSuiteException("ERROR");

         //Adjust kidney volume and update fat accordingly
         var healthyKidneyVolume = kidneyVolume.Value;
         kidneyVolume.Value = getKidneyVolumeFactor(targetGFR.Value) / getKidneyVolumeFactor(GFR_0) * healthyKidneyVolume;
         fatVolume.Value = fatVolume.Value + healthyKidneyVolume - kidneyVolume.Value;

         //Adjust rena blood flow
         kidneySpecificBloodFlowRate.Value = getRenalBloodFlowFactor(targetGFR.Value) / getRenalBloodFlowFactor(GFR_0) * kidneySpecificBloodFlowRate.Value;

         //Correct specific GFR
         GFR_spec.Value = GFR_spec.Value * targetGFR.Value / GFR_0 * healthyKidneyVolume / kidneyVolume.Value;

         var ckdStage = getCKDStage(targetGFR);
         var (plasmaProteinScaleFactor, gastricEmptyingTimeFactor, smallIntestinalTransitTimeFactor) = getCategorialFactors(ckdStage);

         //Categorial Parameters
         plasmaProteinScaleFactorParameter.Value = plasmaProteinScaleFactor;
         gastricEmptyingTime.Value *= gastricEmptyingTimeFactor;
         smallIntestinalTransitTime.Value *= smallIntestinalTransitTimeFactor;

         //Special case for Hematocrit
         hct.Value *= getHematocritFactor(targetGFR, individual.OriginData.Gender);

         //Finally update all value origin of modified parameters
         updateValueOriginsFor(fatVolume, kidneyVolume, kidneySpecificBloodFlowRate, GFR_spec, plasmaProteinScaleFactorParameter, smallIntestinalTransitTime, gastricEmptyingTime, hct);
      }

      private double getHematocritFactor(OriginDataParameter targetGFR, Gender gender)
      {
         var isFemale = gender.IsNamed(CoreConstants.Gender.FEMALE);
         var healthyValue = isFemale ? 40 : 45.5;
         double diseaseValue;
         var value = getTargetGFRValue(targetGFR);
         if (value <= 20)
            diseaseValue = isFemale ? 33.5 : 34.3;
         else if (value <= 30)
            diseaseValue = isFemale ? 37.4 : 41.7;
         else if (value <= 40)
            diseaseValue = isFemale ? 38.4 : 42.6;
         else if (value <= 50)
            diseaseValue = isFemale ? 39.7 : 43.3;
         else
            diseaseValue = isFemale ? 39.6 : 44.8;

         return diseaseValue / healthyValue;
      }

      private void updateValueOriginsFor(params IParameter[] parameters)
      {
         parameters.Each(x => x.ValueOrigin.UpdateAllFrom(_valueOriginRepository.FindBy(CKD_VALUE_ORIGIN_ID)));
      }

      private double getKidneyVolumeFactor(double gfrValue) => getFactor(gfrValue, 6.3 * 10E-5, 0.0149, 4.13);

      private double getRenalBloodFlowFactor(double gfrValue) => getFactor(gfrValue, -3.1 * 10E-5, 0.0170, 4.09);

      private double getFactor(double variable, double a, double b, double c)
      {
         var factor = a * Math.Pow(variable, 2) + b * variable + c;
         return Math.Exp(factor);
      }

      private CKDStage getCKDStage(OriginDataParameter targetGFR)
      {
         var value = getTargetGFRValue(targetGFR);
         if (value < 15)
            return CKDStage.Stage5;
         if (value < 30)
            return CKDStage.Stage4;

         return CKDStage.Stage3;
      }

      private double getTargetGFRValue(OriginDataParameter targetGFR)
      {
         var dimension = _dimensionRepository.DimensionForUnit(targetGFR.Unit);
         return dimension.BaseUnitValueToUnitValue(dimension.Unit(targetGFR.Unit), targetGFR.Value);
      }

      private (double plasmaProteinScaleFactor, double gastricEmptyingTimeFactor, double smallIntestinalTransitTimeFactor) getCategorialFactors(CKDStage stage)
      {
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