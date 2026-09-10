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