using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.IntegrationTests
{
   public class When_serializing_overwrite_parameter_set_selections_holding_a_selection_without_a_set : ContextForSerialization<OverwriteParameterSetSelections>
   {
      private OverwriteParameterSetSelections _selections;
      private OverwriteParameterSetSelections _deserializedSelections;

      protected override void Context()
      {
         base.Context();
         _selections = new OverwriteParameterSetSelections();
         _selections.SetSelectionForCompound("Aspirin", null);
      }

      protected override void Because()
      {
         _deserializedSelections = SerializeAndDeserialize(_selections);
      }

      [Observation]
      public void should_restore_the_selection_for_the_compound()
      {
         _deserializedSelections.HasSelectionFor("Aspirin").ShouldBeTrue();
      }

      [Observation]
      public void should_restore_the_selection_without_a_set()
      {
         _deserializedSelections.SelectedSetFor("Aspirin").ShouldBeNull();
      }
   }
}
