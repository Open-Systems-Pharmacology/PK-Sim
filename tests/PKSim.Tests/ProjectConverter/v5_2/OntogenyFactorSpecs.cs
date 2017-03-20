using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v5_2
{
   public class When_converting_the_Ontogeny_project : ContextWithLoadedProject<Converter514To521>
   {
      private Simulation _simulation;
      private Individual _individual;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Ontogeny");
      }

      protected override void Context()
      {
         _individual = First<Individual>();
         _simulation = First<Simulation>();
      }

      [Observation]
      public void should_have_converted_the_ontogeny_factor()
      {
         validateIndividual(_individual);
         validateIndividual(_simulation.BuildingBlock<Individual>());
      }

      private void validateIndividual(Individual individual)
      {
         var molecule = individual.AllMolecules().First();
         molecule.Ontogeny.ShouldNotBeNull();
         molecule.OntogenyFactorParameter.ShouldNotBeNull();
         molecule.OntogenyFactorGIParameter.ShouldNotBeNull();
      }
   }
}