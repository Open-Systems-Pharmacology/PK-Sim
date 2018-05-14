using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_Species : ContextSpecification<Species>
   {
      protected SpeciesPopulation _speciesPopulation1;
      protected SpeciesPopulation _speciesPopulation2;

      protected override void Context()
      {
         _speciesPopulation1 = new SpeciesPopulation {Name = "POP1"};
         _speciesPopulation2 = new SpeciesPopulation {Name = "POP2"};
         sut = new Species();
         sut.AddPopulation(_speciesPopulation1);
         sut.AddPopulation(_speciesPopulation2);
      }
   }

   public class When_returing_the_default_population : concern_for_Species
   {
      [Observation]
      public void should_return_the_first_population_if_any_is_available()
      {
         sut.DefaultPopulation.ShouldBeEqualTo(_speciesPopulation1);
      }

      [Observation]
      public void should_return_null_if_no_population_was_defined_for_the_species()
      {
         sut = new Species();
         sut.DefaultPopulation.ShouldBeNull();
      }
   }
}