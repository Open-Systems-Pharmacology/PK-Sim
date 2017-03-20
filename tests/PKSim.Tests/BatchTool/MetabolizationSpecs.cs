using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.BatchTool
{
   public abstract class concern_for_ProcessesJson : ContextForBatch
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         Load("metabolization");
      }
   }

   public class When_loading_the_simulation_defined_in_the_metabolization_json_file : concern_for_ProcessesJson
   {
      [Observation]
      public void should_have_loaded_one_simulation()
      {
         _simulationForBatch.ParameterVariationSets.Count.ShouldBeEqualTo(0);
         _simulation.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_loaded_one_systemic_process_in_the_compound()
      {
         var compound = _simulation.BuildingBlock<Compound>();
         compound.AllSystemicProcessesOfType(SystemicProcessTypes.Hepatic).Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_loaded_one_partial_enzymatic_process_in_the_compound()
      {
         var compound = _simulation.BuildingBlock<Compound>();
         compound.AllProcesses<EnzymaticProcess>().Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_loaded_the_enzyme_define_in_the_individual()
      {
         var individual = _simulation.BuildingBlock<Individual>();
         var molecule = individual.MoleculeByName<IndividualEnzyme>("CYP");
         molecule.ShouldNotBeNull();
         molecule.ReferenceConcentration.Value.ShouldBeEqualTo(5);
         molecule.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.Pericentral).Value.ShouldBeEqualTo(1);
         molecule.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.Periportal).Value.ShouldBeEqualTo(1);
         molecule.GetRelativeExpressionNormParameterFor("Kidney").Value.ShouldBeEqualTo(0.2);
      }

      [Observation]
      public void should_have_created_one_systemic_liver_clearance()
      {
         var metaSelection = _simulation.CompoundPropertiesList.First().Processes.MetabolizationSelection;
         metaSelection.AllSystemicProcesses().Count().ShouldBeEqualTo(1);
         metaSelection.AllSystemicProcesses().ElementAt(0).ProcessName.ShouldBeEqualTo("Total Hepatic Clearance-Lab");
         metaSelection.AllSystemicProcesses().ElementAt(0).ProcessType.ShouldBeEqualTo(SystemicProcessTypes.Hepatic);
      }

      [Observation]
      public void should_have_created_one_partial_process_for_enzyme_cyp()
      {
         var metaSelection = _simulation.CompoundPropertiesList.First().Processes.MetabolizationSelection;
         metaSelection.AllPartialProcesses().Count().ShouldBeEqualTo(1);
         metaSelection.AllPartialProcesses().ElementAt(0).MoleculeName.ShouldBeEqualTo("CYP");
         metaSelection.AllPartialProcesses().ElementAt(0).ProcessName.ShouldBeEqualTo("CYP-Lab");
      }
   }
}