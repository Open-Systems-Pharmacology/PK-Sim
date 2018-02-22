using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots.Mappers;
using IOutputIntervalFactory = PKSim.Core.Model.IOutputIntervalFactory;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_OutputIntervalMapper : ContextSpecificationAsync<OutputIntervalMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected OutputInterval _outputInterval;
      private IParameter _endTimeParameter;
      protected Snapshots.OutputInterval _snapshot;
      protected Parameter _endTimeSnapshotParameter;
      protected Parameter _anotherSnapshotParameter;
      protected IOutputIntervalFactory _outputIntervalFactory;
      private IParameter _anotherParameter;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _outputIntervalFactory = A.Fake<IOutputIntervalFactory>();
         sut = new OutputIntervalMapper(_parameterMapper, _outputIntervalFactory);

         _outputInterval = new OutputInterval
         {
            Name = "Interval"
         };
         _endTimeParameter = DomainHelperForSpecs.ConstantParameterWithValue(1, isDefault:true).WithName(Constants.Parameters.END_TIME);
         _anotherParameter = DomainHelperForSpecs.ConstantParameterWithValue(1, isDefault:false).WithName("Another");
         _outputInterval.Add(_endTimeParameter);
         _outputInterval.Add(_anotherParameter);

         _endTimeSnapshotParameter = new Parameter().WithName(_endTimeParameter.Name);
         _anotherSnapshotParameter = new Parameter().WithName(_anotherParameter.Name);
         A.CallTo(() => _parameterMapper.MapToSnapshot(_endTimeParameter)).Returns(_endTimeSnapshotParameter);
         A.CallTo(() => _parameterMapper.MapToSnapshot(_anotherParameter)).Returns(_anotherSnapshotParameter);

         return _completed;
      }
   }

   public class When_mapping_an_output_interval_to_snapshot : concern_for_OutputIntervalMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_outputInterval);
      }

      [Observation]
      public void should_return_a_snapshot_with_all_changed_parameters_mapped_as_expected()
      {
         _snapshot.Parameters.ShouldContain(_anotherSnapshotParameter);
      }

      [Observation]
      public void should_return_a_snapshot_with_all_start_time_and_end_time_parameter_exported()
      {
         _snapshot.Parameters.ShouldContain(_endTimeSnapshotParameter);
      }

      [Observation]
      public void should_have_reset_the_name_as_it_is_set_automatically()
      {
         _snapshot.Name.ShouldBeNull();
      }
   }

   public class When_mapping_output_interval_snapshot_to_output_interval : concern_for_OutputIntervalMapper
   {
      private IParameter _intervalParameter;
      private OutputInterval _newInteval;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_outputInterval);
         _intervalParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1");
         var interval = new OutputInterval {_intervalParameter};

         A.CallTo(() => _outputIntervalFactory.CreateDefault()).Returns(interval);
      }

      protected override async Task Because()
      {
        _newInteval= await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_return_a_new_interval_with_updated_parameters()
      {
         A.CallTo(() => _parameterMapper.MapParameters(_snapshot.Parameters, _newInteval, Constants.OUTPUT_INTERVAL)).MustHaveHappened();
      }
   }
}