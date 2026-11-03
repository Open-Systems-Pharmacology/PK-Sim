using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;

namespace PKSim.Core
{
   /// <summary>
   ///    Records whose expected expressions were characterized against the pivot grid and chart that computed the relative
   ///    expressions before the query was independent of the view
   /// </summary>
   public static class ExpressionQueryForSpecs
   {
      public const string EST = "EST";
      public const string ARRAY_EXPRESS = "ArrayExpress";
      public const string RT_PCR = "RT-PCR";
      public const string MALE = "MALE";
      public const string FEMALE = "FEMALE";

      public static IReadOnlyList<ExpressionContainerInfo> Containers() => new[]
      {
         new ExpressionContainerInfo("Liver", "Liver D", 0.5),
         new ExpressionContainerInfo("Kidney", "Kidney D", 0.4),
         new ExpressionContainerInfo("Duodenum", "Duodenum D", 0.3),
         new ExpressionContainerInfo("Jejunum", "Jejunum D", 0.2),
         new ExpressionContainerInfo("Brain", "Brain D", 0.1)
      };

      public static IReadOnlyList<TissueContainerMapping> Mapping() => new[]
      {
         new TissueContainerMapping("liver", "Liver"),
         new TissueContainerMapping("kidney", "Kidney"),
         new TissueContainerMapping("intestine", "Duodenum"),
         new TissueContainerMapping("intestine", "Jejunum"),
         new TissueContainerMapping("brain", "Brain"),
         new TissueContainerMapping("muscle", "Muscle"),
         new TissueContainerMapping("skin", null)
      };

      public static ExpressionDataRecord Record(string database, string gender, string tissue, double? ageMin, double? ageMax, double? normValue, string unit) => new ExpressionDataRecord
      {
         VariantName = "V1",
         Database = database,
         DatabaseRecId = $"{database}_{tissue}_{normValue}",
         Gender = gender,
         Tissue = tissue,
         HealthState = "Healthy",
         SampleSource = "Normal",
         AgeMin = ageMin,
         AgeMax = ageMax,
         SampleCount = 1,
         TotalCount = 10,
         Ratio = 0.1,
         NormValue = normValue,
         Unit = unit
      };

      public static IReadOnlyList<ExpressionDataRecord> Records() => new[]
      {
         Record("DB1", MALE, "liver", 20, 30, 1, EST),
         Record("DB1", FEMALE, "liver", 20, 30, 3, EST),
         Record("DB2", MALE, "liver", 40, 40, 8, EST),
         Record("DB1", MALE, "kidney", null, null, 2, EST),
         Record("DB1", FEMALE, "kidney", null, null, null, EST),
         Record("DB2", MALE, "kidney", 20, 30, 4, EST),
         Record("DB1", MALE, "intestine", 20, 30, 6, EST),
         Record("DB1", MALE, "liver", 20, 30, 0, ARRAY_EXPRESS),
         Record("DB1", FEMALE, "kidney", 20, 30, 0, ARRAY_EXPRESS),
         Record("DB1", MALE, "kidney", 20, 30, 5, RT_PCR),
         Record("DB1", MALE, "liver", 40, 40, 10, RT_PCR),
         Record("DB1", MALE, "skin", 20, 30, 100, EST),
         Record("DB1", MALE, "muscle", 20, 30, 100, EST)
      };

      public static ExpressionQuery Query(params ExpressionDataRecord[] additionalRecords) =>
         new ExpressionQuery("CYP3A4", Records().Concat(additionalRecords).ToList(), Mapping());

      public static ExpressionDataFilter FilterOn(string fieldName, ExpressionDataFilterType filterType, bool showBlanks, params object[] values) =>
         new ExpressionDataFilter(new[] {new ExpressionDataFieldFilter(fieldName, filterType, values, showBlanks)});
   }
}
