using System.Collections.Generic;
using System.Linq;

namespace PKSim.Core.Model
{
   public static class ParameterDistributionMetaDataExtensions
   {
      public static IReadOnlyList<ParameterDistributionMetaData> DefinedFor(this IEnumerable<ParameterDistributionMetaData> allMetaData, OriginData originData)
      {
         var allValidMetaData = new List<ParameterDistributionMetaData>();
         foreach (var metaDataForParameter in allMetaData.GroupBy(x => x.ParameterName))
         {
            var allNonPreterm = metaDataForParameter.allForGA(CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS).ToList();

            if (!isRealPreterm(originData))
               allValidMetaData.AddRange(allNonPreterm);
            else
            {
               var allForGestationalAge = metaDataForParameter.allForGA(originData.GestationalAge).ToList();
            
               if (allForGestationalAge.Any())
               {
                  allValidMetaData.AddRange(allForGestationalAge);
                  allValidMetaData.AddRange(addMissingAgeForGA(allNonPreterm,allForGestationalAge));
               }
               else
                  allValidMetaData.AddRange(allNonPreterm);

            }
         }

         return allValidMetaData;
      }

      private static IReadOnlyList<ParameterDistributionMetaData> addMissingAgeForGA(IReadOnlyList<ParameterDistributionMetaData> allNonPreterm, IReadOnlyList<ParameterDistributionMetaData> allForGestationalAge)
      {
         var allMissing = new List<ParameterDistributionMetaData>();
         var allAvailableAgesForGA = allForGestationalAge.GroupBy(x => x.Age).Select(x => x.Key).ToList();

         foreach (var metaData in allNonPreterm)
         {
            if (!allAvailableAgesForGA.Contains(metaData.Age))
               allMissing.Add(metaData);
         }
         return allMissing;
      }

      private static bool isRealPreterm(OriginData originData)
      {
         return originData.GestationalAge.GetValueOrDefault(CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS) < CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS;
      }

      private static IEnumerable<ParameterDistributionMetaData> allForGA(this IEnumerable<ParameterDistributionMetaData> allMetaData, double? gestationalAge)
      {
         return allMetaData.Where(p => p.GestationalAge == gestationalAge.GetValueOrDefault(CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS));
      }
   }
}