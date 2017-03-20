using System;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    One type for each organ. This type allows us to easily create
   ///    collections of organs subset
   /// </summary>
   [Flags]
   public enum OrganType
   {
      None = 0,
      ArterialBlood = 2 << 0,
      Bone = 2 << 1,
      Brain = 2 << 2,
      Fat = 2 << 3,
      GallBladder = 2 << 4,
      Gonads = 2 << 5,
      Heart = 2 << 6,
      Kidney = 2 << 7,
      LargeIntestine = 2 << 8,
      Liver = 2 << 9,
      Lung = 2 << 10,
      Muscle = 2 << 11,
      Pancreas = 2 << 12,
      PortalVein = 2 << 13,
      Skin = 2 << 14,
      SmallIntestine = 2 << 15,
      Spleen = 2 << 16,
      Stomach = 2 << 17,
      VenousBlood = 2 << 18,
      Saliva = 2 << 19,
      PlasmaMetabolization = 2 << 20,
      EndogenousIgG = 2 << 21,
      PeripheralVenousBlood = 2 << 22,
      Lumen = 2 << 23,

      GiTractOrgans = SmallIntestine | LargeIntestine | Stomach,
      TissueNoGiTract = Bone | Brain | Fat | Gonads | Heart | Kidney | Liver | Lung | Muscle | Pancreas | Skin | Spleen,
      Tissue = TissueNoGiTract | GiTractOrgans,

      VascularSystem = ArterialBlood | PortalVein | VenousBlood
   }
}