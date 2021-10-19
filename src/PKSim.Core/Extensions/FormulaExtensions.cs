using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Assets;

namespace PKSim.Core.Extensions
{
   public static class FormulaExtensions
   {
      public static TFormula InitializedWith<TFormula>(this TFormula tableFormula, string xName, string yName, IDimension xDimension, IDimension yDimension) where TFormula : TableFormula
      {
         tableFormula.XName = xName;
         tableFormula.YName = yName;
         tableFormula.XDimension = xDimension;
         tableFormula.Dimension = yDimension;
         return tableFormula;
      }

      public static void ReplaceKeywordInObjectPaths(
         this IFormula formula,
         string keyword,
         string replacementValue)

      {
         ReplaceKeywordsInObjectPaths(formula, new []{keyword}, new []{replacementValue});
      }
      /// <summary>
      /// In each object path: replaces path entries from <paramref name="keywords"/> with entries from <paramref name="replacementValues"/>
      /// </summary>
      public static void ReplaceKeywordsInObjectPaths(
         this IFormula formula, 
         string[] keywords,
         string[] replacementValues)
      {
         if (formula == null)
            return;

         if (keywords.Length != replacementValues.Length)
            throw new InvalidArgumentException(PKSimConstants.Error.KeywordsAndReplacementsSizeDiffer);

         foreach (var objectPath in formula.ObjectPaths)
         {
            for (int i = 0; i < keywords.Length; i++)
            {
               objectPath.Replace(keywords[i], replacementValues[i]);
            }
         }
      }
   }
}