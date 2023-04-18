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

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_HIDiseaseStateImplementation : ContextForIntegration<HIDiseaseStateImplementation>
   {
      protected IDiseaseStateRepository _diseaseStateRepository;
      protected DiseaseState _diseaseStateHI;
      protected IParameter _childPughScore;
      protected Individual _individual;
      protected OriginDataParameter _originChildPughSCore;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _diseaseStateRepository = IoC.Resolve<IDiseaseStateRepository>();
         _diseaseStateHI = _diseaseStateRepository.FindById(CoreConstants.DiseaseStates.HI);
         _childPughScore = _diseaseStateHI.Parameter(HIDiseaseStateImplementation.CHILD_PUGH_SCORE);
         _originChildPughSCore = new OriginDataParameter
         {
            Name = _childPughScore.Name,
         };

         _individual = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Population.ICRP);
         _individual.OriginData.DiseaseState = _diseaseStateHI;
         _individual.OriginData.AddDiseaseStateParameter(_originChildPughSCore);
         //Create an individual with values coming from the table
         _individual.Organism.Organ(STOMACH).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 38.76;
         _individual.Organism.Organ(SMALL_INTESTINE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 90.12;
         _individual.Organism.Organ(LARGE_INTESTINE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 63.30;
         _individual.Organism.Organ(SPLEEN).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 91.12;
         _individual.Organism.Organ(PANCREAS).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 34.89;
         _individual.Organism.Organ(LIVER).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 18.76;
         _individual.Organism.Organ(LIVER).Parameter(VOLUME).ValueInDisplayUnit = 2.35;
         _individual.Organism.Organ(KIDNEY).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 259.93;
         _individual.Organism.Organ(BONE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 2.75;
         _individual.Organism.Organ(FAT).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 2.18;
         _individual.Organism.Organ(GONADS).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 8.06;
         _individual.Organism.Organ(HEART).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 62.87;
         _individual.Organism.Organ(MUSCLE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 3.42;
         _individual.Organism.Organ(SKIN).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit = 8.65;
      }
   }

   public class When_applying_the_HI_disease_state_algorithm_to_a_child_pugh_A_individual : concern_for_HIDiseaseStateImplementation
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _originChildPughSCore.Value = HIDiseaseStateImplementation.ChildPughScore.A;
      }

      protected override void Because()
      {
         sut.ApplyTo(_individual);
      }

      [Observation]
      public void should_return_the_expected_values_for_specific_blood_flows()
      {
         _individual.Organism.Organ(STOMACH).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(15.48, 1e-2);
         _individual.Organism.Organ(SMALL_INTESTINE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(35.99, 1e-2);
         _individual.Organism.Organ(LARGE_INTESTINE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(25.28, 1e-2);
         _individual.Organism.Organ(SPLEEN).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(27.55, 1e-2);
         _individual.Organism.Organ(PANCREAS).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(14.20, 1e-2);
         _individual.Organism.Organ(LIVER).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(24.98, 1e-2);
         _individual.Organism.Organ(KIDNEY).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(209.84, 1e-2);
         _individual.Organism.Organ(BONE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(4.36, 1e-2);
         _individual.Organism.Organ(FAT).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(3.46, 1e-2);
         _individual.Organism.Organ(GONADS).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(12.77, 1e-2);
         _individual.Organism.Organ(HEART).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(100.31, 1e-2);
         _individual.Organism.Organ(MUSCLE).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(5.42, 1e-2);
         _individual.Organism.Organ(SKIN).Parameter(SPECIFIC_BLOOD_FLOW_RATE).ValueInDisplayUnit.ShouldBeEqualTo(13.70, 1e-2);
      }

      [Observation]
      public void should_return_the_expected_values_for_volumes()
      {
         _individual.Organism.Organ(LIVER).Parameter(VOLUME).ValueInDisplayUnit.ShouldBeEqualTo(1.60);
      }
   }
}