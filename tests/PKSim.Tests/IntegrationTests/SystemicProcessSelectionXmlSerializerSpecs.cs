using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.IntegrationTests
{
   public class When_serializing_a_systemic_process_selection_that_was_saved_in_the_5_0_1_format : ContextForSerialization<SystemicProcessSelection>
   {
      private SystemicProcessSelection _systemicProcessSelection;
      private SystemicProcessSelection _deserializedSystemicProcessSelection;

      protected override void Context()
      {
         base.Context();
         _systemicProcessSelection = new SystemicProcessSelection();
         _systemicProcessSelection.ProcessName = "Liv";
         _systemicProcessSelection.ProcessType = SystemicProcessTypes.Hepatic;
         _systemicProcessSelection.CompoundName = "Comp";
      }

      protected override void Because()
      {
         _deserializedSystemicProcessSelection = SerializeAndDeserialize(_systemicProcessSelection);
      }

      [Observation]
      public void should_have_updated_the_name_to_contain_the_systemic_process_type()
      {
         _deserializedSystemicProcessSelection.ProcessName.ShouldBeEqualTo(SystemicProcessTypes.Hepatic.DisplayName + "-Liv");
      }

      [Observation]
      public void should_have_saved_the_name_of_the_compound()
      {
         _deserializedSystemicProcessSelection.CompoundName.ShouldBeEqualTo(_systemicProcessSelection.CompoundName);
      }
   }

   public class When_serializing_a_systemic_process_selection_that_was_saved_in_the_5_0_1_format_and_that_is_empty : ContextForSerialization<SystemicProcessSelection>
   {
      private SystemicProcessSelection _systemicProcessSelection;
      private SystemicProcessSelection _deserializedSystemicProcessSelection;

      protected override void Context()
      {
         base.Context();
         _systemicProcessSelection = new SystemicProcessSelection();
         _systemicProcessSelection.ProcessName = null;
         _systemicProcessSelection.ProcessType = SystemicProcessTypes.Hepatic;
      }

      protected override void Because()
      {
         _deserializedSystemicProcessSelection = SerializeAndDeserialize(_systemicProcessSelection);
      }

      [Observation]
      public void should_have_left_the_name_empty()
      {
         _deserializedSystemicProcessSelection.ProcessName.ShouldBeNull();
      }
   }
}