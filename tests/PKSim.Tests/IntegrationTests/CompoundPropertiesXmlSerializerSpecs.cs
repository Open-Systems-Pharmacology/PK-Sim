using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.IntegrationTests
{
   public class When_serializing_some_compound_properties : ContextForSerialization<CompoundProperties>
   {
      private CompoundProperties _compoundProperties;
      private CompoundProperties _deserializedProperties;

      protected override void Context()
      {
         base.Context();
         _compoundProperties = new CompoundProperties();
         _compoundProperties.Processes.MetabolizationSelection.AddPartialProcessSelection(new ProcessSelection { MoleculeName = "M1", ProcessName = "Proc1" });
         _compoundProperties.Processes.TransportAndExcretionSelection.AddPartialProcessSelection(new ProcessSelection { MoleculeName = "M2", ProcessName = "Proc2" });
         _compoundProperties.Processes.SpecificBindingSelection.AddPartialProcessSelection(new ProcessSelection { MoleculeName = "M3", ProcessName = "Proc3" });

         _compoundProperties.AddCompoundGroupSelection(new CompoundGroupSelection {AlternativeName = "Alt", GroupName = "Group"});
         _compoundProperties.OverwriteParameterSetName = "Renal impairment";
      }

      protected override void Because()
      {
         _deserializedProperties = SerializeAndDeserialize(_compoundProperties);
      }

      [Observation]
      public void should_be_able_to_restore_the_properties_with_the_same_values()
      {
         var processSelection = getFirstPartialProcessFor(_deserializedProperties.Processes.MetabolizationSelection);
         processSelection.MoleculeName.ShouldBeEqualTo("M1");
         processSelection.ProcessName.ShouldBeEqualTo("Proc1");

         processSelection = getFirstPartialProcessFor(_deserializedProperties.Processes.TransportAndExcretionSelection);
         processSelection.MoleculeName.ShouldBeEqualTo("M2");
         processSelection.ProcessName.ShouldBeEqualTo("Proc2");

         processSelection = getFirstPartialProcessFor(_deserializedProperties.Processes.SpecificBindingSelection);
         processSelection.MoleculeName.ShouldBeEqualTo("M3");
         processSelection.ProcessName.ShouldBeEqualTo("Proc3");

         _deserializedProperties.CompoundGroupSelections.ElementAt(0).AlternativeName.ShouldBeEqualTo("Alt");
         _deserializedProperties.CompoundGroupSelections.ElementAt(0).GroupName.ShouldBeEqualTo("Group");
      }

      [Observation]
      public void should_restore_the_name_of_the_selected_overwrite_parameter_set()
      {
         _deserializedProperties.OverwriteParameterSetName.ShouldBeEqualTo("Renal impairment");
      }

      private ProcessSelection getFirstPartialProcessFor(ProcessSelectionGroup processSelectionGroup)
      {
         return processSelectionGroup.AllPartialProcesses().ElementAt(0);
      }
   }
}