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

   public abstract class IndividualProtein : IndividualMolecule
   {
      private TissueLocation _tissueLocation;
      private MembraneLocation _membraneLocation;
      private IntracellularVascularEndoLocation _intracellularVascularEndoLocation;

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceProtein = sourceObject as IndividualProtein;
         if (sourceProtein == null) return;
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
   }
}