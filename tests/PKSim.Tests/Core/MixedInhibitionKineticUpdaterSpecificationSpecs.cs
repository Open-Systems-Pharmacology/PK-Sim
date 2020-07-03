using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_MixedInhibitionKineticUpdaterSpecification : ContextSpecification<MixedInhibitionKineticUpdaterSpecification>
   {
      private InhibitionProcess _mixedInhibition1;
      private InhibitionProcess _mixedInhibition2;
      protected Simulation _simulation;
      private Compound _compound1;
      private Compound _compound2;
      private InteractionProperties _interactionProperties;
      protected string _moleculeName = "ENZYME";
      protected string _drugName = "DRUG";

      protected override void Context()
      {
         _simulation = A.Fake<Simulation>();
         _mixedInhibition1 = new InhibitionProcess { InteractionType = InteractionType.MixedInhibition }.WithName("MixedInhibition1");
         _mixedInhibition2 = new InhibitionProcess { InteractionType = InteractionType.MixedInhibition }.WithName("MixedInhibition2");
         _compound1 = new Compound().WithName("Compound1");

         _compound2 = new Compound().WithName("Compound2");
         _compound2.AddProcess(_mixedInhibition1);
         _compound2.AddProcess(_mixedInhibition2);

         _interactionProperties = new InteractionProperties();
         A.CallTo(() => _simulation.InteractionProperties).Returns(_interactionProperties);
         A.CallTo(() => _simulation.Compounds).Returns(new[] { _compound1, _compound2 });
         
         sut = new MixedInhibitionKineticUpdaterSpecification(new ObjectPathFactoryForSpecs(), A.Fake<IDimensionRepository>(), new InteractionTask());

         _interactionProperties.AddInteraction(new InteractionSelection { MoleculeName = _moleculeName, ProcessName = _mixedInhibition1.Name, CompoundName = _compound2.Name });
         _interactionProperties.AddInteraction(new InteractionSelection { MoleculeName = _moleculeName, ProcessName = _mixedInhibition2.Name, CompoundName = _compound2.Name });
      }
   }

   public class When_retrieving_the_km_numerator_factor_for_a_mixed_inhibition : concern_for_MixedInhibitionKineticUpdaterSpecification
   {
      [Observation]
      public void should_only_return_the_part_of_the_interaction_for_mixed_inhibition()
      {
         sut.KmNumeratorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water_m1*Im1/KmcI1 + K_water_m2*Im2/KmcI2");
      }

   }

   public class When_retrieving_the_km_denominator_factor_for_a_mixed_inhibition : concern_for_MixedInhibitionKineticUpdaterSpecification
   {
      [Observation]
      public void should_only_return_the_part_of_the_interaction_for_mixed_inhibition()
      {
         sut.KmDenominatorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water_m1*Im1/KmuI1 + K_water_m2*Im2/KmuI2");
      }
   }

   public class When_retrieving_the_vmax_denominator_factor_for_a_mixed_inhibition : concern_for_MixedInhibitionKineticUpdaterSpecification
   {
      [Observation]
      public void should_only_return_the_part_of_the_interaction_for_mixed_inhibition()
      {
         sut.KcatDenominatorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water_m1*Im1/KmuI1 + K_water_m2*Im2/KmuI2");
      }
   }

   public class When_retrieving_the_cl_denominator_factor_for_a_mixed_inhibition : concern_for_MixedInhibitionKineticUpdaterSpecification
   {
      [Observation]
      public void should_only_return_the_part_of_the_interaction_for_mixed_inhibition()
      {
         sut.CLSpecDenominatorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water_m1*Im1/KmcI1 + K_water_m2*Im2/KmcI2");
      }
   }
}	