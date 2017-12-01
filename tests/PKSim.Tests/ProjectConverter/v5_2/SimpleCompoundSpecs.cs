using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.IntegrationTests;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_2
{
   public abstract class concern_for_SimpleCompound : ContextWithLoadedProject<Converter514To521>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimpleCompound_515");
      }
   }

   public class When_converting_the_simple_compound_project : concern_for_SimpleCompound
   {
      private Compound _compound;

      protected override void Context()
      {
         _compound = First<Compound>();
      }

      [Observation]
      public void should_have_added_the_plasma_protein_binding_partner_parameter()
      {
         _compound.Parameter(CoreConstants.Parameter.PLASMA_PROTEIN_BINDING_PARTNER).ShouldNotBeNull();
         _compound.Parameter(CoreConstants.Parameter.PLASMA_PROTEIN_BINDING_PARTNER).Value.ShouldBeEqualTo((double) PlasmaProteinBindingPartner.Unknown);
      }

      [Observation]
      public void should_have_added_the_gain_per_charge_to_all_solubility_alternatives()
      {
         var solGroup = _compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_SOLUBILITY);
         foreach (var alternative in solGroup.AllAlternatives)
         {
            alternative.Parameter(CoreConstants.Parameter.SolubilityGainPerCharge).Value.ShouldBeEqualTo(1000);
         }
      }

      [Observation]
      public void should_have_added_the_gain_per_charge_parameter_to_the_compound()
      {
         var solubilityParameter = _compound.Parameter(CoreConstants.Parameter.SolubilityGainPerCharge);
         solubilityParameter.ShouldNotBeNull();
         solubilityParameter.GroupName.ShouldBeEqualTo(CoreConstants.Groups.COMPOUND_SOLUBILITY);
      }

      [Observation]
      public void should_have_added_the_required_binding_constant_parameters()
      {
         _compound.Parameter(ConverterConstants.Parameter.BP_AGP).ShouldNotBeNull();
         _compound.Parameter(ConverterConstants.Parameter.BP_ALBUMIN).ShouldNotBeNull();
         _compound.Parameter(ConverterConstants.Parameter.BP_UNKNOWN).ShouldNotBeNull();
      }

      [Observation]
      public void should_have_added_the_new_specific_parameter_for_permeability()
      {
         _compound.Parameter(ConverterConstants.Parameter.CalculatedSpecificIntestinalPermeability).ShouldNotBeNull();
      }

      [Observation]
      public void should_have_renamed_the_fu_parameter_to_fu_relative()
      {
         var fuGroup= _compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND);
         foreach (var alternative in fuGroup.AllAlternatives)
         {
            alternative.Parameter(CoreConstants.Parameter.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE).ShouldNotBeNull();
         }

         _compound.Parameter(CoreConstants.Parameter.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE).ShouldNotBeNull();
      }

      [Observation]
      public void should_have_renamed_the_parameter_Kd_fc_rn()
      {
         _compound.Parameter("Kd (FcRn)").ShouldBeNull();
         _compound.Parameter("Kd (FcRn) in endosomal space").ShouldNotBeNull();
      }
   }
}