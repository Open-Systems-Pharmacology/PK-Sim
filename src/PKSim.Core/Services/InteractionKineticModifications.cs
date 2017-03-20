using System;

namespace PKSim.Core.Services
{
   [Flags]
   public enum InteractionKineticModifications
   {
      KmNumerator = 1 << 0,
      KmDenominator = 1 << 1,
      KcatDenominator = 1 << 2,
      CLSpecDenominator = 1 << 3,
      CompetitiveInhibition = KmNumerator | CLSpecDenominator,
      NonCompetitiveInhibition = KmNumerator | KmDenominator | KcatDenominator | CLSpecDenominator,
      MixedInhibition = KmNumerator | KmDenominator | KcatDenominator | CLSpecDenominator,
      UncompetitiveInhibition = KmDenominator | KcatDenominator,
      IrreversibleInhibition = KmNumerator | CLSpecDenominator
   }

   public static class InteractionKineticModificationsExtensions
   {
      public static bool Is(this InteractionKineticModifications modifications, InteractionKineticModifications typeToCompare)
      {
         return (modifications & typeToCompare) != 0;
      }
   }
}