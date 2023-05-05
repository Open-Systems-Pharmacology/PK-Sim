using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using static OSPSuite.Core.Domain.Constants.Parameters;
using static PKSim.Core.CoreConstants.Organ;
using static PKSim.Core.CoreConstants.Parameters;
using IContainer = OSPSuite.Core.Domain.IContainer;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_CKDDiseaseStateImplementation : ContextForIntegration<CKDDiseaseStateImplementation>
   {
      protected IDiseaseStateRepository _diseaseStateRepository;
      protected DiseaseState _diseaseStateCKD;
      protected IParameter _targetGFR;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _diseaseStateRepository = IoC.Resolve<IDiseaseStateRepository>();
         _diseaseStateCKD = _diseaseStateRepository.FindById(CoreConstants.DiseaseStates.CKD);
         _targetGFR = _diseaseStateCKD.Parameter(CKDDiseaseStateImplementation.TARGET_GFR);
      }
   }

   public class When_applying_the_CKD_disease_state_algorithm_to_a_stage_4_individual : concern_for_CKDDiseaseStateImplementation
   {
      private Individual _individual;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Population.ICRP);

         //verify that the individual as the expected values
         _individual.Age.ShouldBeEqualTo(30);
         _individual.InputHeight.ShouldBeEqualTo(17.6);
         _individual.InputWeight.ShouldBeEqualTo(73);
         _individual.InputWeight.ShouldBeEqualTo(73);
         _individual.Organism.Parameter(PLASMA_PROTEIN_SCALE_FACTOR).IsChangedByCreateIndividual.ShouldBeFalse();
         _individual.Organism.Parameter(BMI).ValueInDisplayUnit.ShouldBeEqualTo(23.57, 1e-2);
         _individual.Organism.Organ(FAT).Parameter(VOLUME).ValueInDisplayUnit.ShouldBeEqualTo(14.8680, 1e-2);
         _individual.Organism.Organ(KIDNEY).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(302.705, 1e-2);
         _individual.OriginData.DiseaseState = _diseaseStateCKD;
         _individual.OriginData.AddDiseaseStateParameter(new OriginDataParameter
         {
            Name = _targetGFR.Name,
            Value = _targetGFR.ConvertToBaseUnit(22, _targetGFR.DisplayUnitName()),
            Unit = _targetGFR.DisplayUnitName(),
         });
      }

      protected override void Because()
      {
         sut.ApplyTo(_individual);
      }

      [Observation]
      public void should_return_the_expected_values()
      {
         var organism = _individual.Organism;
         var kidney = organism.Organ(KIDNEY);
         var fat = organism.Organ(FAT);
         var smallIntestine = organism.Organ(SMALL_INTESTINE);
         var hct = organism.Parameter(HCT);
         var kidneyVolume = kidney.Parameter(VOLUME);
         var kidneySpecificBloodFlowRate = kidney.Parameter(SPECIFIC_BLOOD_FLOW_RATE);
         var fatVolume = fat.Parameter(VOLUME);
         var stomach = organism.EntityAt<IContainer>(LUMEN, STOMACH);
         var plasmaProteinScaleFactorParameter = organism.Parameter(PLASMA_PROTEIN_SCALE_FACTOR);
         var smallIntestinalTransitTime = smallIntestine.Parameter(SMALL_INTESTINAL_TRANSIT_TIME);
         var gastricEmptyingTime = stomach.Parameter(GASTRIC_EMPTYING_TIME);

         hct.Value.ShouldBeEqualTo(0.4307472527, 1e-2);
         plasmaProteinScaleFactorParameter.Value.ShouldBeEqualTo(1.16, 1e-2);
         gastricEmptyingTime.Value.ShouldBeEqualTo(24, 1e-2);
         smallIntestinalTransitTime.Value.ShouldBeEqualTo(176.4, 1e-2);
         kidneyVolume.ConvertToUnit(kidneyVolume.Value, "ml").ShouldBeEqualTo(246.637955, 1e-2);
         fatVolume.ConvertToUnit(fatVolume.Value, "ml").ShouldBeEqualTo(15059.86086, 1e-2);
         kidneySpecificBloodFlowRate.ConvertToUnit(kidneySpecificBloodFlowRate.Value, "ml/min/100g organ").ShouldBeEqualTo(100.446757, 1e-2);
      }

      [Observation]
      public void should_set_some_parameters_as_changed_by_created_individuals()
      {
         _individual.Organism.Parameter(PLASMA_PROTEIN_SCALE_FACTOR).IsChangedByCreateIndividual.ShouldBeTrue();
      }
   }

   public class When_validating_an_origin_data : concern_for_CKDDiseaseStateImplementation
   {
      private OriginData _originData;

      protected override void Context()
      {
         base.Context();
         var individual = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Population.ICRP);
         _originData = individual.OriginData;
         _originData.DiseaseState = _diseaseStateCKD;
      }

      [Observation]
      public void should_return_valid_for_an_adult_age()
      {
         _originData.Age.Value = 30;
         sut.IsValid(_originData).isValid.ShouldBeTrue();
      }

      [Observation]
      public void should_return_invalid_for_a_child_age()
      {
         _originData.Age.Value = 14;
         sut.IsValid(_originData).isValid.ShouldBeFalse();
      }
   }
}