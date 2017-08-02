using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_2
{
   public abstract class concern_for_TwoPoreConverter : ContextWithLoadedProject<Converter514To521>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("2Pore_515");
      }
   }

   public class When_converting_the_two_pore_project : concern_for_TwoPoreConverter
   {
      private Simulation _simulation;
      private Individual _individual;
      private Compound _compound;

      protected override void Context()
      {
         _simulation = First<Simulation>();
         _individual = First<Individual>();
         _compound = First<Compound>();
      }

      [Observation]
      public void should_have_added_the_missing_parameters_into_the_individuals()
      {
         validate_individual_parameters_added(_individual);
         validate_individual_parameters_added(_simulation.BuildingBlock<Individual>());
      }

      [Observation]
      public void should_have_added_the_missing_parameters_into_the_compound()
      {
         validate_compound_parameters_added(_compound);
         validate_compound_parameters_added(_simulation.BuildingBlock<Compound>());
      }

      [Observation]
      public void should_have_remove_the_container_EndosomalClearance_from_all_organs_and_container_except_EndogenousIgG()
      {
         validate_container_removed_in(_individual);
         validate_container_removed_in(_simulation.BuildingBlock<Individual>());
      }

      [Observation]
      public void should_have_updated_the_neighborhoods_end_to_ecl()
      {
         validate_neighborhood(_individual);
         validate_neighborhood(_simulation.BuildingBlock<Individual>());
      }


      private void validate_neighborhood(Individual individual)
      {
         var endosomalClearance = individual.Organism.Container(ConverterConstants.ContainerName.ENDOSOMAL_CLEARANCE);
         individual.Neighborhoods.GetSingleChildByName<INeighborhood>("Spleen_end_Spleen_ecl").SecondNeighbor.ShouldBeEqualTo(endosomalClearance);
      }

      private void validate_container_removed_in(Individual individual)
      {
         individual.Organism.Organ(CoreConstants.Organ.Bone).Compartment(ConverterConstants.ContainerName.ENDOSOMAL_CLEARANCE).ShouldBeNull();
         individual.Organism.Organ(CoreConstants.Organ.SmallIntestine)
            .Compartment(CoreConstants.Compartment.Mucosa).Container(ConverterConstants.ContainerName.ENDOSOMAL_CLEARANCE).ShouldBeNull();

         individual.Organism.Organ(CoreConstants.Organ.EndogenousIgG).Compartment(ConverterConstants.ContainerName.ENDOSOMAL_CLEARANCE).ShouldNotBeNull();

         individual.Organism.Container(ConverterConstants.ContainerName.ENDOSOMAL_CLEARANCE).ShouldNotBeNull();
      }
      private void validate_compound_parameters_added(Compound compound)
      {
         compound.ContainsName(ConverterConstants.Parameter.Kass_FcRn).ShouldBeTrue();
         compound.ContainsName(ConverterConstants.Parameter.Kd_FcRn_pls_int).ShouldBeTrue();
      }

      private void validate_individual_parameters_added(Individual individual)
      {
         var endogenouIgG = individual.Organism.Organ(CoreConstants.Organ.EndogenousIgG);
         endogenouIgG.ContainsName(ConverterConstants.Parameter.Kass_FcRn_ligandEndo).ShouldBeTrue();
         endogenouIgG.ContainsName(ConverterConstants.Parameter.Kd_FcRn_LigandEndo).ShouldBeTrue();
         endogenouIgG.ContainsName(ConverterConstants.Parameter.Kd_FcRn_ligandEndo_pls_int).ShouldBeTrue();
         endogenouIgG.Container(CoreConstants.Compartment.Endosome).ContainsName(ConverterConstants.Parameter.Start_concentration_FcRn_endosome).ShouldBeTrue();
         endogenouIgG.Container(CoreConstants.Compartment.Plasma).ContainsName(ConverterConstants.Parameter.Start_concentration_endogenous_plasma).ShouldBeTrue();
      }
   }
}