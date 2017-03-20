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
         get { return _membraneLocation; }
         set
         {
            _membraneLocation = value;
            OnPropertyChanged(() => MembraneLocation);
         }
      }

      public virtual TissueLocation TissueLocation
      {
         get { return _tissueLocation; }
         set
         {
            _tissueLocation = value;
            OnPropertyChanged(() => TissueLocation);
         }
      }

      public virtual IntracellularVascularEndoLocation IntracellularVascularEndoLocation
      {
         get { return _intracellularVascularEndoLocation; }
         set
         {
            _intracellularVascularEndoLocation = value;
            OnPropertyChanged(() => IntracellularVascularEndoLocation);
         }
      }
   }
}