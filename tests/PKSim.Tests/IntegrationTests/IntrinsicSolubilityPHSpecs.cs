using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure;
using Compound = PKSim.Core.Model.Compound;
using static PKSim.Core.CoreConstants.Groups;
using static PKSim.CoreConstantsForSpecs;

namespace PKSim.IntegrationTests;

//https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3730
public abstract class concern_for_pH_intrinsic_solubility : ContextForIntegration<ICompoundFactory>
{
   private Compound _compound;

   public override void GlobalContext()
   {
      base.GlobalContext();
      _compound = DomainFactoryForSpecs.CreateStandardCompound();
   }

   protected IParameter PHIntrinsicSolubilityParameter => _compound.EntityAt<IParameter>(Parameters.PH_INTRINSIC_SOLUBILITY);

   protected void SetCompoundType(int index, CompoundType compoundType, double pKa)
   {
      _compound.Parameter(Constants.Parameters.ParameterCompoundType(index)).Value = (int) compoundType;
      _compound.Parameter(CoreConstants.Parameters.ParameterPKa(index)).Value = pKa;
   }
}

public class When_creating_a_compound : concern_for_pH_intrinsic_solubility
{
   [Observation]
   public void should_add_the_ph_intrinsic_solubility_parameter()
   {
      PHIntrinsicSolubilityParameter.ShouldNotBeNull();
   }

   [Observation]
   public void should_define_the_ph_intrinsic_solubility_parameter_as_visible_and_read_only_in_the_advanced_intestinal_solubility_group()
   {
      var parameter = PHIntrinsicSolubilityParameter;
      parameter.Visible.ShouldBeTrue();
      parameter.Editable.ShouldBeFalse();
      parameter.GroupName.ShouldBeEqualTo(COMPOUND_ADVANCED_SOLUBILITY);
   }

   [Observation]
   public void should_calculate_the_ph_intrinsic_solubility_parameter_value_with_a_formula()
   {
      PHIntrinsicSolubilityParameter.Formula.IsConstant().ShouldBeFalse();
   }

}

public class When_calculating_the_ph_intrinsic_solubility_for_a_neutral_compound : concern_for_pH_intrinsic_solubility
{
   [Observation]
   public void should_return_zero()
   {
      PHIntrinsicSolubilityParameter.Value.ShouldBeEqualTo(0, 1e-10);
   }
}

public class When_calculating_the_ph_intrinsic_solubility_for_a_compound_with_bases_only : concern_for_pH_intrinsic_solubility
{
   protected override void Because()
   {
      SetCompoundType(0, CompoundType.Base, 9);
   }

   [Observation]
   public void should_return_fourteen()
   {
      PHIntrinsicSolubilityParameter.Value.ShouldBeEqualTo(14, 1e-10);
   }
}

public class When_calculating_the_ph_intrinsic_solubility_for_a_compound_with_acids_only : concern_for_pH_intrinsic_solubility
{
   protected override void Because()
   {
      SetCompoundType(0, CompoundType.Acid, 4);
      SetCompoundType(1, CompoundType.Acid, 6);
   }

   [Observation]
   public void should_return_zero()
   {
      PHIntrinsicSolubilityParameter.Value.ShouldBeEqualTo(0, 1e-10);
   }
}

public class When_calculating_the_ph_intrinsic_solubility_for_a_zwitterionic_compound_with_one_base : concern_for_pH_intrinsic_solubility
{
   protected override void Because()
   {
      SetCompoundType(0, CompoundType.Acid, 4);
      SetCompoundType(1, CompoundType.Base, 9);
   }

   [Observation]
   public void should_return_the_mean_of_the_smallest_acid_pKa_and_the_base_pKa()
   {
      PHIntrinsicSolubilityParameter.Value.ShouldBeEqualTo((4 + 9) / 2.0, 1e-10);
   }
}

public class When_calculating_the_ph_intrinsic_solubility_for_a_zwitterionic_compound_with_two_bases : concern_for_pH_intrinsic_solubility
{
   protected override void Because()
   {
      SetCompoundType(0, CompoundType.Acid, 4);
      SetCompoundType(1, CompoundType.Base, 7);
      SetCompoundType(2, CompoundType.Base, 10);
   }

   [Observation]
   public void should_return_the_mean_of_the_largest_of_acid_and_smallest_base_pKa_and_the_largest_base_pKa()
   {
      PHIntrinsicSolubilityParameter.Value.ShouldBeEqualTo((7 + 10) / 2.0, 1e-10);
   }
}

//the Max(pKa_Acid_0; pKa_Base_0) of the two bases branch only becomes visible when the acid pKa is the larger one.
//With an acid pKa below the smallest base pKa the Max is never binding and a Min would yield the same result.
public class When_calculating_the_ph_intrinsic_solubility_for_a_zwitterionic_compound_with_two_bases_and_a_dominating_acid : concern_for_pH_intrinsic_solubility
{
   protected override void Because()
   {
      SetCompoundType(0, CompoundType.Acid, 8);
      SetCompoundType(1, CompoundType.Base, 7);
      SetCompoundType(2, CompoundType.Base, 10);
   }

   [Observation]
   public void should_use_the_acid_pKa_when_it_exceeds_the_smallest_base_pKa()
   {
      PHIntrinsicSolubilityParameter.Value.ShouldBeEqualTo((8 + 10) / 2.0, 1e-10);
   }
}

public class When_calculating_the_ph_intrinsic_solubility_for_a_zwitterionic_compound_with_two_acids : concern_for_pH_intrinsic_solubility
{
   protected override void Because()
   {
      SetCompoundType(0, CompoundType.Acid, 4);
      SetCompoundType(1, CompoundType.Acid, 6);
      SetCompoundType(2, CompoundType.Base, 9);
   }

   [Observation]
   public void should_return_the_mean_of_the_smallest_acid_pKa_and_the_smallest_of_largest_acid_and_base_pKa()
   {
      PHIntrinsicSolubilityParameter.Value.ShouldBeEqualTo((4 + 6) / 2.0, 1e-10);
   }
}

//the Min(pKa_Acid_1; pKa_Base_0) of the two acids branch only becomes visible when the base pKa is the smaller one
public class When_calculating_the_ph_intrinsic_solubility_for_a_zwitterionic_compound_with_two_acids_and_a_dominating_base : concern_for_pH_intrinsic_solubility
{
   protected override void Because()
   {
      SetCompoundType(0, CompoundType.Acid, 4);
      SetCompoundType(1, CompoundType.Acid, 6);
      SetCompoundType(2, CompoundType.Base, 5);
   }

   [Observation]
   public void should_use_the_base_pKa_when_it_is_smaller_than_the_largest_acid_pKa()
   {
      PHIntrinsicSolubilityParameter.Value.ShouldBeEqualTo((4 + 5) / 2.0, 1e-10);
   }
}

public class When_calculating_the_ph_intrinsic_solubility_for_a_triprotic_base : concern_for_pH_intrinsic_solubility
{
   protected override void Because()
   {
      SetCompoundType(0, CompoundType.Base, 7);
      SetCompoundType(1, CompoundType.Base, 9);
      SetCompoundType(2, CompoundType.Base, 11);
   }

   [Observation]
   public void should_return_fourteen()
   {
      PHIntrinsicSolubilityParameter.Value.ShouldBeEqualTo(14, 1e-10);
   }
}

public class When_calculating_the_ph_intrinsic_solubility_for_a_triprotic_acid : concern_for_pH_intrinsic_solubility
{
   protected override void Because()
   {
      SetCompoundType(0, CompoundType.Acid, 3);
      SetCompoundType(1, CompoundType.Acid, 5);
      SetCompoundType(2, CompoundType.Acid, 7);
   }

   [Observation]
   public void should_return_zero()
   {
      PHIntrinsicSolubilityParameter.Value.ShouldBeEqualTo(0, 1e-10);
   }
}

//the pKa values are sorted into pKa_Acid_0/1 and pKa_Base_0/1 by the pKa_TwoAcids_*/pKa_TwoBases_* rates.
//The result must not depend on which compound type slot a given pKa was entered in.
public class When_calculating_the_ph_intrinsic_solubility_for_the_same_compound_entered_in_different_pKa_slots : concern_for_pH_intrinsic_solubility
{
   private readonly (CompoundType compoundType, double pKa)[] _groups =
   [
      (CompoundType.Acid, 4),
      (CompoundType.Base, 7),
      (CompoundType.Base, 10)
   ];

   [Observation]
   public void should_return_the_same_ph_for_every_permutation_of_the_pKa_slots()
   {
      foreach (var permutation in permutationsOf([0, 1, 2]))
      {
         for (var slot = 0; slot < _groups.Length; slot++)
         {
            var (compoundType, pKa) = _groups[permutation[slot]];
            SetCompoundType(slot, compoundType, pKa);
         }

         var permutationDescription = string.Join(",", permutation);
         PHIntrinsicSolubilityParameter.Value.ShouldBeEqualTo((7 + 10) / 2.0, 1e-10, $"pKa slot permutation [{permutationDescription}]");
      }
   }

   private static IEnumerable<int[]> permutationsOf(int[] items)
   {
      if (items.Length <= 1)
      {
         yield return items;
         yield break;
      }

      for (var i = 0; i < items.Length; i++)
      {
         var remaining = items.Where((_, index) => index != i).ToArray();
         foreach (var permutation in permutationsOf(remaining))
            yield return new[] {items[i]}.Concat(permutation).ToArray();
      }
   }
}

public class When_creating_a_simulation_with_a_compound : ContextForIntegration<ISimulationConstructor>
{
   private Simulation _simulation;
   private Compound _compound;
   private IContainer _compoundInSimulation => _simulation.Model.Root.EntityAt<IContainer>(_compound.Name);
   private IParameter _intrinsicSolubilityParameter => _compoundInSimulation.Parameter(Parameters.SOLUBILITY_INTRINSIC);
   private IParameter _phIntrinsicSolubilityParameter => _compoundInSimulation.Parameter(Parameters.PH_INTRINSIC_SOLUBILITY);
   private IParameter _solubilityAtReferencePH => _compoundInSimulation.Parameter(CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH);
   private IParameter _solubilityPKaRefPHFactor => _compoundInSimulation.Parameter(CoreConstants.Parameters.SOLUBILITY_PKA_PH_FACTOR);
   private IParameter _solubilityPKaIntrinsicPHFactor => _compoundInSimulation.Parameter(Parameters.INTRINSIC_SOLUBILITY_PKA_PH_FACTOR);

   public override void GlobalContext()
   {
      base.GlobalContext();
      var individual = DomainFactoryForSpecs.CreateStandardIndividual();
      _compound = DomainFactoryForSpecs.CreateStandardCompound();
      var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
      _simulation = DomainFactoryForSpecs.CreateSimulationWith(individual, _compound, protocol);
   }

   [Observation]
   public void should_create_the_ph_intrinsic_solubility_parameter()
   {
      var parameter = _phIntrinsicSolubilityParameter;
      parameter.ShouldNotBeNull();
      parameter.Value.ShouldBeEqualTo(0, 1e-10);
   }

   [Observation]
   public void should_add_the_intrinsic_solubility_parameter()
   {
      _intrinsicSolubilityParameter.ShouldNotBeNull();
   }

   [Observation]
   public void should_calculate_the_intrinsic_solubility_parameter_value_with_a_formula()
   {
      _intrinsicSolubilityParameter.Formula.IsConstant().ShouldBeFalse();
   }

   [Observation]
   public void should_calculate_the_intrinsic_solubility_from_the_solubility_at_reference_ph_and_the_pka_ph_factors()
   {
      var expected = _solubilityAtReferencePH.Value * _solubilityPKaRefPHFactor.Value / _solubilityPKaIntrinsicPHFactor.Value;
      _intrinsicSolubilityParameter.Value.ShouldBeEqualTo(expected, 1e-10);
   }

   [Observation]
   public void should_add_the_intrinsic_solubility_pka_ph_factor_parameter()
   {
      _solubilityPKaIntrinsicPHFactor.ShouldNotBeNull();
   }

   [Observation]
   public void should_define_the_intrinsic_solubility_pka_ph_factor_parameter_as_hidden_and_read_only()
   {
      _solubilityPKaIntrinsicPHFactor.Visible.ShouldBeFalse();
      _solubilityPKaIntrinsicPHFactor.Editable.ShouldBeFalse();
   }

   [Observation]
   public void should_calculate_the_intrinsic_solubility_pka_ph_factor_parameter_value_with_a_formula()
   {
      _solubilityPKaIntrinsicPHFactor.Formula.IsConstant().ShouldBeFalse();
   }

   [Observation]
   public void intrinsic_solubility_should_be_equal_to_solubility_at_reference_pH_when_settin_reference_pH_equal_to_intrinsic_pH()
   {
      _compoundInSimulation.Parameter(CoreConstants.Parameters.REFERENCE_PH).Value = _phIntrinsicSolubilityParameter.Value;
      _intrinsicSolubilityParameter.Value.ShouldBeEqualTo(_solubilityAtReferencePH.Value, 1e-10);
   }
}

/// <summary>
///    The standard compound is neutral, so all pKa pH factors collapse to 1 and the intrinsic solubility equals the
///    solubility at reference pH for trivial reasons. This context uses an ionizable compound for which the intrinsic
///    solubility must differ from the solubility at reference pH by a known, externally computed factor.
/// </summary>
public abstract class concern_for_intrinsic_solubility_of_an_ionizable_compound : ContextForIntegration<ISimulationConstructor>
{
   //di-base, verified against the MATLAB export of the PBBM test model
   protected const double _pKaBase0 = 8;
   protected const double _pKaBase1 = 1;
   protected const double _solubilityGainPerCharge = 2500;
   protected const double _referencePH = 6.5;
   protected const double _solubilityAtReferencePHValue = 7e-06; //[kg/l]

   //intrinsic pH of a compound without acidic groups, see PARAM_pH_Intrinsic_Solubility
   protected const double _expectedIntrinsicPH = 14;

   //S0 = S_ref * F(6.5)/F(14) with F(pH) = E[SolubilityGainPerCharge^(-|z|)] over the 4 protonation microstates.
   //F(6.5) = 0.031041070538483, F(14) = 0.999999000400900 => S0 = 2.1728771097e-07 kg/l
   protected const double _expectedIntrinsicSolubility = 2.1728771097e-07; //[kg/l]

   protected Simulation _simulation;
   protected Compound _compound;

   protected IContainer CompoundInSimulation => _simulation.Model.Root.EntityAt<IContainer>(_compound.Name);
   protected IParameter IntrinsicSolubilityParameter => CompoundInSimulation.Parameter(Parameters.SOLUBILITY_INTRINSIC);
   protected IParameter PHIntrinsicSolubilityParameter => CompoundInSimulation.Parameter(Parameters.PH_INTRINSIC_SOLUBILITY);
   protected IParameter SolubilityAtReferencePH => CompoundInSimulation.Parameter(CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH);
   protected IParameter ReferencePH => CompoundInSimulation.Parameter(CoreConstants.Parameters.REFERENCE_PH);
   protected IParameter SolubilityPKaRefPHFactor => CompoundInSimulation.Parameter(CoreConstants.Parameters.SOLUBILITY_PKA_PH_FACTOR);
   protected IParameter IntrinsicSolubilityPKaPHFactor => CompoundInSimulation.Parameter(Parameters.INTRINSIC_SOLUBILITY_PKA_PH_FACTOR);

   public override void GlobalContext()
   {
      base.GlobalContext();
      var individual = DomainFactoryForSpecs.CreateStandardIndividual();
      _compound = DomainFactoryForSpecs.CreateStandardCompound();

      setCompoundType(0, CompoundType.Base, _pKaBase0);
      setCompoundType(1, CompoundType.Base, _pKaBase1);

      //the solubility parameters are defined in the default alternative of the solubility group and not
      //as direct children of the compound. Setting them on the compound itself has no effect on the simulation.
      var solubilityAlternative = _compound.ParameterAlternativeGroup(COMPOUND_SOLUBILITY).DefaultAlternative;
      solubilityAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_GAIN_PER_CHARGE).Value = _solubilityGainPerCharge;
      solubilityAlternative.Parameter(CoreConstants.Parameters.REFERENCE_PH).Value = _referencePH;
      solubilityAlternative.Parameter(CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH).Value = _solubilityAtReferencePHValue;

      var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
      _simulation = DomainFactoryForSpecs.CreateSimulationWith(individual, _compound, protocol);
   }

   private void setCompoundType(int index, CompoundType compoundType, double pKa)
   {
      _compound.Parameter(Constants.Parameters.ParameterCompoundType(index)).Value = (int) compoundType;
      _compound.Parameter(CoreConstants.Parameters.ParameterPKa(index)).Value = pKa;
   }
}

public class When_calculating_the_intrinsic_solubility_of_an_ionizable_compound : concern_for_intrinsic_solubility_of_an_ionizable_compound
{
   [Observation]
   public void should_calculate_the_intrinsic_pH_of_a_compound_without_acidic_groups_as_fourteen()
   {
      PHIntrinsicSolubilityParameter.Value.ShouldBeEqualTo(_expectedIntrinsicPH, 1e-10);
   }

   //https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/3730#issuecomment-5662251824
   //Guards against the intrinsic solubility becoming an algebraic no-op: when the pKa pH factors of the intrinsic
   //solubility are evaluated at the reference pH instead of at the intrinsic pH, the ratio of the two factors is
   //exactly 1 and the intrinsic solubility silently equals the solubility at reference pH.
   [Observation]
   public void should_calculate_the_intrinsic_solubility_expected_for_the_pKa_values_and_the_solubility_gain_per_charge()
   {
      IntrinsicSolubilityParameter.Value.ShouldBeEqualTo(_expectedIntrinsicSolubility, 1e-9);
   }

   [Observation]
   public void should_calculate_an_intrinsic_solubility_well_below_the_solubility_at_reference_pH()
   {
      IntrinsicSolubilityParameter.Value.ShouldBeSmallerThan(0.5 * SolubilityAtReferencePH.Value);
   }

   [Observation]
   public void should_evaluate_the_two_pka_ph_factors_at_different_pH_values()
   {
      //the factor of the intrinsic solubility is evaluated at pH 14, the one of the reference solubility at pH 6.5
      IntrinsicSolubilityPKaPHFactor.Value.ShouldBeGreaterThan(2 * SolubilityPKaRefPHFactor.Value);
   }

}

/// <summary>
///    The intrinsic solubility is a property of the compound and must not depend on the reference point chosen to
///    report it. Moving the reference pH to the intrinsic pH and providing the solubility belonging to that pH
///    therefore has to reproduce the same intrinsic solubility.
/// </summary>
public class When_reporting_the_solubility_of_an_ionizable_compound_at_the_intrinsic_pH : concern_for_intrinsic_solubility_of_an_ionizable_compound
{
   private double _intrinsicSolubilityAtOriginalReferencePH;

   protected override void Because()
   {
      _intrinsicSolubilityAtOriginalReferencePH = IntrinsicSolubilityParameter.Value;

      ReferencePH.Value = _expectedIntrinsicPH;
      SolubilityAtReferencePH.Value = _intrinsicSolubilityAtOriginalReferencePH;
   }

   [Observation]
   public void should_not_change_the_intrinsic_solubility()
   {
      IntrinsicSolubilityParameter.Value.ShouldBeEqualTo(_intrinsicSolubilityAtOriginalReferencePH, 1e-9);
   }

   [Observation]
   public void should_evaluate_both_pka_ph_factors_at_the_same_pH()
   {
      IntrinsicSolubilityPKaPHFactor.Value.ShouldBeEqualTo(SolubilityPKaRefPHFactor.Value, 1e-9);
   }
}

/// <summary>
///    Saq(pH) = S_ref * F(pH_ref)/F(pH). The intrinsic solubility is by definition the minimum of the aqueous
///    solubility over all pH values, which is attained where F(pH) is maximal. This is what makes the
///    Max(Saq - S0; 0) clamp of the solubility increase from ionization a pure safeguard rather than a load
///    bearing term.
///    <para />
///    This context changes the reference pH of the simulation and must therefore stay in a class of its own:
///    all observations of a context share one simulation instance.
/// </summary>
public class When_scanning_the_aqueous_solubility_of_an_ionizable_compound_over_the_whole_pH_range : concern_for_intrinsic_solubility_of_an_ionizable_compound
{
   private double _intrinsicSolubility;
   private double _solubilityAtReferencePH;
   private double _pKaPHFactorAtReferencePH;

   protected override void Because()
   {
      _intrinsicSolubility = IntrinsicSolubilityParameter.Value;
      _solubilityAtReferencePH = SolubilityAtReferencePH.Value;
      _pKaPHFactorAtReferencePH = SolubilityPKaRefPHFactor.Value;
   }

   [Observation]
   public void should_never_calculate_an_aqueous_solubility_below_the_intrinsic_solubility()
   {
      //the reference pH is used here as a probe to evaluate F at an arbitrary pH
      for (var pH = 0.0; pH <= 14.0; pH += 0.25)
      {
         ReferencePH.Value = pH;
         var pKaPHFactorAtPH = SolubilityPKaRefPHFactor.Value;
         var aqueousSolubilityAtPH = _solubilityAtReferencePH * _pKaPHFactorAtReferencePH / pKaPHFactorAtPH;

         _intrinsicSolubility.ShouldBeSmallerThan(aqueousSolubilityAtPH * (1 + 1e-9));
      }
   }
}

public class When_calculating_the_intrinsic_solubility_of_an_ionizable_compound_without_solubility_gain_per_charge : concern_for_intrinsic_solubility_of_an_ionizable_compound
{
   protected override void Because()
   {
      //without a solubility gain per charge all microstates carry the same weight and the pKa pH factors are 1
      CompoundInSimulation.Parameter(CoreConstants.Parameters.SOLUBILITY_GAIN_PER_CHARGE).Value = 1;
   }

   [Observation]
   public void should_equal_the_solubility_at_reference_pH()
   {
      IntrinsicSolubilityParameter.Value.ShouldBeEqualTo(SolubilityAtReferencePH.Value, 1e-10);
   }

   [Observation]
   public void should_evaluate_both_pka_ph_factors_to_one()
   {
      SolubilityPKaRefPHFactor.Value.ShouldBeEqualTo(1, 1e-10);
      IntrinsicSolubilityPKaPHFactor.Value.ShouldBeEqualTo(1, 1e-10);
   }
}

public class When_calculating_the_intrinsic_solubility_of_a_compound_without_solubility_at_reference_pH : concern_for_intrinsic_solubility_of_an_ionizable_compound
{
   protected override void Because()
   {
      SolubilityAtReferencePH.Value = 0;
   }

   [Observation]
   public void should_return_zero_and_not_an_undefined_value()
   {
      double.IsNaN(IntrinsicSolubilityParameter.Value).ShouldBeFalse();
      IntrinsicSolubilityParameter.Value.ShouldBeEqualTo(0, 1e-10);
   }
}