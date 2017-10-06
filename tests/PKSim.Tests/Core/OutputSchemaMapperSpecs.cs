using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Snapshots.Mappers;
using IOutputSchemaFactory = PKSim.Core.Model.IOutputSchemaFactory;

namespace PKSim.Core
{
   public abstract class concern_for_OutputSchemaMapper : ContextSpecificationAsync<OutputSchemaMapper>
   {
      protected OutputIntervalMapper _outputIntervalMapper;
      protected OutputSchema _outputSchema;
      private OutputInterval _outputInterval;
      protected Snapshots.OutputInterval _snapshotInterval;
      protected Snapshots.OutputSchema _snapshot;
      protected IOutputSchemaFactory _outputSchemaFactory;
      protected IContainerTask _containerTask;

      protected override Task Context()
      {
         _outputIntervalMapper = A.Fake<OutputIntervalMapper>();
         _outputSchemaFactory = A.Fake<IOutputSchemaFactory>();
         _containerTask = A.Fake<IContainerTask>();
         sut = new OutputSchemaMapper(_outputIntervalMapper, _outputSchemaFactory, _containerTask);

         _outputSchema = new OutputSchema();
         _outputInterval = new OutputInterval().WithName("Interval");
         _outputSchema.AddInterval(_outputInterval);
         _snapshotInterval = new Snapshots.OutputInterval();
         A.CallTo(() => _outputIntervalMapper.MapToSnapshot(_outputInterval)).Returns(_snapshotInterval);
         return _completed;
      }
   }

   public class When_mapping_an_output_schema_to_snapshot : concern_for_OutputSchemaMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_outputSchema);
      }

      [Observation]
      public void should_return_a_snaphsot_with_the_expected_intervals()
      {
         _snapshot.ShouldOnlyContain(_snapshotInterval);
      }
   }

   public class When_mapping_a_snapshot_output_schema_to_output_schema : concern_for_OutputSchemaMapper
   {
      private OutputSchema _newOutputSchema;
      private OutputInterval _newInterval;
      private readonly string _newName = "UNIQUE_NAME";

      protected override async Task Context()
      {
         await base.Context();
         var outputSchema = new OutputSchema();
         _newInterval = new OutputInterval().WithName("Interval");
         A.CallTo(() => _outputSchemaFactory.CreateEmpty()).Returns(outputSchema);
         A.CallTo(() => _outputIntervalMapper.MapToModel(_snapshotInterval)).Returns(_newInterval);
         _snapshot = await sut.MapToSnapshot(_outputSchema);
         A.CallTo(() => _containerTask.CreateUniqueName(outputSchema, _newInterval.Name, false)).Returns(_newName);
      }

      protected override async Task Because()
      {
         _newOutputSchema = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_contain_the_expected_intervals()
      {
         _newOutputSchema.Intervals.ShouldOnlyContain(_newInterval);
      }

      [Observation]
      public void should_have_renamed_the_intervals_to_be_unique()
      {
         _newInterval.Name.ShouldBeEqualTo(_newName);
      }
   }
}