﻿using System;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Services
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

   public static class LocalizationConverter
   {
      public static Localization ConvertToLocalization(TissueLocation tissueLocation, MembraneLocation membraneLocation,
         IntracellularVascularEndoLocation vascularEndoLocation)
      {
         switch (tissueLocation)
         {
            case TissueLocation.Interstitial:
               return Localization.Interstitial | Localization.BloodCellsIntracellular | Localization.VascMembraneTissueSide;

            case TissueLocation.Intracellular:
               var vacEndoLocalization = vascularEndoLocation == IntracellularVascularEndoLocation.Endosomal
                  ? Localization.VascEndosome
                  : Localization.VascMembraneTissueSide;
               return Localization.Intracellular | Localization.BloodCellsIntracellular | vacEndoLocalization;

            case TissueLocation.ExtracellularMembrane:
               var membLocation = membraneLocation == MembraneLocation.Apical
                  ? Localization.VascMembranePlasmaSide
                  : Localization.VascMembraneTissueSide;
               return Localization.Interstitial | Localization.BloodCellsMembrane | membLocation;

            default:
               throw new ArgumentOutOfRangeException(nameof(tissueLocation), tissueLocation, null);
         }
      }
   }

   
}