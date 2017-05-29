using System.Collections.Generic;

namespace PKSim.Core.Services
{
   public class IndividualExtractionOptions
   {
      public const string POPULATION_NAME = "{PopulationName}";
      public const string INDIVIDUAL_ID = "{IndividualId}";
      public static readonly string DEFAULT_NAMING_PATTERN = $"{POPULATION_NAME}-{INDIVIDUAL_ID}";

      public IEnumerable<int> IndividualIds { get; set; } = new List<int>();
      public string NamingPattern { get; set; } = DEFAULT_NAMING_PATTERN;

      /// <summary>
      /// Generates an individual name based on the defined <see cref="NamingPattern"/> 
      /// </summary>
      public string GenerateIndividualName(string populationName, int individualId)
      {
         return NamingPattern.Replace(POPULATION_NAME, populationName)
            .Replace(INDIVIDUAL_ID, individualId.ToString());
      }
   }
}