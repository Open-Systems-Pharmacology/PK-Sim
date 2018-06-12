using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_6;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Resources;
using Constants = OSPSuite.Core.Domain.Constants;

namespace PKSim.ProjectConverter.v5_6
{
   public class When_converting_the_Individual_553_project : ContextWithLoadedProject<Converter552To561>
   {
      private Individual _individual;
      private Population _population;
      private Simulation _simulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Individual_553");
         _individual = First<Individual>();
         _population = First<Population>();
         _simulation = First<Simulation>();
      }

      [Observation]
      public void should_have_added_the_new_liver_structure_to_the_individual()
      {
         validateLiverStructureIn(_individual);
      }

      [Observation]
      public void should_have_added_the_new_liver_structure_to_the_individual_of_the_population()
      {
         validateLiverStructureIn(_population.FirstIndividual);
      }

      [Observation]
      public void should_have_added_the_new_liver_structure_to_the_individual_of_the_simulation()
      {
         validateLiverStructureIn(_simulation.Individual);
      }

      private void validateLiverStructureIn(Individual indiviual)
      {
         var liver = liverIn(indiviual);
         liver.Container(CoreConstants.Compartment.Pericentral).ShouldNotBeNull();
         liver.Container(CoreConstants.Compartment.Periportal).ShouldNotBeNull();
      }

      [Observation]
      public void should_have_added_the_IsZonatedLiver_flag_to_the_individual()
      {
         var parameter = liverIn(_individual).Parameter(CoreConstants.Parameters.IS_LIVER_ZONATED);
         parameter.Value.ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_have_kept_the_surface_area_values_defined_in_the_liver()
      {
         var SA_int_cell = _individual.Neighborhoods.Container(ConverterConstants.Neighborhoods.LiverIntToLiverCell)
            .Parameter(ConverterConstants.Parameter.SA_int_cell);
         SA_int_cell.IsFixedValue.ShouldBeTrue();
         SA_int_cell.Value.ShouldBeEqualTo(10000);
      }

      [Observation]
      public void should_have_updated_the_reference_to_first_and_second_neighbor_in_the_neighborhood()
      {
         var liverIntToLiverCell = _individual.Neighborhoods.GetSingleChildByName<INeighborhood>(ConverterConstants.Neighborhoods.LiverIntToLiverCell);
         var liver = liverIn(_individual);
         liverIntToLiverCell.FirstNeighbor.ShouldBeEqualTo(liver.Container(CoreConstants.Compartment.Interstitial));
         liverIntToLiverCell.SecondNeighbor.ShouldBeEqualTo(liver.Container(CoreConstants.Compartment.Intracellular));
      }

      [Observation]
      public void should_have_kept_the_volume_values_defined_in_the_liver()
      {
         var liver = liverIn(_individual);
         liver.Parameter(Constants.Parameters.VOLUME).Value.ShouldBeEqualTo(5);
      }

      [Observation]
      public void should_have_converted_the_AUC_iv_values_for_the_simulation()
      {
         var individualSimulation = FindByName<IndividualSimulation>("Oral");
         var compoundName = First<Compound>().Name;
         individualSimulation.AucIVFor(compoundName).HasValue.ShouldBeTrue();
      }

      [Observation]
      public void should_have_added_the_new_neighborhoods()
      {
         _individual.Neighborhoods.GetSingleChildByName<INeighborhood>(ConverterConstants.Neighborhoods.Periportal_int_Periportal_cell).ShouldNotBeNull();
      }

      [Observation]
      public void should_have_removed_the_old_neighborhoods()
      {
         _individual.Neighborhoods.GetSingleChildByName<INeighborhood>("ArterialBlood_pls_Liver_pls").ShouldBeNull();
      }

      [Observation]
      public void should_have_updated_the_metabolizing_enzyme_expression_in_periportal_and_pericentral()
      {
         var enzyme = _individual.MoleculeByName<IndividualEnzyme>("CYP");
         validateZoneValues(enzyme);
      }

      [Observation]
      public void should_have_removed_the_expression_for_liver()
      {
         var enzyme = _individual.MoleculeByName<IndividualEnzyme>("CYP");
         enzyme.ExpressionContainer(CoreConstants.Organ.Liver).ShouldBeNull();
      }

      [Observation]
      public void should_have_updated_the_transporter_expression_in_periportal_and_pericentral()
      {
         var transBaso = _individual.MoleculeByName<IndividualTransporter>("TRANS_BASO");
         validateZoneValues(transBaso);
         validateTransporterMembrane(transBaso, MembraneLocation.Basolateral);

         var transApical = _individual.MoleculeByName<IndividualTransporter>("TRANS_APICAL");
         validateZoneValues(transApical);
         validateTransporterMembrane(transApical, MembraneLocation.Apical);
      }

      [Observation]
      public void should_have_updated_the_protein_binding_expression_in_periportal_and_pericentral()
      {
         var protein = _individual.MoleculeByName<IndividualOtherProtein>("BIND");
         validateZoneValues(protein);
      }

      [Observation]
      public void should_have_set_the_ontogeny_factor_as_visible()
      {
         foreach (var plasmaOntogenyFactoryName in CoreConstants.Parameters.AllPlasmaProteinOntogenyFactors)
         {
            _individual.Organism.Parameter(plasmaOntogenyFactoryName).Visible.ShouldBeTrue();
         }

         foreach (var molecule in _individual.AllMolecules())
         {
            molecule.OntogenyFactorParameter.Visible.ShouldBeTrue();
            molecule.OntogenyFactorGIParameter.Visible.ShouldBeTrue();
         }
      }

      [Observation]
      public void should_have_updated_the_group_of_the_ontogeny_factor_parametrs()
      {
         foreach (var molecule in _individual.AllMolecules())
         {
            molecule.OntogenyFactorParameter.GroupName.ShouldBeEqualTo(CoreConstants.Groups.ONTOGENY_FACTOR);
            molecule.OntogenyFactorGIParameter.GroupName.ShouldBeEqualTo(CoreConstants.Groups.ONTOGENY_FACTOR);
         }
      }

      [Observation]
      public void should_have_converted_the_summary_chart()
      {
         _project.AllSimulationComparisons.Count.ShouldBeEqualTo(1);
         var comparison = _project.AllSimulationComparisons.ElementAt(0);
         _lazyLoadTask.Load(comparison);
         comparison.IsLoaded.ShouldBeTrue();
      }

      private static void validateZoneValues(IndividualMolecule molecule)
      {
         CoreConstants.Compartment.LiverZones.Each(z =>
         {
            var expressionContainer = molecule.ExpressionContainer(z);
            expressionContainer.OrganPath.PathAsString.ShouldBeEqualTo(new[] {Constants.ORGANISM, CoreConstants.Organ.Liver, z}.ToPathString());
            expressionContainer.RelativeExpression.ShouldBeEqualTo(10);
            expressionContainer.RelativeExpressionNorm.ShouldBeEqualTo(1);
            expressionContainer.ContainerName = z;
         });
      }

      private static void validateTransporterMembrane(IndividualTransporter transporter, MembraneLocation membraneLocation)
      {
         CoreConstants.Compartment.LiverZones.Each(z => transporter.AllExpressionsContainers().FindByName(z).MembraneLocation.ShouldBeEqualTo(membraneLocation));
      }

      private IContainer liverIn(Individual individual)
      {
         return individual.Organism.Organ(CoreConstants.Organ.Liver);
      }
   }
}