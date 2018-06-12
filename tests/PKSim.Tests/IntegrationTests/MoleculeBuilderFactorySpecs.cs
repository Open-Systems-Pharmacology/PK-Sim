using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using IMoleculeBuilderFactory = PKSim.Core.Model.IMoleculeBuilderFactory;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_MoleculeBuilderFactory : ContextForIntegration<IMoleculeBuilderFactory>
   {
   }

   public class When_creating_a_molecule_for_a_given_molecule_type : concern_for_MoleculeBuilderFactory
   {
      private QuantityType _moleculeType;
      private IFormulaCache _formulaCache;
      private IMoleculeBuilder _result;

      protected override void Context()
      {
         base.Context();
         _moleculeType = QuantityType.Enzyme;
         _formulaCache = new FormulaCache();
      }

      protected override void Because()
      {
         _result = sut.Create(_moleculeType, _formulaCache);
      }

      [Observation]
      public void should_retrieve_a_template_molecule_from_the_molecule_repository()
      {
         _result.ShouldNotBeNull();
      }

      [Observation]
      public void should_add_the_molecule_default_start_value_to_the_given_formula_cache()
      {
         _formulaCache.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void should_add_the_concentration_parameter_to_the_created_molecule_builder_as_a_local_parameter()
      {
         var param = _result.Parameter(CoreConstants.Parameters.CONCENTRATION);
         param.ShouldNotBeNull();
         param.BuildMode.ShouldBeEqualTo(ParameterBuildMode.Local);
      }

      [Observation]
      public void should_have_kept_the_global_relative_expression_parameters()
      {
         CoreConstants.Parameters.AllGlobalRelExpParameters.Each(parmaterName => _result.Parameter(parmaterName).ShouldNotBeNull());
      }
   }

   public class When_creating_a_molecule_for_a_transporter : concern_for_MoleculeBuilderFactory
   {
      private IMoleculeBuilder _result;

      protected override void Because()
      {
         _result = sut.Create(QuantityType.Transporter, new FormulaCache());
      }

      [Observation]
      public void should_remove_the_global_relative_expression_paramters_that_do_not_make_sense_for_trnasporter()
      {
         CoreConstants.Parameters.AllGlobalRelExpParameters.Each(parmaterName => _result.Parameter(parmaterName).ShouldBeNull());
      }
   }

   public abstract class concern_for_creating_a_builder_from_a_compound : concern_for_MoleculeBuilderFactory
   {
      protected Compound _compound;
      protected IFormulaCache _formulaCache;
      protected ParameterAlternative _alternativeLipo1;
      protected ParameterAlternative _alternativePerm1;
      protected ParameterAlternative _alternativeLipo2;
      protected ParameterAlternative _alternativePerm2;
      private ICompoundFactory _compoundFactory;
      private IParameterAlternativeFactory _parameterAlternativeFactory;

      protected override void Context()
      {
         base.Context();
         _formulaCache = new FormulaCache();
         _compoundFactory = IoC.Resolve<ICompoundFactory>();
         _parameterAlternativeFactory = IoC.Resolve<IParameterAlternativeFactory>();
         _compound = _compoundFactory.Create().WithName("Comp");
         _compound.Parameter(Constants.Parameters.MOL_WEIGHT).Value = 250;
         //Two simple parameters without alternatives

         //one parameter defined as a constant for which an alternative was also specififed
         var lipoGroup = _compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_LIPOPHILICITY);
         _alternativeLipo1 = _parameterAlternativeFactory.CreateAlternativeFor(lipoGroup).WithName("ALT_LIPO1").WithId("ALT_LIPO1");
         _alternativeLipo1.Parameter(CoreConstants.Parameters.LIPOPHILICITY).Value = 2;
         _alternativeLipo2 = _parameterAlternativeFactory.CreateAlternativeFor(lipoGroup).WithName("ALT_LIPO2").WithId("ALT_LIPO2");
         _alternativeLipo2.Parameter(CoreConstants.Parameters.LIPOPHILICITY).Value = 5;
         lipoGroup.AddAlternative(_alternativeLipo1);
         lipoGroup.AddAlternative(_alternativeLipo2);

         //one parameter defined as a formula with a default calculated alternative

         var permAlternativeGroup = _compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_PERMEABILITY);

         //value cannot be changed by user
         _alternativePerm1 = _parameterAlternativeFactory.CreateDefaultAlternativeFor(permAlternativeGroup).WithName("ALT_PERM1").WithId("ALT_PERM1");
         _alternativePerm2 = _parameterAlternativeFactory.CreateAlternativeFor(permAlternativeGroup).WithName("ALT_PERM2").WithId("ALT_PERM2");
         _alternativePerm2.Parameter(CoreConstants.Parameters.PERMEABILITY).Value = 10;
         permAlternativeGroup.AddAlternative(_alternativePerm1);
         permAlternativeGroup.AddAlternative(_alternativePerm2);
      }
   }

   public class When_mapping_a_compound_to_a_molecule_builder : concern_for_creating_a_builder_from_a_compound
   {
      private IMoleculeBuilder _molecule;
      private CompoundProperties _compoundProperties;
      private InteractionProperties _interactionProperties;

      protected override void Context()
      {
         base.Context();
         _compoundProperties = new CompoundProperties();
         _interactionProperties = new InteractionProperties();
      }

      protected override void Because()
      {
         _molecule = sut.Create(_compound, _compoundProperties, _interactionProperties, _formulaCache);
      }

      [Observation]
      public void should_return_the_default_molecule_from_the_molecule_factory()
      {
         _molecule.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_set_the_name_of_the_molecule_builder_to_the_compound_name()
      {
         _molecule.Name.ShouldBeEqualTo(_compound.Name);
      }

      [Observation]
      public void should_set_the_default_values_of_all_default_parameter_into_the_molecule_that_where_fixed_by_the_user()
      {
         _molecule.Parameter(Constants.Parameters.MOL_WEIGHT).Value.ShouldBeEqualTo(_compound.Parameter(Constants.Parameters.MOL_WEIGHT).Value);
      }
   }

   public class When_mapping_a_compound_to_a_molecule_builder_based_on_a_compound_settings_using_only_the_default_alternatives : concern_for_creating_a_builder_from_a_compound
   {
      private CompoundProperties _compoundProperties;
      private IMoleculeBuilder _molecule;
      private InteractionProperties _interactionProperties;

      protected override void Context()
      {
         base.Context();
         _compoundProperties = new CompoundProperties();
         _interactionProperties = new InteractionProperties();
         _compoundProperties.AddCompoundGroupSelection(new CompoundGroupSelection {AlternativeName = _alternativeLipo2.Name, GroupName = CoreConstants.Groups.COMPOUND_LIPOPHILICITY});
         _compoundProperties.AddCompoundGroupSelection(new CompoundGroupSelection {AlternativeName = _alternativePerm1.Name, GroupName = CoreConstants.Groups.COMPOUND_PERMEABILITY});
      }

      protected override void Because()
      {
         _molecule = sut.Create(_compound, _compoundProperties, _interactionProperties, _formulaCache);
      }

      [Observation]
      public void should_let_the_parameter_defined_as_formula_untouched_but_not_editable()
      {
         var permParameter = _molecule.Parameter(CoreConstants.Parameters.PERMEABILITY);
         permParameter.IsFixedValue.ShouldBeFalse();
         permParameter.Formula.ShouldBeAnInstanceOf<ExplicitFormula>();
         permParameter.Editable.ShouldBeFalse();
      }

      [Observation]
      public void should_update_the_parameter_value_according_to_the_selected_alternative_for_other_parameters()
      {
         var lipoParameter = _molecule.Parameter(CoreConstants.Parameters.LIPOPHILICITY);
         lipoParameter.IsFixedValue.ShouldBeFalse();
         lipoParameter.Value.ShouldBeEqualTo(_alternativeLipo2.Parameter(CoreConstants.Parameters.LIPOPHILICITY).Value);
      }
   }

   public class When_mapping_a_compound_to_a_molecule_builder_based_on_a_compound_settings_not_using_the_default_alternatives : concern_for_creating_a_builder_from_a_compound
   {
      private CompoundProperties _compoundProperties;
      private IMoleculeBuilder _molecule;
      private InteractionProperties _interactionProperties;

      protected override void Context()
      {
         base.Context();
         _interactionProperties = new InteractionProperties();
         _compoundProperties = new CompoundProperties();
         _compoundProperties.AddCompoundGroupSelection(new CompoundGroupSelection {AlternativeName = _alternativeLipo1.Name, GroupName = CoreConstants.Groups.COMPOUND_LIPOPHILICITY});
         _compoundProperties.AddCompoundGroupSelection(new CompoundGroupSelection {AlternativeName = _alternativePerm2.Name, GroupName = CoreConstants.Groups.COMPOUND_PERMEABILITY});
      }

      protected override void Because()
      {
         _molecule = sut.Create(_compound, _compoundProperties, _interactionProperties, _formulaCache);
      }

      [Observation]
      public void should_override_the_parameters_define_as_formula()
      {
         var permParameter = _molecule.Parameter(CoreConstants.Parameters.PERMEABILITY);
         permParameter.IsFixedValue.ShouldBeFalse();
         permParameter.Formula.ShouldBeAnInstanceOf<ConstantFormula>();
         permParameter.Value.ShouldBeEqualTo(_alternativePerm2.Parameter(CoreConstants.Parameters.PERMEABILITY).Value);
      }

      [Observation]
      public void should_update_the_parameter_value_according_to_the_selected_alternative_for_other_parameters()
      {
         var lipoParameter = _molecule.Parameter(CoreConstants.Parameters.LIPOPHILICITY);
         lipoParameter.IsFixedValue.ShouldBeFalse();
         lipoParameter.Value.ShouldBeEqualTo(_alternativeLipo1.Parameter(CoreConstants.Parameters.LIPOPHILICITY).Value);
      }
   }

   public class When_mapping_a_compound_to_a_molecule_builder_with_some_inhibition_parameters_used_in_the_simulation : concern_for_creating_a_builder_from_a_compound
   {
      private CompoundProperties _compoundProperties;
      private IMoleculeBuilder _molecule;
      private InteractionProperties _interactionProperties;

      protected override void Context()
      {
         base.Context();
         _interactionProperties = new InteractionProperties();
         var inhibitionProcess = new InhibitionProcess {InteractionType = InteractionType.CompetitiveInhibition}.WithName("Proc");
         var interaction = new InteractionSelection {CompoundName = _compound.Name, ProcessName = inhibitionProcess.Name};
         _compound.AddProcess(inhibitionProcess);
         _interactionProperties.AddInteraction(interaction);
         _compoundProperties = new CompoundProperties();
      }

      protected override void Because()
      {
         _molecule = sut.Create(_compound, _compoundProperties, _interactionProperties, _formulaCache);
      }

      [Observation]
      public void should_have_added_the_interaction_process_containers()
      {
         _molecule.InteractionContainerCollection.Any().ShouldBeTrue();
      }
   }

   public class When_mapping_a_compound_to_a_molecule_builder_with_some_inhibition_parameters_used_in_the_simulation_but_for_irrerversible_inhibition : concern_for_creating_a_builder_from_a_compound
   {
      private CompoundProperties _compoundProperties;
      private IMoleculeBuilder _molecule;
      private InteractionProperties _interactionProperties;

      protected override void Context()
      {
         base.Context();
         _interactionProperties = new InteractionProperties();
         var inhibitionProcess = new InhibitionProcess().WithName("Proc");
         _compound.AddProcess(inhibitionProcess);
         _compoundProperties = new CompoundProperties();
      }

      protected override void Because()
      {
         _molecule = sut.Create(_compound, _compoundProperties, _interactionProperties, _formulaCache);
      }

      [Observation]
      public void should_not_add_the_interaction_container()
      {
         _molecule.InteractionContainerCollection.Any().ShouldBeFalse();
      }
   }
}