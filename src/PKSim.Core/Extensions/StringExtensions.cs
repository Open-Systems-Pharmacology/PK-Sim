using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Extensions
{
   public static class StringExtensions
   {
      /// <summary>
      ///    Returns true if the container is representing a generic compartment such as blood cells, plasma or endosome
      /// </summary>
      public static bool IsSurrogate(this string containerName)
      {
         {
            return containerName.IsPlasma() || containerName.IsBloodCells() ||
                   containerName.IsEndosome() || containerName.IsVascularEndothelium();
         }
      }

      /// <summary>
      ///    Returns true if the container is representing an undefined molecule.
      /// </summary>
      public static bool IsUndefinedMolecule(this string containerName)
      {
         return containerName.IsOneOf(CoreConstants.Molecule.UndefinedLiver, CoreConstants.Molecule.UndefinedLiverTransporter);
      }

      public static bool IsEndosome(this string containerName) => string.Equals(containerName, CoreConstants.Compartment.ENDOSOME);

      public static bool IsVascularEndothelium(this string containerName) => string.Equals(containerName, CoreConstants.Compartment.VASCULAR_ENDOTHELIUM);

      public static bool IsBloodCells(this string containerName) => string.Equals(containerName, CoreConstants.Compartment.BLOOD_CELLS);

      public static bool IsPlasma(this string containerName) => string.Equals(containerName, CoreConstants.Compartment.PLASMA);
      
      public static bool IsInterstitial(this string containerName) => string.Equals(containerName, CoreConstants.Compartment.INTERSTITIAL);

      public static bool IsLumen(this string containerName) => string.Equals(containerName, CoreConstants.Organ.LUMEN);

      public static bool IsLiver(this string containerName) => string.Equals(containerName, CoreConstants.Organ.LIVER);
      
      public static bool IsKidney(this string containerName) => string.Equals(containerName, CoreConstants.Organ.KIDNEY);
      
      public static bool IsBrain(this string containerName) => string.Equals(containerName, CoreConstants.Organ.BRAIN);

      public static string ReplaceKeywords(this string input, string[] keywords, string[] replacementValues)
      {
         if (string.IsNullOrEmpty(input))
            return input;

         if (keywords.Length != replacementValues.Length)
            throw new InvalidArgumentException(PKSimConstants.Error.KeywordsAndReplacementsSizeDiffer);

         var replacedString = input;
         for (int i = 0; i < keywords.Length; i++)
         {
            replacedString = replacedString.Replace(keywords[i], replacementValues[i]);
         }

         return replacedString;
      }

     
   }
}