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
}