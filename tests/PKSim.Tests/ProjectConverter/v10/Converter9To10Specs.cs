using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.SubSystems.Conversion;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v10;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v10
{
   public class When_converting_the_9_1_P1_project_to_10 : ContextWithLoadedProject<Converter9To10>
   {
      private List<Simulation> _allSimulations;
      private Simulation _simulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("9.1_P1");
         _allSimulations = All<Simulation>().ToList();
         _allSimulations.Each(Load);
         _simulation = _allSimulations.First();
      }

      [Observation]
      public void should_make_all_normalized_parameter_readonly()
      {
         var allRelExpNorms = _simulation.Model.Root.GetAllChildren<IParameter>(x => x.IsNamed(ConverterConstants.Parameters.REL_EXP_NORM));
         allRelExpNorms.Any().ShouldBeTrue();
         allRelExpNorms.Each(x=>x.Visible.ShouldBeFalse());
         allRelExpNorms.Each(x => x.Editable.ShouldBeFalse());
      }
   }



   public class When_converting_the_simple_project_730_project_to_10 : ContextWithLoadedProject<Converter9To10>
   {
      private List<PopulationSimulation> _allSimulations;
      private List<Population> _allPopulations;
      private List<Individual> _allIndividuals;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimplePop_73");
         _allSimulations = All<PopulationSimulation>().ToList();
         _allPopulations = All<Population>().ToList();
         _allIndividuals = All<Individual>().ToList();
         _allSimulations.Each(Load);
         _allPopulations.Each(Load);
         _allIndividuals.Each(Load);
      }

      [Observation]
      public void should_have_converted_the_individual_enzyme_and_protein_to_use_the_new_localization_concept()
      {
         verifyIndividuals(_allSimulations.Select(x => x.BuildingBlock<Individual>()));
         verifyIndividuals(_allIndividuals);
         verifyIndividuals(_allPopulations.Select(x => x.FirstIndividual));

         var ind = _allIndividuals.FindByName("Human");
         var cyp3A4 = ind.MoleculeByName<IndividualEnzyme>("CYP3A4");
         var allExpressionParameters = ind.AllExpressionParametersFor(cyp3A4);
         allExpressionParameters["Bone"].Value.ShouldBeEqualTo(0.04749, 1e-2);
         allExpressionParameters["Duodenum"].Value.ShouldBeEqualTo(0.3999, 1e-2);
      }

      private void verifyIndividuals(IEnumerable<Individual> individuals) => individuals.Each(verifyIndividual);

      private void verifyIndividual(Individual individual)
      {
         individual.AllMolecules<IndividualProtein>().Each(m =>
         {
            m.Localization.ShouldNotBeEqualTo(Localization.None);

            //Should have created parameters in most compartments. About 30+
            var allExpressionParameters = individual.AllExpressionParametersFor(m);
            allExpressionParameters.Count.ShouldBeGreaterThan(30);
         });
      }
   }

   public class When_converting_the_v9_individual_project_to_10 : ContextWithLoadedProject<Converter9To10>
   {
      private Individual _individual;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("V9_Individual");
         _individual = FindByName<Individual>("I1");
      }

      [Observation]
      public void should_have_converted_the_transporter_as_expected()
      {
         var influx = _individual.MoleculeByName<IndividualTransporter>("T1_Influx");
         influx.TransportType.ShouldBeEqualTo(TransportType.Influx);

         //checking that it is not set to the default Efflux and that a conversion occurred
         var allTransporterContainers = _individual.AllMoleculeContainersFor<TransporterExpressionContainer>(influx);
         allTransporterContainers.Any(x => x.TransportDirection.ToString().Contains("Efflux")).ShouldBeFalse();

         var efflux_apical_bbb = _individual.MoleculeByName<IndividualTransporter>("T4_Efflux_Apical_BBB");
         efflux_apical_bbb.TransportType.ShouldBeEqualTo(TransportType.Efflux);
      }

      [Observation]
      public void should_have_converted_the_enzyme_as_expected()
      {
         var enzyme = _individual.MoleculeByName<IndividualEnzyme>("E1_Intracellular_Endosomal");
         enzyme.Localization.Is(Localization.Intracellular).ShouldBeTrue();
         enzyme.Localization.Is(Localization.VascEndosome).ShouldBeTrue();
         enzyme.Localization.Is(Localization.BloodCellsIntracellular).ShouldBeTrue();
      }

      [Observation]
      public void should_have_normalized_the_expression()
      {
         var enzyme = _individual.MoleculeByName<IndividualEnzyme>("E1_Intracellular_Endosomal");
         var allExpressionParameters = _individual.AllExpressionParametersFor(enzyme);
         allExpressionParameters[CoreConstants.Compartment.VASCULAR_ENDOTHELIUM].Value.ShouldBeEqualTo(1);
         allExpressionParameters[CoreConstants.Compartment.PLASMA].Value.ShouldBeEqualTo(2 / 3.0);
         allExpressionParameters[CoreConstants.Compartment.BLOOD_CELLS].Value.ShouldBeEqualTo(1 / 3.0);
      }

      [Observation]
      public void should_have_updated_the_ontogeny()
      {
         var enzyme = _individual.MoleculeByName<IndividualEnzyme>("E1_Intracellular_Endosomal");
         enzyme.Ontogeny.Name.ShouldBeEqualTo("CYP2C18");
      }

      [Observation]
      public void should_have_updated_the_fraction_expressed_intracellular_based_on_the_localization()
      {
         verifyFractionExpressedIntracellular("E1_Intracellular_Endosomal", 1);
         verifyFractionExpressedIntracellular("E2_Intracellular_Interstitial", 1);
         verifyFractionExpressedIntracellular("E3_Extracellular_Apical", 0);
         verifyFractionExpressedIntracellular("E4_Extracellular_Basolateral", 0);
         verifyFractionExpressedIntracellular("E5_Interstitial", 0);
      }

      [Observation]
      public void should_have_updated_the_fraction_expressed_in_blood_cells_based_on_the_localization()
      {
         verifyFractionExpressedBloodCells("E1_Intracellular_Endosomal", 1);
         verifyFractionExpressedBloodCells("E2_Intracellular_Interstitial", 1);
         verifyFractionExpressedBloodCells("E3_Extracellular_Apical", 0);
         verifyFractionExpressedBloodCells("E4_Extracellular_Basolateral", 0);
         verifyFractionExpressedBloodCells("E5_Interstitial", 1);
      }

      [Observation]
      public void should_have_updated_the_fraction_expressed_in_endosomes_based_on_the_localization()
      {
         verifyFractionExpressedEndosomes("E1_Intracellular_Endosomal", 1);
         verifyFractionExpressedEndosomes("E2_Intracellular_Interstitial", 0);
         verifyFractionExpressedEndosomes("E3_Extracellular_Apical", 0);
         verifyFractionExpressedEndosomes("E4_Extracellular_Basolateral", 0);
         verifyFractionExpressedEndosomes("E5_Interstitial", 0);
      }

      private void verifyFractionExpressedIntracellular(string moleculeName, double value)
      {
         var enzyme = _individual.MoleculeByName<IndividualEnzyme>(moleculeName);
         var allEnzymeContainers = _individual.AllMoleculeContainersFor<MoleculeExpressionContainer>(enzyme);
         var boneIntracellular = allEnzymeContainers.Find(x => x.LogicalContainerName == CoreConstants.Organ.BONE && x.CompartmentName == CoreConstants.Compartment.INTRACELLULAR);
         boneIntracellular.Parameter(CoreConstants.Parameters.FRACTION_EXPRESSED_INTRACELLULAR).Value.ShouldBeEqualTo(value);
      }

      private void verifyFractionExpressedBloodCells(string moleculeName, double value)
      {
         var enzyme = _individual.MoleculeByName<IndividualEnzyme>(moleculeName);
         enzyme.Parameter(CoreConstants.Parameters.FRACTION_EXPRESSED_BLOOD_CELLS).Value.ShouldBeEqualTo(value);
      }

      private void verifyFractionExpressedEndosomes(string moleculeName, double value)
      {
         var enzyme = _individual.MoleculeByName<IndividualEnzyme>(moleculeName);
         enzyme.Parameter(CoreConstants.Parameters.FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME).Value.ShouldBeEqualTo(value);
      }
   }
}