using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v6_1;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v6_1
{
   public class When_converting_the_Population_Individual_603_project : ContextWithLoadedProject<Converter602To612>
   {
      private RandomPopulation _population;
      private Individual _individual;
      private IndividualSimulation _indSimulation;
      private PopulationSimulation _popSimulation;
      private Formulation _formulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Population_Individual_603");

         _population = First<RandomPopulation>();
         _individual = First<Individual>();
         _indSimulation = First<IndividualSimulation>();
         _popSimulation = First<PopulationSimulation>();
         _formulation = First<Formulation>();
      }

      [Observation]
      public void should_have_converted_the_protein_expression_container_in_molecule_expression_container()
      {
         _individual.AllMolecules().Count().ShouldBeEqualTo(1);
         _population.AllMolecules().Count().ShouldBeEqualTo(1);
         _indSimulation.Individual.AllMolecules().Count().ShouldBeEqualTo(1);
         _popSimulation.Population.AllMolecules().Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_set_the_negative_allowed_flag_to_true_in_all_molecule_amounts_defined_in_the_simulations()
      {
         validateNegativeValueAllowedFlags(_indSimulation);
         validateNegativeValueAllowedFlags(_popSimulation);
      }

      [Observation]
      public void should_have_set_the_value_of_parameter_use_as_suspenssion_to_zero()
      {
         _formulation.Parameter(CoreConstants.Parameters.USE_AS_SUSPENSION).Value.ShouldBeEqualTo(0);
      }

      private void validateNegativeValueAllowedFlags(Simulation simulation)
      {
         simulation.Model.Root.GetAllChildren<IMoleculeAmount>().Each(x => x.NegativeValuesAllowed.ShouldBeTrue());
      }
   }

   public class When_converting_603_project_with_particles_formulation : ContextWithLoadedProject<Converter602To612>
   {
      private Formulation _formulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("6.0.3_ParticlesFormulation");

         _formulation = First<Formulation>();
      }

      [Observation]
      public void should_not_add_use_as_suspension_parameter()
      {
         _formulation.FormulationType.ShouldBeEqualTo(CoreConstants.Formulation.Particles);
         _formulation.Parameter(CoreConstants.Parameter.USE_AS_SUSPENSION).ShouldBeNull();
      }
   }
}