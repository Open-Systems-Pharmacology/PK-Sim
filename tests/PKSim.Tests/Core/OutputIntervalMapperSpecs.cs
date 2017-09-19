using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_OutputIntervalMapper : ContextSpecificationAsync<OutputIntervalMapper>
   {
      private ParameterMapper _parameterMapper;
      protected OutputInterval _outputInterval;
      private IParameter _parameter1;
      protected Snapshots.OutputInterval _snapshot;
      protected Parameter _snapshotParameter;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         sut = new OutputIntervalMapper(_parameterMapper);

         _outputInterval = new OutputInterval
         {
            Name = "Interval"
         };
         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1");
         _outputInterval.Add(_parameter1);
         _snapshotParameter = new Parameter();
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
}