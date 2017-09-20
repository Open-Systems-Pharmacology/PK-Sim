using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Assets;
using PKSim.Core.Model;
using IOutputIntervalFactory = PKSim.Core.Model.IOutputIntervalFactory;
using OutputIntervalFactory = PKSim.Core.Model.OutputIntervalFactory;

namespace PKSim.Core
{
   public abstract class concern_for_OutputIntervalFactory : ContextSpecification<IOutputIntervalFactory>
   {
      protected OSPSuite.Core.Domain.IOutputIntervalFactory _outputIntervalFactory;
      protected IContainerTask _containerTask;

      protected override void Context()
      {
         _outputIntervalFactory = A.Fake<OSPSuite.Core.Domain.IOutputIntervalFactory>();
         sut = new OutputIntervalFactory(_outputIntervalFactory);
      }
   }

   public class When_creating_a_default_simulation_interval : concern_for_OutputIntervalFactory
   {
      private OutputInterval _simulationInterval;
      private IParameter _startTimeParameter;
      private IParameter _endTimeParameter;
      private IParameter _numberOfTimePointParameter;
      private OutputInterval _defaultInterval;
      private IDimension _time;
      private IDimension _resolution;

      protected override void Context()
      {
         base.Context();
         _defaultInterval = new OutputInterval();

         A.CallTo(() => _outputIntervalFactory.CreateDefault()).Returns(_defaultInterval);
         _time = A.Fake<IDimension>();
         A.CallTo(() => _time.Name).Returns(Constants.Dimension.TIME);
         _resolution = A.Fake<IDimension>();
         A.CallTo(() => _resolution.Name).Returns(Constants.Dimension.RESOLUTION);
         _startTimeParameter = new PKSimParameter().WithName(Constants.Parameters.START_TIME).WithDimension(_time);
         _endTimeParameter = new PKSimParameter().WithName(Constants.Parameters.END_TIME).WithDimension(_time);
         _numberOfTimePointParameter = new PKSimParameter().WithName(Constants.Parameters.RESOLUTION).WithDimension(_resolution);

         _defaultInterval.Add(_startTimeParameter);
         _defaultInterval.Add(_endTimeParameter);
         _defaultInterval.Add(_numberOfTimePointParameter);
      }

      protected override void Because()
      {
         _simulationInterval = sut.CreateDefault();
      }

      [Observation]
      public void should_return_a_simulation_interval_with_parameters_set_to_the_default_value()
      {
         _simulationInterval.StartTime.ShouldBeEqualTo(_startTimeParameter);
         _simulationInterval.EndTime.ShouldBeEqualTo(_endTimeParameter);
         _simulationInterval.Resolution.ShouldBeEqualTo(_numberOfTimePointParameter);
      }

      [Observation]
      public void should_use_the_default_name()
      {
         _simulationInterval.Name.ShouldBeEqualTo(PKSimConstants.UI.SimulationInterval);
      }
   }
}