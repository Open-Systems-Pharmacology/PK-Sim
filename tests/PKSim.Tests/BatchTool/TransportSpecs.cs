using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.BatchTool
{
   public abstract class concern_for_TransportJson : ContextForBatch
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         Load("transport");
      }
   }

   public class When_loading_the_simulation_defined_in_the_transport_json_file : concern_for_TransportJson
   {
      [Observation]
      public void should_have_loaded_one_systemic_process_in_the_compound()
      {
         var compound = _simulation.BuildingBlock<Compound>();
         compound.AllSystemicProcessesOfType(SystemicProcessTypes.Hepatic).Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_loaded_one_partial_transport_process_in_the_compound()
      {
         var compound = _simulation.BuildingBlock<Compound>();
         compound.AllProcesses<TransportPartialProcess>().Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_loaded_the_transporter_define_in_the_individual()
      {
         var individual = _simulation.BuildingBlock<Individual>();
         var molecule = individual.MoleculeByName<IndividualTransporter>("TRANS");
         molecule.ShouldNotBeNull();
         molecule.ReferenceConcentration.Value.ShouldBeEqualTo(5);
         molecule.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.Pericentral).Value.ShouldBeEqualTo(1);
         molecule.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.Periportal).Value.ShouldBeEqualTo(1);
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
      public void should_have_created_one_transport_process_for_transporter_TRANS()
      {
         var transporter = _simulation.CompoundPropertiesList.First().Processes.TransportAndExcretionSelection;
         transporter.AllPartialProcesses().Count().ShouldBeEqualTo(1);
         transporter.AllPartialProcesses().ElementAt(0).MoleculeName.ShouldBeEqualTo("TRANS");
         transporter.AllPartialProcesses().ElementAt(0).ProcessName.ShouldBeEqualTo("TRANS-Lab");
      }
   }
}