using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using IOutputIntervalFactory = PKSim.Core.Model.IOutputIntervalFactory;
using IOutputSchemaFactory = PKSim.Core.Model.IOutputSchemaFactory;
using OutputSchemaFactory = PKSim.Core.Model.OutputSchemaFactory;

namespace PKSim.Core
{
   public abstract class concern_for_OutputSchemaFactory : ContextSpecification<IOutputSchemaFactory>
   {
      protected IObjectBaseFactory _objectBaseFactory;
      protected IOutputIntervalFactory _outputIntervalFactory;
      private IDimensionRepository _dimensionRepository;
      protected IDimension _timeDimension;
      protected Unit _dayUnit;

      protected override void Context()
      {
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _outputIntervalFactory = A.Fake<IOutputIntervalFactory>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _timeDimension = A.Fake<IDimension>();
         _dayUnit = A.Fake<Unit>();
         A.CallTo(() => _dimensionRepository.Time).Returns(_timeDimension);
         A.CallTo(() => _timeDimension.Unit(CoreConstants.Units.Days)).Returns(_dayUnit);
         sut = new OutputSchemaFactory(_objectBaseFactory, _outputIntervalFactory, _dimensionRepository);
      }
   }

   public class When_creating_the_simulation_output_for_a_given_simulation : concern_for_OutputSchemaFactory
   {
      private OutputSchema _output;
      private Simulation _simulation;
      private Protocol _protocol;
      private double _currentEndTimeInMinutes;
      private double _currentEndTimeInHours;
      private OutputInterval _highResolutionInterval;
      private OutputInterval _lowResolutionInterval;
      private readonly double _offsetInMinutes = 100;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         _protocol = A.Fake<Protocol>();
         _simulation.SimulationSettings = null;
         _currentEndTimeInHours = 50;
         _currentEndTimeInMinutes = _currentEndTimeInHours * 60;
         _highResolutionInterval = A.Fake<OutputInterval>();
         _lowResolutionInterval = A.Fake<OutputInterval>();

         A.CallTo(() => _timeDimension.UnitValueToBaseUnitValue(_dayUnit, 1)).Returns(_offsetInMinutes);
         A.CallTo(() => _objectBaseFactory.Create<OutputSchema>()).Returns(new OutputSchema());
         A.CallTo(() => _outputIntervalFactory.Create(0, CoreConstants.HIGH_RESOLUTION_END_TIME_IN_MIN, CoreConstants.HIGH_RESOLUTION_IN_PTS_PER_MIN)).Returns(_highResolutionInterval);
         A.CallTo(() => _outputIntervalFactory.Create(CoreConstants.HIGH_RESOLUTION_END_TIME_IN_MIN, _currentEndTimeInMinutes + _offsetInMinutes, CoreConstants.LOW_RESOLUTION_IN_PTS_PER_MIN)).Returns(_lowResolutionInterval);
         A.CallTo(() => _protocol.EndTime).Returns(_currentEndTimeInMinutes);

         A.CallTo(() => _simulation.AllBuildingBlocks<Protocol>()).Returns(new[] {_protocol});
      }

      protected override void Because()
      {
         _output = sut.CreateFor(_simulation);
      }

      [Observation]
      public void should_add_one_interval_with_high_resolution_for_the_first_two_hours_names_high_resolution_interval()
      {
         var highResolutionInterval = _output.Intervals.First();
         highResolutionInterval.ShouldBeEqualTo(_highResolutionInterval);
         highResolutionInterval.Name.ShouldBeEqualTo(PKSimConstants.UI.SimulationIntervalHighResolution);
      }

      [Observation]
      public void should_add_a_second_interval_with_low_resolution_for_the_last_period_of_time_and_a_end_time_equal_to_the_protocol_end_time_with_24_hours()
      {
         var lowResolution = _output.Intervals.ElementAt(1);
         lowResolution.ShouldBeEqualTo(_lowResolutionInterval);
         lowResolution.Name.ShouldBeEqualTo(PKSimConstants.UI.SimulationIntervalLowResolution);
      }
   }
}