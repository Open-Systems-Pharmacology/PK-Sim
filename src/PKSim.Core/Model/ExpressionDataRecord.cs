namespace PKSim.Core.Model
{
   /// <summary>
   ///    One expression value of a gene variant retrieved from a gene expression database
   /// </summary>
   public class ExpressionDataRecord
   {
      public const string UNSPECIFIED_AGE = "UNSPECIFIED";

      public string VariantName { get; set; }
      public string Database { get; set; }
      public string DatabaseRecId { get; set; }
      public string Gender { get; set; }
      public string Tissue { get; set; }
      public string HealthState { get; set; }
      public string SampleSource { get; set; }
      public double? AgeMin { get; set; }
      public double? AgeMax { get; set; }
      public double? SampleCount { get; set; }
      public double? TotalCount { get; set; }
      public double? Ratio { get; set; }

      /// <summary>
      ///    Expression value normalized by the average expression of the unit. This is the value aggregated by a query
      /// </summary>
      public double? NormValue { get; set; }

      public string Unit { get; set; }

      /// <summary>
      ///    Age range of the record used to group and filter records by age
      /// </summary>
      public string Age => AgeFrom(AgeMin, AgeMax);

      public static string AgeFrom(double? ageMin, double? ageMax)
      {
         var ageMinValue = ageMin?.ToString() ?? string.Empty;
         var ageMaxValue = ageMax?.ToString() ?? string.Empty;
         var age = ageMinValue == ageMaxValue ? ageMinValue : $"{ageMinValue} - {ageMaxValue}";
         return string.IsNullOrEmpty(age) ? UNSPECIFIED_AGE : age;
      }
   }
}
