using PKSim.Core.Model;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Extensions
{
   public static class RepresentationInfoExtensions
   {
      public static PathElementDTO ToPathElement(this RepresentationInfo representationInfo)
      {
         return new PathElementDTO
         {
            Description = representationInfo.DisplayName,
            IconName = representationInfo.IconName,
            DisplayName = representationInfo.DisplayName,
         };
      }

      public static void UpdatePathElement(this RepresentationInfo representationInfo, PathElementDTO pathElementDTO)
      {
         pathElementDTO.Description = representationInfo.Description;
         pathElementDTO.DisplayName = representationInfo.DisplayName;

         if (!string.IsNullOrEmpty(representationInfo.IconName))
            pathElementDTO.IconName = representationInfo.IconName;
      }
   }
}