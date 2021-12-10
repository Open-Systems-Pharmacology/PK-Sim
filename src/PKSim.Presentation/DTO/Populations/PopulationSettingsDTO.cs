using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Populations
{
   public class PopulationSettingsDTO : ValidatableDTO
   {
      public virtual uint NumberOfIndividuals { get; set; }

      public virtual int ProportionOfFemales { get; set; }
      public virtual ICache<string, ParameterRangeDTO> Parameters { get; }

      //Based Individual used to create the population
      public virtual Individual Individual { get; set; }

      public PopulationSettingsDTO()
      {
         Parameters = new Cache<string, ParameterRangeDTO>(x => x.ParameterName);
         Rules.AddRange(AllRules.All());
      }

      public virtual IReadOnlyList<Gender> AvailableGenders => Individual?.AvailableGenders ?? new List<Gender>();

      public string DiseaseState
      {
         get
         {
            var diseaseState = Individual?.OriginData?.DiseaseState;
            if (diseaseState == null)
               return string.Empty;
            return diseaseState.DisplayName;
         }
      }

      public virtual string Population
      {
         get
         {
            if (Individual == null)
               return string.Empty;

            return Individual.OriginData.Population.DisplayName;
         }
      }

      public virtual bool HasMultipleGenders => AvailableGenders.Count() > 1;

      public virtual Gender Female => AvailableGenders.FindByName(CoreConstants.Gender.FEMALE);

      public virtual Gender Male => AvailableGenders.FindByName(CoreConstants.Gender.MALE);

      private static class AllRules
      {
         private static IBusinessRule proportionOfFemaleBetween0And100 { get; } = CreateRule.For<PopulationSettingsDTO>()
            .Property(x => x.ProportionOfFemales)
            .WithRule((dto, value) => (value <= 100 && value >= 0))
            .WithError(PKSimConstants.Rules.Parameter.ProportionOfFemaleBetween0And100);

         private static IBusinessRule numberOfIndividualShouldBeBiggerThan2 { get; } = CreateRule.For<PopulationSettingsDTO>()
            .Property(x => x.NumberOfIndividuals)
            .WithRule((dto, value) => (value >= 2))
            .WithError(PKSimConstants.Rules.Parameter.NumberOfIndividualShouldBeBiggerThan2);

         private static IBusinessRule numberOfIndividualShouldBeSmallerThan10000 { get; } = CreateRule.For<PopulationSettingsDTO>()
            .Property(x => x.NumberOfIndividuals)
            .WithRule((dto, value) => (value <= 10000))
            .WithError(PKSimConstants.Rules.Parameter.NumberOfIndividualShouldBeSmallerThan10000);

         public static IEnumerable<IBusinessRule> All()
         {
            yield return proportionOfFemaleBetween0And100;
            yield return numberOfIndividualShouldBeBiggerThan2;
            yield return numberOfIndividualShouldBeSmallerThan10000;
         }
      }
   }
}