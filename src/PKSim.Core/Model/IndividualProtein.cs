using System;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public enum TissueLocation
   {
      ExtracellularMembrane,
      Intracellular,
      Interstitial,
   }

   public enum IntracellularVascularEndoLocation
   {
      Endosomal,
      Interstitial
   }

   [Flags]
   public enum Localization
   {
      None = 0,
      Intracellular = 1 << 0,
      Interstitial = 1 << 1,
      BloodCellsMembrane = 1 << 2,
      BloodCellsIntracellular = 1 << 3,
      VascEndosome = 1 << 4,
      VascMembraneApical = 1 << 5,
      VascMembraneBasolateral = 1 << 6,
    
      InTissue = Intracellular | Interstitial,
      InBloodCells = BloodCellsMembrane | BloodCellsIntracellular,
      InVascularEndothelium = VascEndosome | VascMembraneApical | VascMembraneBasolateral
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
      public Localization Localization { get; set; } = Localization.Intracellular;
      private TissueLocation _tissueLocation;
      private MembraneLocation _membraneLocation;
      private IntracellularVascularEndoLocation _intracellularVascularEndoLocation;

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceProtein = sourceObject as IndividualProtein;
         if (sourceProtein == null) return;
         Localization = sourceProtein.Localization;

         //TODO REMOVE
         TissueLocation = sourceProtein.TissueLocation;
         MembraneLocation = sourceProtein.MembraneLocation;
         IntracellularVascularEndoLocation = sourceProtein.IntracellularVascularEndoLocation;
      }

      public virtual MembraneLocation MembraneLocation
      {
         get => _membraneLocation;
         set => SetProperty(ref _membraneLocation, value);
      }

      public virtual TissueLocation TissueLocation
      {
         get => _tissueLocation;
         set => SetProperty(ref _tissueLocation, value);
      }

      public virtual IntracellularVascularEndoLocation IntracellularVascularEndoLocation
      {
         get => _intracellularVascularEndoLocation;
         set => SetProperty(ref _intracellularVascularEndoLocation, value);
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
         if(Localization.Is(localization)==value)
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

      public bool IsVascMembraneApical
      {
         get => Localization.Is(Localization.VascMembraneApical);
         set => setLocalizationFlag(Localization.VascMembraneApical, value);
      }

      public bool IsVascMembraneBasolateral
      {
         get => Localization.Is(Localization.VascMembraneBasolateral);
         set => setLocalizationFlag(Localization.VascMembraneBasolateral, value);
      }
   }
}