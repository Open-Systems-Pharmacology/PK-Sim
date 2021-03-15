using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_FileSelection : ContextSpecification<FileSelection>
   {
      protected override void Context()
      {
         sut = new FileSelection();
      }
   }

   public class A_file_selection_with_a_file_not_set_is_invalid : concern_for_FileSelection
   {
      [Observation]
      public void Observation()
      {
         sut.IsValid().ShouldBeFalse();
      }
   }

   public class A_file_selection_with_a_file_set_is_valid : concern_for_FileSelection
   {
      [Observation]
      public void Observation()
      {
         sut.FilePath = "a file";
         sut.IsValid().ShouldBeTrue();
      }
   }

   public class When_adding_a_suffix_to_a_file_selection : concern_for_FileSelection
   {
      private FileSelection _withSuffix;

      protected override void Context()
      {
         base.Context();
         sut.FilePath = @"C:\test\toto\sim.csv";
      }
      protected override void Because()
      {
         _withSuffix= sut.AddSuffixToFileName("-aging");
      }

      [Observation]
      public void should_add_the_suffix_to_the_name_and_keep_the_extension()
      {
         _withSuffix.FilePath.ShouldBeEqualTo(@"C:\test\toto\sim-aging.csv");
         sut.FilePath.ShouldBeEqualTo(@"C:\test\toto\sim.csv"); }
   }
}