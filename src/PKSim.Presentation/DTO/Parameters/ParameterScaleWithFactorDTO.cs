using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Validation;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Parameters
{
   public class ParameterScaleWithFactorDTO : ValidatableDTO
   {
      public double Factor { get; set; }

      public ParameterScaleWithFactorDTO()
      {
         Rules.AddRange(AllRules.All());
      }

      private static class AllRules
      {
         private static IBusinessRule factorBiggerThanZero
         {
            get
            {
               return CreateRule.For<ParameterScaleWithFactorDTO>()
                  .Property(item => item.Factor)
                  .WithRule((param, value) => value > 0)
                  .WithError((param, value) => PKSimConstants.Error.FactorShouldBeBiggerThanZero);
            }
         }

         public static IEnumerable<IBusinessRule> All()
         {
            yield return factorBiggerThanZero;
         }
      }
   }
}