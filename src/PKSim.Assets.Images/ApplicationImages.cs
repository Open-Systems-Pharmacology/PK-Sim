using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Utility.Collections;

namespace PKSim.Assets.Images
{
   public static class ApplicationImages
   {
      private static readonly ICache<string, ApplicationImage> _allImages = new Cache<string, ApplicationImage>();
      private static IList<ApplicationImage> _allImageList;

      public static readonly ApplicationImage Splash = createImageFrom(Images.PKSim, "PKSim");
      public static readonly ApplicationImage Model4Comp = createImageFrom(Images.SmallMoleculesStructure, "4Comp");
      public static readonly ApplicationImage Model4Comp2Pores = createImageFrom(Images.TwoPoresModelStructure, "TwoPores");

      private static ApplicationImage createImageFrom(Image bitmap, string imageName)
      {
         var appImage = new ApplicationImage(bitmap);
         _allImages.Add(imageName, appImage);
         return appImage;
      }

      public static int ImageIndex(ApplicationImage image)
      {
         if (_allImageList == null)
            _allImageList = _allImages.ToList();
         return _allImageList.IndexOf(image);
      }

      public static int ImageIndex(string imageName)
      {
         return ImageIndex(ImageByName(imageName));
      }

      public static ApplicationImage ImageByName(string imageName)
      {
         return _allImages[imageName];
      }
   }
}