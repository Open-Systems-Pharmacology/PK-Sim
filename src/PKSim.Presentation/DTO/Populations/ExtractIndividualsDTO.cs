using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.DTO.Populations
{
   public class ExtractIndividualsDTO : DxValidatableDTO
   {
      private readonly int _numberOfIndividuals;
      private string _individualIdsExpression;
      private string _nanmingPattern;
      public const char ID_SEPARATOR = ',';

      public ExtractIndividualsDTO(int numberOfIndividuals)
      {
         _numberOfIndividuals = numberOfIndividuals;
         Rules.AddRange(AllRules.All());
      }

      public string IndividualIdsExpression
      {
         get => _individualIdsExpression;
         set
         {
            _individualIdsExpression = value;
            OnPropertyChanged();
            OnPropertyChanged(()=>Count);
         }
      }

      public string NamingPattern
      {
         get => _nanmingPattern;
         set
         {
            _nanmingPattern = value;
            OnPropertyChanged();
         }
      }

      public int Count => CountFor(IndividualIdsExpression);

      public IReadOnlyList<int> ParseIndividualIds(string individualIdsExpression)
      {
         if(string.IsNullOrEmpty(individualIdsExpression))
            return new List<int>();

         return individualIdsExpression.Trim().Split(ID_SEPARATOR)
            .Select(parseIndividualId)
            .Where(id => id != null)
            .Select(id => id.Value)
            .Distinct()
            .ToList();
      }

      public IEnumerable<int> IndividualIds
      {
         get => ParseIndividualIds(IndividualIdsExpression);
         set => IndividualIdsExpression = value?.ToString(ID_SEPARATOR.ToString()) ?? string.Empty;
      }

      public int CountFor(string individualIds)
      {
         return ParseIndividualIds(individualIds).Count;
      }

      private int? parseIndividualId(string individualId)
      {
         if (string.IsNullOrEmpty(individualId))
            return null;

         if (!int.TryParse(individualId.Trim(), out int res))
            return null;

         if (res >= _numberOfIndividuals)
            return null;

         return res;
      }

      private static class AllRules
      {
         private static IBusinessRule namingPatternDefined { get; } = GenericRules.NonEmptyRule<ExtractIndividualsDTO>(x => x.NamingPattern);
         private static IBusinessRule indiviudalIdsDefined { get; } = GenericRules.NonEmptyRule<ExtractIndividualsDTO>(x => x.IndividualIdsExpression);

         private static IBusinessRule indiviudalIdsWellFormatted
         {
            get
            {
               return CreateRule.For<ExtractIndividualsDTO>()
                  .Property(dto => dto.IndividualIdsExpression)
                  .WithRule((dto, individualIds) => dto.CountFor(individualIds) > 0)
                  .WithError((dto, individualIds) => PKSimConstants.Error.AtLeastOneIndividualIdRequiredTOPerformPopulationExtraction);
            }
         }

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return namingPatternDefined;
            yield return indiviudalIdsDefined;
            yield return indiviudalIdsWellFormatted;
         }
      }
   }
}