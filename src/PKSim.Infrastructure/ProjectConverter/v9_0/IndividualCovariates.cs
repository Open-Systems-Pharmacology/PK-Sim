using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.ProjectConverter.v9_0
{
   public class IndividualCovariates
   {
      /// <summary>
      ///    List of user defined attributes for the individual (e.g. PopulationName, Genotype etc)
      /// </summary>
      public virtual Cache<string, string> Attributes { get; private set; }

      public virtual Gender Gender { get; set; }
      public virtual SpeciesPopulation Race { get; set; }

      public IndividualCovariates()
      {
         Attributes = new Cache<string, string>(onMissingKey: x => string.Empty);
      }

      public void AddCovariate<T>(string attributeName, T attributeValue)
      {
         if (string.Equals(attributeName, CoreConstants.Covariates.GENDER))
            Gender = attributeValue.DowncastTo<Gender>();

         else if (string.Equals(attributeName, CoreConstants.Covariates.RACE))
            Race = attributeValue.DowncastTo<SpeciesPopulation>();

         else
            Attributes[attributeName] = attributeValue.ToString();
      }

      public string Covariate(string covariateName)
      {
         if (string.Equals(covariateName, CoreConstants.Covariates.GENDER))
            return Gender.DisplayName;

         if (string.Equals(covariateName, CoreConstants.Covariates.RACE))
            return Race.DisplayName;

         if (Attributes.Contains(covariateName))
            return Attributes[covariateName];

         //not found. 
         return PKSimConstants.UI.Unknown;
      }
   }
}