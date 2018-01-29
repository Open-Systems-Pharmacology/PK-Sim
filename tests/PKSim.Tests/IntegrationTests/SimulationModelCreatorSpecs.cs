using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.IntegrationTests
{
   public class When_a_model_is_being_created_for_a_simulation : ContextForSimulationIntegration<ISimulationModelCreator>
   {
      private IndividualEnzyme _enzyme;
      private IndividualEnzyme _protein;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var enzymeFactory = IoC.Resolve<IIndividualEnzymeFactory>();
         var templateIndividual = DomainFactoryForSpecs.CreateStandardIndividual();
         var compound = DomainFactoryForSpecs.CreateStandardCompound();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();

         _enzyme = enzymeFactory.CreateFor(templateIndividual).DowncastTo<IndividualEnzyme>().WithName("CYP");
         templateIndividual.AddMolecule(_enzyme);

         _protein = enzymeFactory.CreateFor(templateIndividual).DowncastTo<IndividualEnzyme>().WithName("PROT");
         templateIndividual.AddMolecule(_protein);

         _protein.TissueLocation = TissueLocation.Intracellular;
         _protein.IntracellularVascularEndoLocation = IntracellularVascularEndoLocation.Interstitial;

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(templateIndividual, compound, protocol).DowncastTo<IndividualSimulation>();

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_create_a_build_configuration_based_on_the_building_blocks_and_mapping_selected_in_the_simulation()
      {
         _simulation.Model.ShouldNotBeNull();
      }

      [Observation]
      public void should_update_the_reaction_building_block_in_the_created_simulation()
      {
         _simulation.Reactions.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_updated_the_simulation_id_in_all_parameters_defined_in_the_simulation()
      {
         var allTemplateParameters = _simulation.ParametersOfType(PKSimBuildingBlockType.Template);
         allTemplateParameters.Each(p => p.Origin.SimulationId.ShouldBeEqualTo(_simulation.Id));
      }

      [Observation]
      public void should_have_set_the_molecule_amount_as_not_persistable_except_the_amount_in_urine_feces_and_gall_bladder()
      {
         var allMoleculeAmounts = _simulation.All<IMoleculeAmount>();
         foreach (var moleculeAmount in allMoleculeAmounts)
         {
            if (moleculeAmount.HasAncestorNamed(CoreConstants.Compartment.URINE))
               continue;

            if (moleculeAmount.HasAncestorNamed(CoreConstants.Compartment.FECES))
               continue;

            if (moleculeAmount.HasAncestorNamed(CoreConstants.Organ.Gallbladder))
               continue;

            moleculeAmount.Persistable.ShouldBeFalse();
         }
      }

      [Observation]
      public void the_start_amount_of_all_protein_should_point_the_the_start_amount_parameter()
      {
         var cypMuscleCell = _simulation.Model.Root.EntityAt<MoleculeAmount>(
            Constants.ORGANISM, CoreConstants.Organ.Muscle, CoreConstants.Compartment.Intracellular, _enzyme.Name);

         var explicitFormula = cypMuscleCell.Formula.DowncastTo<ExplicitFormula>();
         var path = explicitFormula.FormulaUsablePathBy("M_0");
         path.ShouldNotBeNull();
         path.Last().ShouldBeEqualTo(CoreConstants.Parameters.START_AMOUNT);
      }

      [Observation]
      public void should_hide_the_relative_expression_parameters_defined_in_interstitial_for_protein_defined_with_a_tissue_localization_in_cell_and_a_vascular_endothelium_localization_in_interstitial()
      {
         var protBoneInterstitial = _simulation.Model.Root.EntityAt<MoleculeAmount>(
            Constants.ORGANISM, CoreConstants.Organ.Bone, CoreConstants.Compartment.Interstitial, _protein.Name);

         protBoneInterstitial.Parameter(CoreConstants.Parameters.REL_EXP).Visible.ShouldBeFalse();
         protBoneInterstitial.Parameter(CoreConstants.Parameters.REL_EXP_NORM).Visible.ShouldBeFalse();
      }
   }
}