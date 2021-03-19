using System;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   [Flags]
   public enum Localization
   {
      None = 0,
      Intracellular = 1 << 0,
      Interstitial = 1 << 1,
      BloodCellsMembrane = 1 << 2,
      BloodCellsIntracellular = 1 << 3,
      VascEndosome = 1 << 4,
      VascMembranePlasmaSide = 1 << 5,
      VascMembraneTissueSide = 1 << 6,

      InTissue = Intracellular | Interstitial,
      InBloodCells = BloodCellsMembrane | BloodCellsIntracellular,
      InVascularEndothelium = VascEndosome | VascMembranePlasmaSide | VascMembraneTissueSide
   }

   public static class LocalizationExtensions
   {
      public static bool Is(this Localization localization, Localization localizationToCompare)
      {
         return (localization & localizationToCompare) != 0;
      }
   }

   public abstract class IndividualProtein : IndividualMolecule
   {
      public Localization Localization { get; set; } = Localization.None;

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceProtein = sourceObject as IndividualProtein;
         if (sourceProtein == null) return;
         Localization = sourceProtein.Localization;
      }

      public bool IsIntracellular
      {
         get => Localization.Is(Localization.Intracellular);
         set => setLocalizationFlag(Localization.Intracellular, value);
      }

      public bool IsInterstitial
      {
         get => Localization.Is(Localization.Interstitial);
         set => setLocalizationFlag(Localization.Interstitial, value);
      }

      public bool InBloodCells => Localization.Is(Localization.InBloodCells);

      public bool InTissue => Localization.Is(Localization.InTissue);

      public bool InVascularEndothelium => Localization.Is(Localization.InVascularEndothelium);

      public bool IsBloodCellsMembrane
      {
         get => Localization.Is(Localization.BloodCellsMembrane);
         set => setLocalizationFlag(Localization.BloodCellsMembrane, value);
      }

      private void setLocalizationFlag(Localization localization, bool value)
      {
         if (Localization.Is(localization) == value)
            return;

         Localization ^= localization;
      }

      public bool IsBloodCellsIntracellular
      {
         get => Localization.Is(Localization.BloodCellsIntracellular);
         set => setLocalizationFlag(Localization.BloodCellsIntracellular, value);
      }

      public bool IsVascEndosome
      {
         get => Localization.Is(Localization.VascEndosome);
         set => setLocalizationFlag(Localization.VascEndosome, value);
      }

      public bool IsVascMembranePlasmaSide
      {
         get => Localization.Is(Localization.VascMembranePlasmaSide);
         set => setLocalizationFlag(Localization.VascMembranePlasmaSide, value);
      }

      public bool IsVascMembraneTissueSide
      {
         get => Localization.Is(Localization.VascMembraneTissueSide);
         set => setLocalizationFlag(Localization.VascMembraneTissueSide, value);
      }
   }
}