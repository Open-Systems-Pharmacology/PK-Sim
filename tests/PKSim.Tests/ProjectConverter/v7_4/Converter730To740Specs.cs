using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v7_4;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v7_4
{
   public class When_converting_the_simple_project_730_project : ContextWithLoadedProject<Converter730To740>
   {
      private List<Simulation> _allSimulations;
      private List<Individual> _allIndividuals;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimpleProject_730");
         _allSimulations = All<Simulation>().ToList();
         _allIndividuals = All<Individual>().ToList();
         _allSimulations.Each(Load);
         _allIndividuals.Each(Load);
      }

      [Observation]
      public void should_be_able_to_load_the_project()
      {
         _project.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_set_the_parameter_tablet_time_delay_factor_to_variable_and_not_readonly_in_the_oral_simulations()
      {
         var oralSimulation = _allSimulations.FindByName("S1");
         var allParameters = oralSimulation.Model.Root.GetAllChildren<IParameter>(x => x.IsNamed(ConverterConstants.Parameters.TabletTimeDelayFactor)).ToList();
         allParameters.Count.ShouldBeGreaterThan(0);
         allParameters.Each(p =>
         {
            p.Visible.ShouldBeTrue();
            p.Editable.ShouldBeTrue();
            p.Sequence.ShouldBeGreaterThan(10);
         });
      }

      [Observation]
      public void should_have_updated_the_ontogeny_factor_parameters_in_all_individual_to_be_visible_editable_and_default_value_set_to_current_value()
      {
         _allIndividuals.Each(verifyIndividual);
         _allSimulations.Select(s => s.BuildingBlock<Individual>()).Each(verifyIndividual);
      }


      [Observation]
      public void should_have_updated_the_plasma_protein_ontogeny_factor_in_the_simulations()
      {
         _allSimulations.Each(s => verifyPlasmaProteinOntogenyFactor(s.Model.Root.Container(Constants.ORGANISM)));
         _allSimulations.Select(s => s.BuildingBlock<Individual>()).Each(verifyIndividual);
      }

      private void verifyIndividual(Individual individual)
      {
         verifyPlasmaProteinOntogenyFactor(individual.Organism);
         individual.AllDefinedMolecules().Each(verifyOntogenyInMolecule);
      }

      private void verifyPlasmaProteinOntogenyFactor(IContainer container)
      {
         verifyOntogenyParameter(container.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR_ALBUMIN));
         verifyOntogenyParameter(container.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR_AGP));
      }

      private void verifyOntogenyInMolecule(IndividualMolecule molecule)
      {
         verifyOntogenyParameter(molecule.OntogenyFactorParameter);
         verifyOntogenyParameter(molecule.OntogenyFactorGIParameter);
      }

      private void verifyOntogenyParameter(IParameter parameter)
      {
         parameter.Visible.ShouldBeTrue();
         parameter.Editable.ShouldBeTrue();
         parameter.DefaultValue.ShouldBeEqualTo(parameter.Value);
      }
   }
}