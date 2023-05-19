using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using static OSPSuite.Core.Domain.Constants.Parameters;
using static PKSim.Core.CoreConstants.Organ;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_HIDiseaseStateImplementation : ContextForIntegration<HIDiseaseStateImplementation>
   {
      protected IDiseaseStateRepository _diseaseStateRepository;
      protected DiseaseState _diseaseStateHI;
      protected IParameter _childPughScore;
      protected OriginDataParameter _originChildPughSCore;

      protected override void Context()
      {
         base.Context();
         _diseaseStateRepository = IoC.Resolve<IDiseaseStateRepository>();
         _diseaseStateHI = _diseaseStateRepository.FindById(CoreConstants.DiseaseStates.HI);
         _childPughScore = _diseaseStateHI.Parameter(CHILD_PUGH_SCORE);
         _originChildPughSCore = new OriginDataParameter
         {
            Name = _childPughScore.Name,
         };
      }
   }

   public abstract class concern_for_HIDiseaseStateImplementationForIndividual : concern_for_HIDiseaseStateImplementation
   {
      protected Individual _individual;

      protected override void Context()
      {
         base.Context();
         _diseaseStateRepository = IoC.Resolve<IDiseaseStateRepository>();
         _diseaseStateHI = _diseaseStateRepository.FindById(CoreConstants.DiseaseStates.HI);
         _childPughScore = _diseaseStateHI.Parameter(CHILD_PUGH_SCORE);
         _originChildPughSCore = new OriginDataParameter
         {
            Name = _childPughScore.Name,
         };

         _individual = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Population.ICRP);


         _individual.OriginData.DiseaseState = _diseaseStateHI;
         _individual.OriginData.AddDiseaseStateParameter(_originChildPughSCore);
         //Create an individual with values coming from the table
         _individual.Organism.Organ(STOMACH).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 38.61;
         _individual.Organism.Organ(SMALL_INTESTINE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 89.77;
         _individual.Organism.Organ(LARGE_INTESTINE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 63.05;
         _individual.Organism.Organ(SPLEEN).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 80.11;
         _individual.Organism.Organ(PANCREAS).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 34.16;
         _individual.Organism.Organ(LIVER).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 17.94;
         _individual.Organism.Organ(LIVER).Parameter(VOLUME).ValueInDisplayUnit = 2.38;
         _individual.Organism.Organ(KIDNEY).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 302.705;
         _individual.Organism.Organ(BONE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 2.75;
         _individual.Organism.Organ(FAT).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 2.18;
         _individual.Organism.Organ(GONADS).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 8.06;
         _individual.Organism.Organ(HEART).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 62.34;
         _individual.Organism.Organ(MUSCLE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 3.42;
         _individual.Organism.Organ(SKIN).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 8.65;
         _individual.Organism.Organ(KIDNEY).Parameter(GFR_SPEC).ValueInDisplayUnit = 20;
         //HCT parameter is a discrete distribution parameter. In order to update the value properly, we need to set the mean value
         _individual.Organism.Parameter(HCT).DowncastTo<IDistributedParameter>().MeanParameter.Value = 0.45;
      }

      protected override void Because()
      {
         //applied at creation
         sut.ApplyTo(_individual);
      }
   }

   public abstract class concern_for_HIDiseaseStateImplementationForExpressionProfile : concern_for_HIDiseaseStateImplementation
   {
      private ExpressionProfile _expressionProfile;
      protected IndividualMolecule _molecule;

      protected override void Context()
      {
         base.Context();
         _diseaseStateRepository = IoC.Resolve<IDiseaseStateRepository>();
         _diseaseStateHI = _diseaseStateRepository.FindById(CoreConstants.DiseaseStates.HI);
         _childPughScore = _diseaseStateHI.Parameter(CHILD_PUGH_SCORE);
         _originChildPughSCore = new OriginDataParameter
         {
            Name = _childPughScore.Name,
         };


         //use an enzyme that is known to the HI algorithm
         _expressionProfile = DomainFactoryForSpecs.CreateExpressionProfile<IndividualEnzyme>(moleculeName: "TOTO");
         _molecule = _expressionProfile.Molecule;
         _molecule.ReferenceConcentration.Value = 5;

         var individual = _expressionProfile.Individual;
         individual.OriginData.DiseaseState = _diseaseStateHI;
         individual.OriginData.AddDiseaseStateParameter(_originChildPughSCore);
      }

      protected override void Because()
      {
         //applied at creation
         sut.ApplyTo(_expressionProfile, "CYP2A6");
      }
   }

   public class When_applying_the_HI_disease_state_algorithm_to_a_child_pugh_A_individual : concern_for_HIDiseaseStateImplementationForIndividual
   {
      protected override void Context()
      {
         base.Context();
         _originChildPughSCore.Value = HIDiseaseStateImplementation.ChildPughScore.A;
      }

      [Observation]
      public void should_return_the_expected_values_for_specific_blood_flows()
      {
         _individual.Organism.Organ(STOMACH).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(15.44, 1e-3);
         _individual.Organism.Organ(SMALL_INTESTINE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(35.91, 1e-3);
         _individual.Organism.Organ(LARGE_INTESTINE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(25.22, 1e-3);
         _individual.Organism.Organ(SPLEEN).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(32.04, 1e-3);
         _individual.Organism.Organ(PANCREAS).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(13.67, 1e-3);
         _individual.Organism.Organ(LIVER).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(28.88, 1e-3);
         _individual.Organism.Organ(KIDNEY).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(266.38, 1e-3);
         //for other blood flow, we have some rounding error due to that we are calculating with 2 digits in the excel file
         _individual.Organism.Organ(BONE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(4.539, 1e-2);
         _individual.Organism.Organ(FAT).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(3.598, 1e-2);
         _individual.Organism.Organ(GONADS).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(13.28, 1e-2);
         _individual.Organism.Organ(HEART).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(102.71, 1e-2);
         _individual.Organism.Organ(MUSCLE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(5.63, 1e-2);
         _individual.Organism.Organ(SKIN).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(14.25, 1e-2);
      }

      [Observation]
      public void should_return_the_expected_values_for_ontogeny_factor_albumin_and_glycoprotein()
      {
         _individual.Organism.Parameter(ONTOGENY_FACTOR_ALBUMIN).ValueInDisplayUnit.ShouldBeEqualTo(0.81, 1e-3);
         _individual.Organism.Parameter(ONTOGENY_FACTOR_AGP).ValueInDisplayUnit.ShouldBeEqualTo(0.6, 1e-3);
      }

      [Observation]
      public void should_return_the_expected_values_for_gfr_spec()
      {
         _individual.Organism.Organ(KIDNEY).Parameter(GFR_SPEC).ValueInDisplayUnit.ShouldBeEqualTo(20 * 1, 1e-3);
      }

      [Observation]
      public void should_return_the_expected_values_for_hct()
      {
         _individual.Organism.Parameter(HCT).ValueInDisplayUnit.ShouldBeEqualTo(0.387, 1e-3);
      }

      [Observation]
      public void should_return_the_expected_values_for_volumes()
      {
         _individual.Organism.Organ(LIVER).Parameter(VOLUME).ValueInDisplayUnit.ShouldBeEqualTo(1.642, 1e-3);
      }
   }

   public class When_applying_the_HI_disease_state_algorithm_to_a_child_pugh_A_expression_profile : concern_for_HIDiseaseStateImplementationForExpressionProfile
   {
      protected override void Context()
      {
         base.Context();
         _originChildPughSCore.Value = HIDiseaseStateImplementation.ChildPughScore.A;
      }

      [Observation]
      public void should_return_the_expected_values_for_reference_concentration()
      {
         _molecule.ReferenceConcentration.Value.ShouldBeEqualTo(5 * 0.89, 1e-3);
      }
   }

   public class When_applying_the_HI_disease_state_algorithm_to_a_child_pugh_B_individual : concern_for_HIDiseaseStateImplementationForIndividual
   {
      protected override void Context()
      {
         base.Context();
         _originChildPughSCore.Value = HIDiseaseStateImplementation.ChildPughScore.B;
      }

      [Observation]
      public void should_return_the_expected_values_for_specific_blood_flows()
      {
         _individual.Organism.Organ(STOMACH).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(13.90, 1e-3);
         _individual.Organism.Organ(SMALL_INTESTINE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(32.32, 1e-3);
         _individual.Organism.Organ(LARGE_INTESTINE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(22.70, 1e-3);
         _individual.Organism.Organ(SPLEEN).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(28.84, 1e-3);
         _individual.Organism.Organ(PANCREAS).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(12.30, 1e-3);
         _individual.Organism.Organ(LIVER).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(41.964, 1e-3);
         _individual.Organism.Organ(KIDNEY).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(196.76, 1e-3);
         //for other blood flow, we have some rounding error due to that we are calculating with 2 digits in the excel file
         _individual.Organism.Organ(BONE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(6.03, 1e-2);
         _individual.Organism.Organ(FAT).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(4.78, 1e-2);
         _individual.Organism.Organ(GONADS).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(17.66, 1e-2);
         _individual.Organism.Organ(HEART).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(136.60, 1e-2);
         _individual.Organism.Organ(MUSCLE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(7.49, 1e-2);
         _individual.Organism.Organ(SKIN).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(18.95, 1e-2);
      }

      [Observation]
      public void should_return_the_expected_values_for_ontogeny_factor_albumin_and_glycoprotein()
      {
         _individual.Organism.Parameter(ONTOGENY_FACTOR_ALBUMIN).ValueInDisplayUnit.ShouldBeEqualTo(0.68, 1e-3);
         _individual.Organism.Parameter(ONTOGENY_FACTOR_AGP).ValueInDisplayUnit.ShouldBeEqualTo(0.56, 1e-3);
      }

      [Observation]
      public void should_return_the_expected_values_for_gfr_spec()
      {
         _individual.Organism.Organ(KIDNEY).Parameter(GFR_SPEC).ValueInDisplayUnit.ShouldBeEqualTo(20 * 0.7, 1e-3);
      }

      [Observation]
      public void should_return_the_expected_values_for_hct()
      {
         _individual.Organism.Parameter(HCT).ValueInDisplayUnit.ShouldBeEqualTo(0.37, 1e-3);
      }

      [Observation]
      public void should_return_the_expected_values_for_volumes()
      {
         _individual.Organism.Organ(LIVER).Parameter(VOLUME).ValueInDisplayUnit.ShouldBeEqualTo(1.31, 1e-3);
      }
   }

   public class When_applying_the_HI_disease_state_algorithm_to_a_child_pugh_B_expression_profile : concern_for_HIDiseaseStateImplementationForExpressionProfile
   {
      protected override void Context()
      {
         base.Context();
         _originChildPughSCore.Value = HIDiseaseStateImplementation.ChildPughScore.B;
      }

      [Observation]
      public void should_return_the_expected_values_for_reference_concentration()
      {
         _molecule.ReferenceConcentration.Value.ShouldBeEqualTo(5 * 0.62, 1e-3);
      }
   }

   public class When_applying_the_HI_disease_state_algorithm_to_a_child_pugh_C_individual : concern_for_HIDiseaseStateImplementationForIndividual
   {
      protected override void Context()
      {
         base.Context();
         _originChildPughSCore.Value = HIDiseaseStateImplementation.ChildPughScore.C;
      }

      [Observation]
      public void should_return_the_expected_values_for_specific_blood_flows()
      {
         _individual.Organism.Organ(STOMACH).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(1.544, 1e-3);
         _individual.Organism.Organ(SMALL_INTESTINE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(3.590, 1e-3);
         _individual.Organism.Organ(LARGE_INTESTINE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(2.522, 1e-3);
         _individual.Organism.Organ(SPLEEN).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(3.204, 1e-3);
         _individual.Organism.Organ(PANCREAS).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(1.367, 1e-3);
         _individual.Organism.Organ(LIVER).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(94.933, 1e-3);
         _individual.Organism.Organ(KIDNEY).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(145.30, 1e-3);
         //for other blood flow, we have some rounding error due to that we are calculating with 2 digits in the excel file
         _individual.Organism.Organ(BONE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(7.26, 1e-2);
         _individual.Organism.Organ(FAT).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(5.76, 1e-2);
         _individual.Organism.Organ(GONADS).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(21.29, 1e-2);
         _individual.Organism.Organ(HEART).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(164.66, 1e-2);
         _individual.Organism.Organ(MUSCLE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(9.02, 1e-2);
         _individual.Organism.Organ(SKIN).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(22.85, 1e-2);
      }

      [Observation]
      public void should_return_the_expected_values_for_ontogeny_factor_albumin_and_glycoprotein()
      {
         _individual.Organism.Parameter(ONTOGENY_FACTOR_ALBUMIN).ValueInDisplayUnit.ShouldBeEqualTo(0.5, 1e-2);
         _individual.Organism.Parameter(ONTOGENY_FACTOR_AGP).ValueInDisplayUnit.ShouldBeEqualTo(0.3, 1e-3);
      }

      [Observation]
      public void should_return_the_expected_values_for_gfr_spec()
      {
         _individual.Organism.Organ(KIDNEY).Parameter(GFR_SPEC).ValueInDisplayUnit.ShouldBeEqualTo(20 * 0.36, 1e-3);
      }

      [Observation]
      public void should_return_the_expected_values_for_hct()
      {
         _individual.Organism.Parameter(HCT).ValueInDisplayUnit.ShouldBeEqualTo(0.35, 1e-3);
      }

      [Observation]
      public void should_return_the_expected_values_for_volumes()
      {
         _individual.Organism.Organ(LIVER).Parameter(VOLUME).ValueInDisplayUnit.ShouldBeEqualTo(0.666, 1e-3);
      }
   }

   public class When_applying_the_HI_disease_state_algorithm_to_a_child_pugh_C_expression_profile : concern_for_HIDiseaseStateImplementationForExpressionProfile
   {
      protected override void Context()
      {
         base.Context();
         _originChildPughSCore.Value = HIDiseaseStateImplementation.ChildPughScore.C;
      }

      [Observation]
      public void should_return_the_expected_values_for_reference_concentration()
      {
         _molecule.ReferenceConcentration.Value.ShouldBeEqualTo(5 * 0.32, 1e-3);
      }
   }

   public class When_validating_an_origin_data_for_HI_implementation : concern_for_HIDiseaseStateImplementation
   {
      private OriginData _originData;

      protected override void Context()
      {
         base.Context();
         var individual = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Population.ICRP);
         _originData = individual.OriginData;
         _originData.DiseaseState = _diseaseStateHI;
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