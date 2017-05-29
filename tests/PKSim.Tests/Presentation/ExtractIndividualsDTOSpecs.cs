using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Presentation.DTO.Populations;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExtractIndividualsDTO : ContextSpecification<ExtractIndividualsDTO>
   {
      private int _numberOfIndividuals;

      protected override void Context()
      {
         _numberOfIndividuals = 50;
         sut = new ExtractIndividualsDTO(_numberOfIndividuals);
      }
   }

   public class When_retrieving_the_individual_ids_expression : concern_for_ExtractIndividualsDTO
   {
      [Observation]
      [TestCase("1", 1)]
      [TestCase("  1, 2", 2)]
      [TestCase("1, 2,3, 2  ", 3)]
      [TestCase("1, 2,300,  ", 2)]
      [TestCase("x, 45646,300,  x", 0)]
      public void should_return_the_expected_number_of_individuals(string individualIds, int count)
      {
         sut.IndividualIdsExpression = individualIds;
         sut.Count.ShouldBeEqualTo(count);
      }
   }

   public class When_retrieving_the_individual_ids_from_the_individual_ids_expression : concern_for_ExtractIndividualsDTO
   {
      [Observation]
      [TestCase("1, 2,3, 2  ", new[]{1,2,3})]
      [TestCase("1, 2,300,  ", new []{1,2})]
      public void should_return_the_expected_ids(string individualIds, IEnumerable<int> parsedIds)
      {
         sut.IndividualIdsExpression = individualIds;
         sut.IndividualIds.ShouldOnlyContainInOrder(parsedIds);
      }
   }

   public class When_validating_the_extract_indiviudal_dto : concern_for_ExtractIndividualsDTO
   {
      [Observation]
      public void should_return_valid_if_the_naming_pattern_and_individual_ids_expression_is_set_and_valid()
      {
         sut.IndividualIdsExpression = "1,2";
         sut.NamingPattern = "TOTO";
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_return_invalid_if_the_naming_pattern_is_not_set()
      {
         sut.IndividualIdsExpression = "1,2";
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_return_invalid_if_the_number_of_individuals_resulting_from_parsing_the_individual_ids_expression_is_zero()
      {
         sut.NamingPattern = "TOTO";
         sut.IndividualIdsExpression = "456464";
         sut.IsValid().ShouldBeFalse();

         sut.IndividualIdsExpression = "";
         sut.IsValid().ShouldBeFalse();

         sut.IndividualIdsExpression = "xxx,46546";
         sut.IsValid().ShouldBeFalse();
      }
   }
}	