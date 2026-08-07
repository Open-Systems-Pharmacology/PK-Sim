using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Snapshots;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Infrastructure;
using Compound = PKSim.Core.Model.Compound;
using IMoleculeBuilderFactory = PKSim.Core.Model.IMoleculeBuilderFactory;
using static PKSim.Core.CoreConstants.Parameters;
using static PKSim.Core.CoreConstants.Groups;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_bile_salt_partition_coefficient : ContextForIntegration<IMoleculeBuilderFactory>
   {
      private const string _micelleAffinityPenaltyBases = "Micelle affinity penalty basic compounds";
      protected const double _userPartitionCoefficient = 7.5;

      protected static readonly string[] _advancedSolubilityConstants =
      {
         CRITICAL_MICELLAR_CONCENTRATION,
         BILE_SALT_PARTITION_COEFFICIENT_CONSTANT_1,
         BILE_SALT_PARTITION_COEFFICIENT_CONSTANT_2,
         BILE_SALT_PARTITION_COEFFICIENT_IONIZED
      };

      protected Compound _compound;
      protected ParameterAlternativeGroup _bileSaltGroup;
      protected ParameterAlternative _calculatedAlternative;
      protected ParameterAlternative _userAlternative;

      protected override void Context()
      {
         base.Context();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();

         //a basic compound, so that K_i is derived from K_n instead of collapsing to zero for a neutral compound
         _compound.Parameter(Constants.Parameters.ParameterCompoundType(0)).Value = (int) CompoundType.Base;
         _compound.Parameter(ParameterPKa(0)).Value = 9;

         _bileSaltGroup = _compound.ParameterAlternativeGroup(COMPOUND_BILE_SALT_PARTITION_COEFFICIENT);
         _calculatedAlternative = _bileSaltGroup.AllAlternatives.Single();

         _userAlternative = IoC.Resolve<IParameterAlternativeFactory>().CreateAlternativeFor(_bileSaltGroup).WithName("Measured");
         _userAlternative.Parameter(BILE_SALT_PARTITION_COEFFICIENT_NEUTRAL).Value = _userPartitionCoefficient;
         _bileSaltGroup.AddAlternative(_userAlternative);
      }

      protected MoleculeBuilder MoleculeUsing(ParameterAlternative alternative)
      {
         var compoundProperties = new CompoundProperties();
         compoundProperties.AddCompoundGroupSelection(new CompoundGroupSelection {GroupName = _bileSaltGroup.Name, AlternativeName = alternative.Name});
         return sut.Create(_compound, compoundProperties, new InteractionProperties(), new FormulaCache());
      }

      protected double IonizedCoefficientExpectedFor(MoleculeBuilder molecule, double neutralCoefficient) =>
         neutralCoefficient - molecule.Parameter(_micelleAffinityPenaltyBases).Value;
   }

   public class When_creating_a_compound_with_advanced_solubility_parameters : concern_for_bile_salt_partition_coefficient
   {
      [Observation]
      public void should_define_a_single_calculated_default_alternative_for_the_neutral_partition_coefficient()
      {
         //the user alternative added by the context is not part of a freshly created compound
         var freshCompound = IoC.Resolve<ICompoundFactory>().Create();
         var group = freshCompound.ParameterAlternativeGroup(COMPOUND_BILE_SALT_PARTITION_COEFFICIENT);
         var alternative = group.AllAlternatives.Single();

         alternative.Name.ShouldBeEqualTo(PKSimConstants.UI.CalculatedAlernative);
         alternative.IsDefault.ShouldBeTrue();
         alternative.Parameter(BILE_SALT_PARTITION_COEFFICIENT_NEUTRAL).Formula.IsConstant().ShouldBeFalse();
      }

      [Observation]
      public void should_group_the_four_constants_under_advanced_solubility()
      {
         _compound.AllParameters(x => string.Equals(x.GroupName, COMPOUND_ADVANCED_SOLUBILITY)).Select(x => x.Name)
            .ShouldOnlyContain(_advancedSolubilityConstants);
      }

      [Observation]
      public void should_keep_the_four_constants_as_plain_compound_parameters_outside_of_any_alternative()
      {
         var simpleParameterNames = _compound.AllSimpleParameters().Select(x => x.Name).ToList();
         _advancedSolubilityConstants.Each(x => simpleParameterNames.ShouldContain(x));

         _compound.AllParameterAlternativeGroups()
            .SelectMany(x => x.AllAlternatives)
            .SelectMany(x => x.AllParameters())
            .Any(x => _advancedSolubilityConstants.Contains(x.Name))
            .ShouldBeFalse();
      }

      [Observation]
      public void should_create_an_alternative_holding_the_neutral_partition_coefficient_only_as_an_editable_constant()
      {
         var parameter = _userAlternative.AllParameters().Single();
         parameter.Name.ShouldBeEqualTo(BILE_SALT_PARTITION_COEFFICIENT_NEUTRAL);
         parameter.Formula.IsConstant().ShouldBeTrue();
      }
   }

   public class When_building_a_molecule_using_the_calculated_partition_coefficient_alternative : concern_for_bile_salt_partition_coefficient
   {
      private MoleculeBuilder _molecule;

      protected override void Because()
      {
         _molecule = MoleculeUsing(_calculatedAlternative);
      }

      [Observation]
      public void should_reproduce_the_values_the_compound_itself_calculates()
      {
         _molecule.Parameter(BILE_SALT_PARTITION_COEFFICIENT_NEUTRAL).Value.ShouldBeEqualTo(_compound.Parameter(BILE_SALT_PARTITION_COEFFICIENT_NEUTRAL).Value, 1e-10);
         _molecule.Parameter(BILE_SALT_PARTITION_COEFFICIENT_IONIZED).Value.ShouldBeEqualTo(_compound.Parameter(BILE_SALT_PARTITION_COEFFICIENT_IONIZED).Value, 1e-10);
         _molecule.Parameter(CRITICAL_MICELLAR_CONCENTRATION).Value.ShouldBeEqualTo(_compound.Parameter(CRITICAL_MICELLAR_CONCENTRATION).Value, 1e-10);
      }
   }

   public class When_building_a_molecule_using_a_user_defined_partition_coefficient_alternative : concern_for_bile_salt_partition_coefficient
   {
      private MoleculeBuilder _molecule;

      protected override void Because()
      {
         _molecule = MoleculeUsing(_userAlternative);
      }

      [Observation]
      public void should_use_the_value_entered_by_the_user_for_the_neutral_coefficient()
      {
         _molecule.Parameter(BILE_SALT_PARTITION_COEFFICIENT_NEUTRAL).Value.ShouldBeEqualTo(_userPartitionCoefficient, 1e-10);
      }

      [Observation]
      public void should_derive_the_ionized_coefficient_from_the_value_entered_by_the_user()
      {
         _molecule.Parameter(BILE_SALT_PARTITION_COEFFICIENT_IONIZED).Value
            .ShouldBeEqualTo(IonizedCoefficientExpectedFor(_molecule, _userPartitionCoefficient), 1e-10);
      }

      [Observation]
      public void should_leave_the_advanced_solubility_constants_untouched()
      {
         _molecule.Parameter(CRITICAL_MICELLAR_CONCENTRATION).Value.ShouldBeEqualTo(_compound.Parameter(CRITICAL_MICELLAR_CONCENTRATION).Value, 1e-10);
         double.IsNaN(_molecule.Parameter(BILE_SALT_PARTITION_COEFFICIENT_IONIZED).Value).ShouldBeFalse();
      }
   }

   public class When_round_tripping_a_compound_with_advanced_solubility_settings_through_a_snapshot : concern_for_bile_salt_partition_coefficient
   {
      private const double _editedMicellarConcentration = 2000;
      private Compound _roundTrippedCompound;

      protected override void Context()
      {
         base.Context();
         _userAlternative.IsDefault = true;
         _calculatedAlternative.IsDefault = false;

         var micellarConcentration = _compound.Parameter(CRITICAL_MICELLAR_CONCENTRATION);
         micellarConcentration.Value = _editedMicellarConcentration;
         micellarConcentration.IsDefault = false;
      }

      protected override void Because()
      {
         var compoundMapper = IoC.Resolve<CompoundMapper>();
         var snapshot = compoundMapper.MapToSnapshot(_compound).Result;
         _roundTrippedCompound = compoundMapper.MapToModel(snapshot, new SnapshotContext(new PKSimProject(), SnapshotVersions.Current)).Result;
      }

      [Observation]
      public void should_have_restored_the_alternatives_and_the_one_flagged_as_default()
      {
         var bileSaltGroup = _roundTrippedCompound.ParameterAlternativeGroup(COMPOUND_BILE_SALT_PARTITION_COEFFICIENT);
         bileSaltGroup.AllAlternatives.Select(x => x.Name).ShouldOnlyContain(PKSimConstants.UI.CalculatedAlernative, _userAlternative.Name);
         bileSaltGroup.DefaultAlternative.Name.ShouldBeEqualTo(_userAlternative.Name);
         bileSaltGroup.AlternativeByName(_userAlternative.Name).Parameter(BILE_SALT_PARTITION_COEFFICIENT_NEUTRAL).Value.ShouldBeEqualTo(_userPartitionCoefficient, 1e-10);
      }

      [Observation]
      public void should_have_restored_the_advanced_solubility_constant_edited_by_the_user()
      {
         _roundTrippedCompound.Parameter(CRITICAL_MICELLAR_CONCENTRATION).Value.ShouldBeEqualTo(_editedMicellarConcentration, 1e-10);
      }
   }
}
