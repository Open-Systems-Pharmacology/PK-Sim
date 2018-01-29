using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_2
{
   public abstract class concern_for_P504_Transporter : ContextWithLoadedProject<Converter514To521>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("P504_Transporter");
      }
   }

   public class When_converting_the_project_P504_Transporter : concern_for_P504_Transporter
   {
      private Simulation _simulation;
      private Individual _individual;

      protected override void Context()
      {
         _simulation = First<Simulation>();
         _individual = First<Individual>();
      }

      [Observation]
      public void should_have_converted_the_process_names_in_the_individual()
      {
         assertTransporterHaveTheRightTransportName(_individual);
      }

      [Observation]
      public void should_have_changed_the_container_type_of_all_individual_molecule()
      {
         foreach (var molecule in _individual.AllMolecules())
         {
            molecule.ContainerType.ShouldBeEqualTo(ContainerType.Molecule);
         }
      }

      [Observation]
      public void should_have_added_the_default_ontogenie_parameters()
      {
         assertOntogenieParametersAreAvailable(_individual);
         assertOntogenieParametersAreAvailable(_simulation.BuildingBlock<Individual>());
      }

      [Observation]
      public void should_have_converted_the_process_names_for_the_transporter_in_the_simulation_as_well()
      {
         assertTransporterHaveTheRightTransportName(_simulation.BuildingBlock<Individual>());
      }

      [Observation]
      public void should_have_multiply_the_value_of_A_to_V_bc_by_10_in_the_individual_and_in_the_simulation()
      {
         _individual.Organism.Parameter(ConverterConstants.Parameter.A_to_V_bc).Value.ShouldBeEqualTo(167000);
         _simulation.BuildingBlock<Individual>().Organism.Parameter(ConverterConstants.Parameter.A_to_V_bc).Value.ShouldBeEqualTo(167000);
         _simulation.Model.Root.Container(Constants.ORGANISM).Parameter(ConverterConstants.Parameter.A_to_V_bc).Value.ShouldBeEqualTo(167000);
      }

      [Observation]
      public void should_have_devided_the_value_of_k_SA_by_100_in_the_individual_and_in_the_simulation()
      {
         _individual.Organism.Parameter(ConverterConstants.Parameter.PARAM_k_SA).Value.ShouldBeEqualTo(9500);
         _simulation.BuildingBlock<Individual>().Organism.Parameter(ConverterConstants.Parameter.PARAM_k_SA).Value.ShouldBeEqualTo(9500);
         _simulation.Model.Root.Container(Constants.ORGANISM).Parameter(ConverterConstants.Parameter.PARAM_k_SA).Value.ShouldBeEqualTo(9500);
      }

      private void assertOntogenieParametersAreAvailable(Individual individual)
      {
         individual.Organism.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR_ALBUMIN).Value.ShouldBeEqualTo(1);
         individual.Organism.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR_AGP).Value.ShouldBeEqualTo(1);
         individual.Organism.Parameter(CoreConstants.Parameters.PLASMA_PROTEIN_SCALE_FACTOR).Value.ShouldBeEqualTo(1);
      }

      private void assertTransporterHaveTheRightTransportName(Individual individual)
      {
         foreach (var transporter in individual.AllMolecules<IndividualTransporter>())
         {
            transporter.AllInducedProcesses().Each(p => p.Contains("_MM").ShouldBeFalse());
         }
      }

      private void assertOriginDataConverted(Individual individual)
      {
         individual.OriginData.GestationalAge.ShouldBeEqualTo(CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS);
         individual.OriginData.GestationalAgeUnit.ShouldBeEqualTo(CoreConstants.Units.Weeks);
      }

      [Observation]
      public void should_have_updated_the_origin_data_to_include_a_gestational_age()
      {
         assertOriginDataConverted(_individual);
         assertOriginDataConverted(_simulation.BuildingBlock<Individual>());
      }

      [Observation]
      public void should_have_converted_the_value_of_stomach_parameters()
      { 
         _individual.Organism.Organ(CoreConstants.Organ.Lumen).Compartment(CoreConstants.Organ.Stomach)
            .Parameter("Distal radius").ValueInDisplayUnit.ShouldBeEqualTo(5);

      }

      [Observation]
      public void should_have_converted_the_BMI_according_to_its_original_value()
      {
         var bmi = _individual.Organism.Parameter(CoreConstants.Parameters.BMI).ValueInDisplayUnit;
         bmi.ShouldBeSmallerThan(30);
      }


      [Observation]
      public void should_have_added_the_new_calculation_method_for_mucosa_volume()
      {
         var calculationMethod = _individual.OriginData.AllCalculationMethods().FindByName("MucosaVolume_Human");
         calculationMethod.ShouldNotBeNull();
      }
   }
}