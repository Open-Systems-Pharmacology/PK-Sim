using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
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

      protected override Task Context()
      {
         _logger = A.Fake<IOSPSuiteLogger>();

         sut = new OverwriteParameterSetSelectionMapper(_logger);

         _overwriteParameterSet = new OverwriteParameterSet { Name = "MySet" };
         _compound = new Compound { Name = "Aspirin" };
         _compound.AddOverwriteParameterSet(_overwriteParameterSet);

         _project = new PKSimProject();
         _project.AddBuildingBlock(_compound);

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
         _result = await sut.MapToModel(_snapshot, new SnapshotContext(_project, SnapshotVersions.Current));
      }

      [Observation]
      public void should_restore_the_compound_name()
      {
         _result.CompoundName.ShouldBeEqualTo(_compound.Name);
      }

      [Observation]
      public void should_resolve_the_overwrite_parameter_set_from_the_compound()
      {
         _result.OverwriteParameterSet.ShouldBeEqualTo(_overwriteParameterSet);
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

   public class When_mapping_a_snapshot_with_a_missing_compound_to_model : concern_for_OverwriteParameterSetSelectionMapper
   {
      private ModelOverwriteParameterSetSelection _result;

      protected override async Task Because()
      {
         var snapshot = new SnapshotOverwriteParameterSetSelection
         {
            CompoundName = "DoesNotExist",
            OverwriteParameterSetName = _overwriteParameterSet.Name
         };

         _result = await sut.MapToModel(snapshot, new SnapshotContext(_project, SnapshotVersions.Current));
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }

      [Observation]
      public void should_log_a_warning()
      {
         A.CallTo(() => _logger.AddToLog(A<string>._, LogLevel.Warning, A<string>._)).MustHaveHappened();
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

         _result = await sut.MapToModel(snapshot, new SnapshotContext(_project, SnapshotVersions.Current));
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }
   }
}
