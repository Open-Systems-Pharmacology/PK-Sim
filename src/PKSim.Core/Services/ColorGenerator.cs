using System.Drawing;

namespace PKSim.Core.Services
{
   public interface IColorGenerator
   {
      Color NextColor();
   }

   public class ColorGenerator : IColorGenerator
   {
      private int _colorIndex = 0;

      public Color NextColor()
      {
         return PKSimColors.DefaultSeriesColors[_colorIndex++ % PKSimColors.DefaultSeriesColors.Count];
      }
   }
}