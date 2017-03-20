using OSPSuite.Utility.Validation;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using NUnit.Framework;
using PKSim.Presentation.DTO.PopulationAnalyses;

namespace PKSim.Presentation
{
   public abstract class concern_for_BinSizeGroupingDTO : ContextSpecification<BinSizeGroupingDTO>
   {
      protected override void Context()
      {
         sut = new BinSizeGroupingDTO();
      }
   }

   public class When_checking_if_a_bin_size_grouping_item_is_valid : concern_for_BinSizeGroupingDTO
   {
      [TestCase(2)]
      [TestCase(10)]
      [TestCase(20)]
      [TestCase(30)]
      [TestCase(40)]
      [TestCase(50)]
      [TestCase(60)]
      [TestCase(70)]
      [TestCase(80)]
      [TestCase(90)]
      [TestCase(97)]
      public void should_return_true_for_valid_value(int testValue)
      {
         sut.NumberOfBins = testValue;
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_value_too_low()
      {
         sut.NumberOfBins = 1;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_for_value_too_large()
      {
         sut.NumberOfBins = 98;
         sut.IsValid().ShouldBeFalse();
      }
   }
}