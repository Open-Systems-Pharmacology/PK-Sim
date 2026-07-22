using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using OSPSuite.Utility.Collections;

namespace PKSim.UI.Views.Simulations
{
   /// <summary>
   ///    Raster (PNG) model-structure images embedded alongside <see cref="SimulationModelSelectionView"/>.
   ///    Moved out of the former <c>PKSim.Assets.Images</c> project — it only ever held these two PNGs
   ///    and a (Windows-only) <see cref="Image"/>-typed accessor consumed exclusively from this view.
   /// </summary>
   public static class ApplicationImages
   {
      private static readonly ICache<string, Image> _allImages = new Cache<string, Image>();
      private static IList<Image> _allImageList;

      public static readonly Image Model4Comp = createImageFrom(getEmbeddedImage("SmallMoleculesStructure"), "4Comp");
      public static readonly Image Model4Comp2Pores = createImageFrom(getEmbeddedImage("TwoPoresModelStructure"), "TwoPores");

      private static Image getEmbeddedImage(string imageName)
      {
         var assembly = Assembly.GetExecutingAssembly();
         var resourceName = typeof(ApplicationImages).Namespace + ".Resources." + imageName + ".png";

         using (var stream = assembly.GetManifestResourceStream(resourceName))
         {
            return stream == null ? null : Image.FromStream(stream);
         }
      }

      private static Image createImageFrom(Image bitmap, string imageName)
      {
         _allImages.Add(imageName, bitmap);
         return bitmap;
      }

      public static int ImageIndex(Image image)
      {
         if (_allImageList == null)
            _allImageList = _allImages.ToList();
         return _allImageList.IndexOf(image);
      }

      public static int ImageIndex(string imageName)
      {
         return ImageIndex(ImageByName(imageName));
      }

      public static Image ImageByName(string imageName)
      {
         return _allImages[imageName];
      }
   }
}
