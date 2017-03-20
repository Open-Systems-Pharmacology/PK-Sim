using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.BatchTool
{
   public abstract class concern_for_SpecificBindingJson : ContextForBatch
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         Load("specific_binding");
      }
   }

   public class When_loading_the_simulation_defined_in_the_specific_binding_json_file : concern_for_SpecificBindingJson
   {
      [Observation]
      public void should_have_loaded_two_systemic_binding_process_in_the_compound()
      {
         var compound = _simulation.BuildingBlock<Compound>();
         compound.AllProcesses<SpecificBindingPartialProcess>().Count().ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_have_loaded_the_protein_define_in_the_individual()
      {
         var individual = _simulation.BuildingBlock<Individual>();
         var molecule = individual.MoleculeByName<IndividualOtherProtein>("BIND");
         molecule.ShouldNotBeNull();
         molecule.ReferenceConcentration.Value.ShouldBeEqualTo(1);
         molecule.GetRelativeExpressionNormParameterFor("Kidney").Value.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_loaded_the_enzyme_define_in_the_individual()
      {
         var individual = _simulation.BuildingBlock<Individual>();
         var molecule = individual.MoleculeByName<IndividualEnzyme>("CYP");
         molecule.ShouldNotBeNull();
         molecule.ReferenceConcentration.Value.ShouldBeEqualTo(5);
         molecule.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.Pericentral).Value.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_not_have_created_one_metabolization_process_in_liver()
      {
         var metaSelection = _simulation.CompoundPropertiesList.First().Processes.MetabolizationSelection;
         metaSelection.AllPartialProcesses().Count().ShouldBeEqualTo(0);
         metaSelection.AllSystemicProcesses().Count().ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_have_created_two_specific_binding_processes()
      {
         var specificBindingSelection = _simulation.CompoundPropertiesList.First().Processes.SpecificBindingSelection;
         specificBindingSelection.AllPartialProcesses().Count().ShouldBeEqualTo(2);
         specificBindingSelection.AllPartialProcesses().Select(x => x.MoleculeName).ShouldOnlyContain("CYP", "BIND");
         specificBindingSelection.AllPartialProcesses().Select(x => x.ProcessName).ShouldOnlyContain("CYP-Journal", "BIND-Lab");
      }
   }
}