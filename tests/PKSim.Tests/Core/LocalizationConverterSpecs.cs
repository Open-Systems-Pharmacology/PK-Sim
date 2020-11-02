using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;
using static PKSim.Core.Snapshots.Services.LocalizationConverter;

namespace PKSim.Core
{
   public abstract class concern_for_LocalizationConverter : StaticContextSpecification 
   {

   }

   public class When_converting_molecule_tissue_and_membrane_location_to_new_localization : concern_for_LocalizationConverter
   {
      [Observation]
      public void should_return_the_expected_localization_for_interstitial()
      {
         ConvertToLocalization(TissueLocation.Interstitial, MembraneLocation.Apical, IntracellularVascularEndoLocation.Interstitial)
            .ShouldBeEqualTo(Localization.Interstitial | Localization.BloodCellsIntracellular | Localization.VascMembraneBasolateral);
      }

      [Observation]
      public void should_return_the_expected_localization_for_intracellular_with_vascular_endo_location_endosomal()
      {
         ConvertToLocalization(TissueLocation.Intracellular, MembraneLocation.Apical, IntracellularVascularEndoLocation.Endosomal)
            .ShouldBeEqualTo(Localization.Intracellular | Localization.BloodCellsIntracellular | Localization.VascEndosome);
      }

      [Observation]
      public void should_return_the_expected_localization_for_intracellular_with_vascular_endo_location_interstitial()
      {
         ConvertToLocalization(TissueLocation.Intracellular, MembraneLocation.Apical, IntracellularVascularEndoLocation.Interstitial)
            .ShouldBeEqualTo(Localization.Intracellular | Localization.BloodCellsIntracellular | Localization.VascMembraneBasolateral);
      }

      [Observation]
      public void should_return_the_expected_localization_for_extracellular_membrane_with_membrane_location_apical()
      {
         ConvertToLocalization(TissueLocation.ExtracellularMembrane, MembraneLocation.Apical, IntracellularVascularEndoLocation.Interstitial)
            .ShouldBeEqualTo(Localization.Interstitial | Localization.BloodCellsMembrane | Localization.VascMembraneApical);
      }


      [Observation]
      public void should_return_the_expected_localization_for_extracellular_membrane_with_membrane_location_basolateral()
      {
         ConvertToLocalization(TissueLocation.ExtracellularMembrane, MembraneLocation.Basolateral, IntracellularVascularEndoLocation.Interstitial)
            .ShouldBeEqualTo(Localization.Interstitial | Localization.BloodCellsMembrane | Localization.VascMembraneBasolateral);
      }

   }
}