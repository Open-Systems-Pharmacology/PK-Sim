using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using Compound = PKSim.Core.Model.Compound;

namespace PKSim.Core
{
   public abstract class concern_for_ProcessMappingMapper : ContextSpecificationAsync<ProcessMappingMapper>
   {
      protected Compound _compound;
      protected EnzymaticProcessSelection _enzymaticProcessSelection;
      protected ProcessSelection _specificBindingPartialProcessSelection;
      protected SystemicProcessSelection _transportSystemicProcessSelection;
      protected CompoundProcessSelection _snapshot;
      protected InteractionSelection _interactionSelection;
      protected EnzymaticProcess _enzymaticProcess;
      protected SpecificBindingPartialProcess _specificBindingProcess;
      protected SystemicProcess _transportSystemicProcess;
      protected InductionProcess _interactionProcess;

      protected override Task Context()
      {
         sut = new ProcessMappingMapper();

         _compound = new Compound
         {
            Name = "COMP",
         };


         _enzymaticProcess = new EnzymaticProcess().WithName("MetaProcess");
         _enzymaticProcessSelection = new EnzymaticProcessSelection
         {
            CompoundName = _compound.Name,
            MetaboliteName = "META",
            MoleculeName = "CYP",
            ProcessName = _enzymaticProcess.Name
         };


         _specificBindingProcess = new SpecificBindingPartialProcess().WithName("BindingProcess");
         _specificBindingPartialProcessSelection = new ProcessSelection
         {
            CompoundName = _compound.Name,
            MoleculeName = "BINDER",
            ProcessName = _specificBindingProcess.Name
         };

         _transportSystemicProcess = new SystemicProcess
         {
            Name = "SystemicTransport",
            SystemicProcessType = SystemicProcessTypes.GFR
         };

         _transportSystemicProcessSelection = new SystemicProcessSelection
         {
            CompoundName = _compound.Name,
            ProcessName = _transportSystemicProcess.Name,
            ProcessType = _transportSystemicProcess.SystemicProcessType
         };

         _interactionProcess = new InductionProcess().WithName("InteractionProcess");
         _interactionSelection = new InteractionSelection
         {
            CompoundName = _compound.Name,
            ProcessName = _interactionProcess.Name,
            MoleculeName = "INHIBITOR"
         };

         _compound.AddProcess(_enzymaticProcess);
         _compound.AddProcess(_transportSystemicProcess);
         _compound.AddProcess(_interactionProcess);
         _compound.AddProcess(_specificBindingProcess);

         return _completed;
      }
   }

   public class When_mapping_enzymatic_process_mapping_to_snapshot : concern_for_ProcessMappingMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_enzymaticProcessSelection);
      }

      [Observation]
      public void should_return_the_expected_mapping()
      {
         _snapshot.Name.ShouldBeEqualTo(_enzymaticProcessSelection.ProcessName);
         _snapshot.MoleculeName.ShouldBeEqualTo(_enzymaticProcessSelection.MoleculeName);
         _snapshot.MetaboliteName.ShouldBeEqualTo(_enzymaticProcessSelection.MetaboliteName);
      }
   }

   public class When_mapping_specfic_binding_partial_process_mapping_to_snapshot : concern_for_ProcessMappingMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_specificBindingPartialProcessSelection);
      }

      [Observation]
      public void should_return_the_expected_mapping()
      {
         _snapshot.Name.ShouldBeEqualTo(_specificBindingPartialProcessSelection.ProcessName);
         _snapshot.MoleculeName.ShouldBeEqualTo(_specificBindingPartialProcessSelection.MoleculeName);
         _snapshot.MetaboliteName.ShouldBeNull();
      }
   }

   public class When_mapping_transport_systemic_process_mapping_to_snapshot : concern_for_ProcessMappingMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_transportSystemicProcessSelection);
      }

      [Observation]
      public void should_return_the_expected_mapping()
      {
         _snapshot.Name.ShouldBeEqualTo(_transportSystemicProcessSelection.ProcessName);
         _snapshot.MoleculeName.ShouldBeNull();
         _snapshot.MetaboliteName.ShouldBeNull();
      }
   }

   public class When_mapping_interaction_process_mapping_to_snapshot : concern_for_ProcessMappingMapper
   {

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_interactionSelection);
      }

      [Observation]
      public void should_return_the_expected_mapping()
      {
         _snapshot.Name.ShouldBeEqualTo(_interactionSelection.ProcessName);
         _snapshot.MoleculeName.ShouldBeEqualTo(_interactionSelection.MoleculeName);
         _snapshot.CompoundName.ShouldBeEqualTo(_interactionSelection.CompoundName);
         _snapshot.MetaboliteName.ShouldBeNull();
      }
   }

   public class When_mapping_an_enzymatic_partial_process_snapshot_to_process_selection : concern_for_ProcessMappingMapper
   {
      private EnzymaticProcessSelection _processSelection;

      protected override async Task Context()
      {
         await base.Context();
        _snapshot = await sut.MapToSnapshot(_enzymaticProcessSelection);
      }

      protected override async Task Because()
      {
         _processSelection = await sut.MapToModel(_snapshot, _enzymaticProcess) as EnzymaticProcessSelection;
      }

      [Observation]
      public void should_return_a_process_selection_with_all_properties_set()
      {
         _processSelection.ShouldNotBeNull();
         _processSelection.MetaboliteName.ShouldBeEqualTo(_enzymaticProcessSelection.MetaboliteName);
         _processSelection.ProcessName.ShouldBeEqualTo(_enzymaticProcessSelection.ProcessName);
         _processSelection.CompoundName.ShouldBeEqualTo(_enzymaticProcessSelection.CompoundName);
      }
   }

   public class When_mapping_a_specific_binding_partial_process_snapshot_to_process_selection : concern_for_ProcessMappingMapper
   {
      private ProcessSelection _processSelection;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_specificBindingPartialProcessSelection);
      }

      protected override async Task Because()
      {
         _processSelection = await sut.MapToModel(_snapshot, _specificBindingProcess) as ProcessSelection;
      }

      [Observation]
      public void should_return_a_process_selection_with_all_properties_set()
      {
         _processSelection.ShouldNotBeNull();
         _processSelection.ProcessName.ShouldBeEqualTo(_specificBindingPartialProcessSelection.ProcessName);
         _processSelection.CompoundName.ShouldBeEqualTo(_specificBindingPartialProcessSelection.CompoundName);
      }
   }

   public class When_mapping_a_transport_systemic_process_snapshot_to_process_selection : concern_for_ProcessMappingMapper
   {
      private SystemicProcessSelection _processSelection;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_transportSystemicProcessSelection);
      }

      protected override async Task Because()
      {
         _processSelection = await sut.MapToModel(_snapshot, _transportSystemicProcess) as SystemicProcessSelection;
      }

      [Observation]
      public void should_return_a_process_selection_with_all_properties_set()
      {
         _processSelection.ShouldNotBeNull();
         _processSelection.ProcessName.ShouldBeEqualTo(_transportSystemicProcessSelection.ProcessName);
         _processSelection.CompoundName.ShouldBeEqualTo(_transportSystemicProcessSelection.CompoundName);
         _processSelection.ProcessType.ShouldBeEqualTo(_transportSystemicProcess.SystemicProcessType);
      }
   }

   public class When_mapping_an_interaction_partial_process_snapshot_to_process_selection : concern_for_ProcessMappingMapper
   {
      private InteractionSelection _processSelection;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_interactionSelection);
      }

      protected override async Task Because()
      {
         _processSelection = await sut.MapToModel(_snapshot, _interactionProcess) as InteractionSelection;
      }

      [Observation]
      public void should_return_a_process_selection_with_all_properties_set()
      {
         _processSelection.ShouldNotBeNull();
         _processSelection.ProcessName.ShouldBeEqualTo(_interactionSelection.ProcessName);
         _processSelection.CompoundName.ShouldBeEqualTo(_interactionSelection.CompoundName);
         _processSelection.MoleculeName.ShouldBeEqualTo(_interactionSelection.MoleculeName);
      }
   }
}
