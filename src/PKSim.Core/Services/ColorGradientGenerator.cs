using System.Collections.Generic;
using System.Drawing;

namespace PKSim.Core.Services
{
   public interface IColorGradientGenerator
   {
      /// <summary>
      ///    Generates a color gradient between the <paramref name="start" /> color and the <paramref name="end" /> color using
      ///    the number of <paramref name="steps" />
      /// </summary>
      /// <param name="start">Start color of the gradient</param>
      /// <param name="end">End color of the gradient</param>
      /// <param name="steps">Number of colors to generate between start and end</param>
      /// <returns></returns>
      IReadOnlyList<Color> GenerateGradient(Color start, Color end, int steps);
   }

   public class ColorGradientGenerator : IColorGradientGenerator
   {
      public IReadOnlyList<Color> GenerateGradient(Color start, Color end, int steps)
      {
         var colors = new List<Color>();
         if (steps == 0)
            return colors;

         colors.Add(start);
         if (steps == 1)
            return colors;

         int stepA = ((end.A - start.A) / (steps - 1));
         int stepR = ((end.R - start.R) / (steps - 1));
         int stepG = ((end.G - start.G) / (steps - 1));
         int stepB = ((end.B - start.B) / (steps - 1));

         for (int i = 1; i < steps - 1; i++)
         {
            colors.Add(Color.FromArgb(start.A + (stepA * i),
               start.R + (stepR * i),
               start.G + (stepG * i),
               start.B + (stepB * i)));
         }
         colors.Add(end);
         return colors;
      }
   }
}