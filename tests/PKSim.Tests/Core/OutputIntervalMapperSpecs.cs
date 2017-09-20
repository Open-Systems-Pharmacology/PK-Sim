using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using IOutputIntervalFactory = PKSim.Core.Model.IOutputIntervalFactory;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_OutputIntervalMapper : ContextSpecificationAsync<OutputIntervalMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected OutputInterval _outputInterval;
      private IParameter _parameter1;
      protected Snapshots.OutputInterval _snapshot;
      protected Parameter _snapshotParameter;
      protected IOutputIntervalFactory _outputIntervalFactory;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _outputIntervalFactory = A.Fake<IOutputIntervalFactory>();
         sut = new OutputIntervalMapper(_parameterMapper, _outputIntervalFactory);

         _outputInterval = new OutputInterval
         {
            Name = "Interval"
         };
         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1");
         _outputInterval.Add(_parameter1);
         _snapshotParameter = new Parameter().WithName(_parameter1.Name);
         A.CallTo(() => _parameterMapper.MapToSnapshot(_parameter1)).ReturnsAsync(_snapshotParameter);

         return Task.FromResult(true);
      }
   }

   public class When_mapping_an_output_interval_to_snapshot : concern_for_OutputIntervalMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_outputInterval);
      }

      [Observation]
      public void should_return_a_snapshot_with_all_parameters_mapped_as_expected()
      {
         _snapshot.Parameters.ShouldContain(_snapshotParameter);
      }

      [Observation]
      public void should_have_reset_the_name_as_it_is_set_automatically()
      {
         _snapshot.Name.ShouldBeNull();
      }
   }

   public class When_mapping_output_interval_snapshot_to_output_interval : concern_for_OutputIntervalMapper
   {
      private OutputSchema _outputSchema;
      private OutputInterval _newInterval;
      private IParameter _intervalParameter;

      protected override async Task Context()
      {
         await base.Context();
         _outputSchema = new OutputSchema();
         _snapshot = await sut.MapToSnapshot(_outputInterval);
         _intervalParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1");
         var interval = new OutputInterval {_intervalParameter};

         A.CallTo(() => _outputIntervalFactory.CreateDefaultFor(_outputSchema)).Returns(interval);
      }

      protected override async Task Because()
      {
         _newInterval = await sut.MapToModel(_snapshot, _outputSchema);
      }

      [Observation]
      public void should_return_a_new_interval_with_updated_parameters()
      {
         A.CallTo(() => _parameterMapper.MapToModel(_snapshotParameter,_intervalParameter)).MustHaveHappened();
      }
   }
}