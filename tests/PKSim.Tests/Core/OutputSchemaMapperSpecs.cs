﻿using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_OutputSchemaMapper : ContextSpecificationAsync<OutputSchemaMapper>
   {
      protected OutputIntervalMapper _outputIntervalMapper;
      protected OutputSchema _outputSchema;
      private OutputInterval _outputInterval;
      protected Snapshots.OutputInterval _snapshotInterval;
      protected Snapshots.OutputSchema _snapshot;

      protected override Task Context()
      {
         _outputIntervalMapper = A.Fake<OutputIntervalMapper>();
         sut = new OutputSchemaMapper(_outputIntervalMapper);

         _outputSchema = new OutputSchema();
         _outputInterval = new OutputInterval().WithName("Interval");
         _outputSchema.AddInterval(_outputInterval);
         _snapshotInterval = new Snapshots.OutputInterval();
         A.CallTo(() => _outputIntervalMapper.MapToSnapshot(_outputInterval)).ReturnsAsync(_snapshotInterval);
         return Task.FromResult(true);
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
}