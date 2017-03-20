using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_NonCompetitiveInhibitionKineticUpdaterSpecification : ContextSpecification<NonCompetitiveInhibitionKineticUpdaterSpecification>
   {
      private IDimensionRepository _dimensionRepository;
      private ObjectPathFactoryForSpecs _objectPathFactory;
      protected Simulation _simulation;
      private InhibitionProcess _competitiveInhibition;
      private InhibitionProcess _nonCompetitiveInhibition1;
      private InhibitionProcess _nonCompetitiveInhibition2;
      private Compound _compound1;
      private Compound _compound2;
      private InteractionProperties _interactionProperties;
      protected string _moleculeName = "ENZYME";
      protected string _drugName = "DRUG";
      private InteractionTask _interactionTask;

      protected override void Context()
      {
         _simulation = A.Fake<Simulation>();
         _competitiveInhibition = new InhibitionProcess { InteractionType = InteractionType.CompetitiveInhibition }.WithName("CompetitiveInhibition");
         _nonCompetitiveInhibition1 = new InhibitionProcess { InteractionType = InteractionType.NonCompetitiveInhibition }.WithName("NonCompetitiveInhibition1");
         _nonCompetitiveInhibition2 = new InhibitionProcess { InteractionType = InteractionType.NonCompetitiveInhibition }.WithName("NonCompetitiveInhibition2");
         _compound1 = new Compound().WithName("Compound1");
         _compound1.AddProcess(_competitiveInhibition);
         _interactionTask = new InteractionTask();

         _compound2 = new Compound().WithName("Compound2");
         _compound2.AddProcess(_nonCompetitiveInhibition1);
         _compound2.AddProcess(_nonCompetitiveInhibition2);

         _interactionProperties = new InteractionProperties();
         A.CallTo(() => _simulation.InteractionProperties).Returns(_interactionProperties);
         A.CallTo(() => _simulation.Compounds).Returns(new[] { _compound1, _compound2 });

         _objectPathFactory = new ObjectPathFactoryForSpecs();
         _dimensionRepository = A.Fake<IDimensionRepository>();


         _interactionProperties.AddInteraction(new InteractionSelection { MoleculeName = _moleculeName, ProcessName = _competitiveInhibition.Name, CompoundName = _compound1.Name });
         _interactionProperties.AddInteraction(new InteractionSelection { MoleculeName = _moleculeName, ProcessName = _nonCompetitiveInhibition1.Name, CompoundName = _compound2.Name });
         _interactionProperties.AddInteraction(new InteractionSelection { MoleculeName = _moleculeName, ProcessName = _nonCompetitiveInhibition2.Name, CompoundName = _compound2.Name });


         _dimensionRepository = A.Fake<IDimensionRepository>();
         _objectPathFactory = new ObjectPathFactoryForSpecs();
         sut = new NonCompetitiveInhibitionKineticUpdaterSpecification(_objectPathFactory, _dimensionRepository, _interactionTask   );
      }
   }

   public class When_retrieving_the_km_numerator_factor_for_a_non_competitive_inhibition : concern_for_NonCompetitiveInhibitionKineticUpdaterSpecification
   {
      [Observation]
      public void should_only_return_the_part_of_the_interaction_for_non_competitive_inhibition()
      {
         sut.KmNumeratorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water1*In1/KnI1 + K_water2*In2/KnI2");
      }
   }

   public class When_retrieving_the_km_denominator_factor_for_a_non_competitive_inhibition : concern_for_NonCompetitiveInhibitionKineticUpdaterSpecification
   {
      [Observation]
      public void should_only_return_the_part_of_the_interaction_for_non_competitive_inhibition()
      {
         sut.KmDenominatorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water1*In1/KnI1 + K_water2*In2/KnI2");
      }
   }

   public class When_retrieving_the_vmax_denominator_factor_for_a_non_competitive_inhibition : concern_for_NonCompetitiveInhibitionKineticUpdaterSpecification
   {
      [Observation]
      public void should_only_return_the_part_of_the_interaction_for_non_competitive_inhibition()
      {
         sut.KcatDenominatorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water1*In1/KnI1 + K_water2*In2/KnI2");
      }
   }


   public class When_retrieving_the_cl_denominator_factor_for_a_non_competitive_inhibition : concern_for_NonCompetitiveInhibitionKineticUpdaterSpecification
   {
      [Observation]
      public void should_only_return_the_part_of_the_interaction_for_non_competitive_inhibition()
      {
         sut.CLSpecDenominatorTerm(_moleculeName, _drugName, _simulation).ShouldBeEqualTo("K_water1*In1/KnI1 + K_water2*In2/KnI2");
      }
   }
}	