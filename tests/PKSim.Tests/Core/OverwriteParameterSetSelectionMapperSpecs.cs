using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using ModelOverwriteParameterSetSelection = PKSim.Core.Model.OverwriteParameterSetSelection;
using SnapshotOverwriteParameterSetSelection = PKSim.Core.Snapshots.OverwriteParameterSetSelection;

namespace PKSim.Core
{
   public abstract class concern_for_OverwriteParameterSetSelectionMapper : ContextSpecificationAsync<OverwriteParameterSetSelectionMapper>
   {
      protected IOSPSuiteLogger _logger;
      protected PKSimProject _project;
      protected Compound _compound;
      protected OverwriteParameterSet _overwriteParameterSet;
      protected OverwriteParameterSet _overwriteParameterSetInSimulation;
      protected SnapshotContextWithSimulation _snapshotContext;

      protected override Task Context()
      {
         _logger = A.Fake<IOSPSuiteLogger>();

         sut = new OverwriteParameterSetSelectionMapper(_logger);

         _overwriteParameterSet = new OverwriteParameterSet { Name = "MySet" };
         _compound = new Compound { Name = "Aspirin" };
         _compound.AddOverwriteParameterSet(_overwriteParameterSet);

         _project = new PKSimProject();
         _project.AddBuildingBlock(_compound);

         _overwriteParameterSetInSimulation = new OverwriteParameterSet { Name = _overwriteParameterSet.Name };
         var compoundInSimulation = new Compound { Name = _compound.Name };
         compoundInSimulation.AddOverwriteParameterSet(_overwriteParameterSetInSimulation);
         var simulation = new IndividualSimulation();
         simulation.AddUsedBuildingBlock(new UsedBuildingBlock(_compound.Id, PKSimBuildingBlockType.Compound) { BuildingBlock = compoundInSimulation, Name = compoundInSimulation.Name });

         _snapshotContext = new SnapshotContextWithSimulation(simulation, new SnapshotContext(_project, SnapshotVersions.Current));

         return _completed;
      }
   }

   public class When_round_tripping_an_overwrite_parameter_set_selection_through_snapshot : concern_for_OverwriteParameterSetSelectionMapper
   {
      private ModelOverwriteParameterSetSelection _original;
      private SnapshotOverwriteParameterSetSelection _snapshot;
      private ModelOverwriteParameterSetSelection _result;

      protected override async Task Context()
      {
         await base.Context();
         _original = new ModelOverwriteParameterSetSelection
         {
            CompoundName = _compound.Name,
            OverwriteParameterSet = _overwriteParameterSet
         };

         _snapshot = await sut.MapToSnapshot(_original, _project);
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot, _snapshotContext);
      }

      [Observation]
      public void should_restore_the_compound_name()
      {
         _result.CompoundName.ShouldBeEqualTo(_compound.Name);
      }

      [Observation]
      public void should_resolve_the_overwrite_parameter_set_from_the_compound_used_in_the_simulation()
      {
         _result.OverwriteParameterSet.ShouldBeEqualTo(_overwriteParameterSetInSimulation);
      }
   }

   public class When_mapping_a_selection_to_snapshot : concern_for_OverwriteParameterSetSelectionMapper
   {
      private ModelOverwriteParameterSetSelection _selection;
      private SnapshotOverwriteParameterSetSelection _snapshot;

      protected override async Task Context()
      {
         await base.Context();
         _selection = new ModelOverwriteParameterSetSelection
         {
            CompoundName = _compound.Name,
            OverwriteParameterSet = _overwriteParameterSet
         };
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_selection, _project);
      }

      [Observation]
      public void should_map_the_compound_name()
      {
         _snapshot.CompoundName.ShouldBeEqualTo(_compound.Name);
      }

      [Observation]
      public void should_map_the_overwrite_parameter_set_name()
      {
         _snapshot.OverwriteParameterSetName.ShouldBeEqualTo(_overwriteParameterSet.Name);
      }
   }

   public class When_mapping_a_snapshot_for_a_compound_not_used_in_the_simulation_to_model : concern_for_OverwriteParameterSetSelectionMapper
   {
      private ModelOverwriteParameterSetSelection _result;
      private Compound _compoundNotUsedInSimulation;

      protected override async Task Context()
      {
         await base.Context();
         _compoundNotUsedInSimulation = new Compound { Name = "Ibuprofen" };
         _compoundNotUsedInSimulation.AddOverwriteParameterSet(new OverwriteParameterSet { Name = _overwriteParameterSet.Name });
         _project.AddBuildingBlock(_compoundNotUsedInSimulation);
      }

      protected override async Task Because()
      {
         var snapshot = new SnapshotOverwriteParameterSetSelection
         {
            CompoundName = _compoundNotUsedInSimulation.Name,
            OverwriteParameterSetName = _overwriteParameterSet.Name
         };

         _result = await sut.MapToModel(snapshot, _snapshotContext);
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }

      [Observation]
      public void should_not_log_an_error()
      {
         A.CallTo(() => _logger.AddToLog(A<string>._, LogLevel.Error, A<string>._)).MustNotHaveHappened();
      }
   }

   public class When_round_tripping_a_selection_explicitly_set_to_none_through_snapshot : concern_for_OverwriteParameterSetSelectionMapper
   {
      private SnapshotOverwriteParameterSetSelection _snapshot;
      private ModelOverwriteParameterSetSelection _result;

      protected override async Task Context()
      {
         await base.Context();
         var original = new ModelOverwriteParameterSetSelection { CompoundName = _compound.Name };
         _snapshot = await sut.MapToSnapshot(original, _project);
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot, _snapshotContext);
      }

      [Observation]
      public void should_restore_the_selection_without_a_set()
      {
         _result.CompoundName.ShouldBeEqualTo(_compound.Name);
         _result.OverwriteParameterSet.ShouldBeNull();
      }

      [Observation]
      public void should_not_log_an_error()
      {
         A.CallTo(() => _logger.AddToLog(A<string>._, LogLevel.Error, A<string>._)).MustNotHaveHappened();
      }
   }

   public class When_mapping_a_snapshot_with_a_missing_overwrite_parameter_set_to_model : concern_for_OverwriteParameterSetSelectionMapper
   {
      private ModelOverwriteParameterSetSelection _result;

      protected override async Task Because()
      {
         var snapshot = new SnapshotOverwriteParameterSetSelection
         {
            CompoundName = _compound.Name,
            OverwriteParameterSetName = "DoesNotExist"
         };

         _result = await sut.MapToModel(snapshot, _snapshotContext);
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }

      [Observation]
      public void should_log_an_error()
      {
         A.CallTo(() => _logger.AddToLog(A<string>._, LogLevel.Error, A<string>._)).MustHaveHappened();
      }
   }
}
