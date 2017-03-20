using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure;
using PKSim.Infrastructure.ProjectConverter;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_Individual : ContextForIntegration<Individual>
   {
      protected override void Context()
      {
         sut = DomainFactoryForSpecs.CreateStandardIndividual();
      }
   }

   public class When_creating_a_default_individual : concern_for_Individual
   {
      [Observation]
      public void the_resulting_GFR_spec_value_should_be_comparable_with_the_value_defined_in_the_literature()
      {
         sut.Organism.Organ(CoreConstants.Organ.Kidney)
            .Parameter(ConverterConstants.Parameter.GFRspec).Value.ShouldBeEqualTo(0.266, 1e-2);
      }
   }

   public class When_creating_rabbit : concern_for_Individual
   {
      protected override void Context()
      {
         sut = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Species.Rabbit);
      }

      [Observation]
      public void individual_species_should_be_rabbit()
      {
         sut.Population.Species.ShouldBeEqualTo(CoreConstants.Species.Rabbit);
      }
   }
}