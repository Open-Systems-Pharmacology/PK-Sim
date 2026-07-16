using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public class When_a_model_is_being_created_for_a_simulation : ContextForSimulationIntegration<ISimulationModelCreator>
   {
      private IndividualMolecule _enzyme;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var templateIndividual = DomainFactoryForSpecs.CreateStandardIndividual();
         var compound = DomainFactoryForSpecs.CreateStandardCompound();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();

         var cypExpression = DomainFactoryForSpecs.CreateExpressionProfileAndAddToIndividual<IndividualEnzyme>(templateIndividual, "CYP");
         _enzyme = cypExpression.Molecule;

         var protExpression = DomainFactoryForSpecs.CreateExpressionProfileAndAddToIndividual<IndividualEnzyme>(templateIndividual, "PROT");

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
         var allMoleculeAmounts = _simulation.All<MoleculeAmount>();
         foreach (var moleculeAmount in allMoleculeAmounts)
         {
            if (moleculeAmount.HasAncestorNamed(CoreConstants.Compartment.URINE))
               continue;

            if (moleculeAmount.HasAncestorNamed(CoreConstants.Compartment.FECES))
               continue;

            if (moleculeAmount.HasAncestorNamed(CoreConstants.Organ.GALLBLADDER))
               continue;

            moleculeAmount.Persistable.ShouldBeFalse();
         }
      }

      [Observation]
      public void the_start_amount_of_all_protein_should_point_the_the_start_amount_parameter()
      {
         var cypMuscleCell = _simulation.Model.Root.EntityAt<MoleculeAmount>(
            Constants.ORGANISM, CoreConstants.Organ.MUSCLE, CoreConstants.Compartment.INTRACELLULAR, _enzyme.Name);

         var explicitFormula = cypMuscleCell.Formula.DowncastTo<ExplicitFormula>();
         var path = explicitFormula.FormulaUsablePathBy("M_0");
         path.ShouldNotBeNull();
         path.Last().ShouldBeEqualTo(CoreConstants.Parameters.START_AMOUNT);
      }
   }

   public class When_creating_a_simulation_model_for_a_compound_with_a_selected_overwrite_parameter_set : ContextForSimulationIntegration<ISimulationModelCreator>
   {
      private Compound _compound;
      private string _parameterPath;
      private double _overwriteValue;
      private IParameter _appliedParameter;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var templateIndividual = DomainFactoryForSpecs.CreateStandardIndividual();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(templateIndividual, _compound, protocol).DowncastTo<IndividualSimulation>();
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);

         //pick a real compound-dependent simulation parameter to overwrite
         var containerTask = IoC.Resolve<IContainerTask>();
         var compoundParameter = firstValidCompoundParameter(containerTask);

         _parameterPath = compoundParameter.Key;
         _overwriteValue = compoundParameter.Value.Value + 1.0;

         var overwriteParameterSet = new OverwriteParameterSet { Name = "TestSet" };
         overwriteParameterSet.Add(new ParameterValue { Path = _parameterPath.ToObjectPath(), Value = _overwriteValue });
         _simulation.AddOverwriteParameterSetSelection(_compound.Name, overwriteParameterSet);
      }

      private KeyValuePair<string, IParameter> firstValidCompoundParameter(IContainerTask containerTask)
      {
         return containerTask.CacheAllChildren<IParameter>(_simulation.Model.Root).KeyValues
            .First(kv => kv.Value.BuildingBlockType == PKSimBuildingBlockType.Simulation &&
                         _simulation.CompoundNameForParameterPath(kv.Key) == _compound.Name &&
                         !double.IsNaN(kv.Value.Value));
      }

      protected override void Because()
      {
         //rebuild the model so the selected overwrite parameter set is applied during construction
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
         _appliedParameter = IoC.Resolve<IContainerTask>().CacheAllChildren<IParameter>(_simulation.Model.Root)[_parameterPath];
      }

      [Observation]
      public void should_apply_the_overwrite_parameter_set_value_to_the_matching_simulation_parameter()
      {
         _appliedParameter.Value.ShouldBeEqualTo(_overwriteValue);
      }

      [Observation]
      public void should_mark_the_applied_parameter_as_a_compound_parameter()
      {
         _appliedParameter.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Compound);
      }
   }

   public class When_creating_a_simulation_model_with_an_unresolvable_overwrite_parameter_set_path : ContextForSimulationIntegration<ISimulationModelCreator>
   {
      private Compound _compound;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var templateIndividual = DomainFactoryForSpecs.CreateStandardIndividual();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(templateIndividual, _compound, protocol).DowncastTo<IndividualSimulation>();
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);

         var overwriteParameterSet = new OverwriteParameterSet { Name = "TestSet" };
         overwriteParameterSet.Add(new ParameterValue { Path = $"Organism|{_compound.Name}|ThisPathDoesNotExist".ToObjectPath(), Value = 1.0 });
         _simulation.AddOverwriteParameterSetSelection(_compound.Name, overwriteParameterSet);
      }

      [Observation]
      public void should_throw_a_cannot_apply_overwrite_parameter_set_exception_and_fail_to_create_the_simulation()
      {
         The.Action(() => DomainFactoryForSpecs.AddModelToSimulation(_simulation)).ShouldThrowAn<CannotApplyOverwriteParameterSetException>();
      }
   }
}