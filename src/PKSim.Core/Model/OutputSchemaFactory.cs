using PKSim.Assets;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;

namespace PKSim.Core.Model
{
   public interface IOutputSchemaFactory
   {
      OutputSchema Create();
      OutputSchema CreateFor(Simulation simulation);
   }

   public class OutputSchemaFactory : IOutputSchemaFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IOutputIntervalFactory _outputIntervalFactory;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IDisplayUnitRetriever _displayUnitRetriever;

      public OutputSchemaFactory(IObjectBaseFactory objectBaseFactory, IOutputIntervalFactory outputIntervalFactory,
         IDimensionRepository dimensionRepository, IDisplayUnitRetriever displayUnitRetriever)
      {
         _objectBaseFactory = objectBaseFactory;
         _outputIntervalFactory = outputIntervalFactory;
         _dimensionRepository = dimensionRepository;
         _displayUnitRetriever = displayUnitRetriever;
      }

      public OutputSchema Create()
      {
         return create(CoreConstants.DEFAULT_PROTOCOL_END_TIME_IN_MIN, _displayUnitRetriever.PreferredUnitFor(_dimensionRepository.Time));
      }

      public OutputSchema CreateFor(Simulation simulation)
      {
         var protocol = longestProtocolIn(simulation);
         double endTime = protocol.EndTime;

         var dayUnit = _dimensionRepository.Time.Unit(CoreConstants.Units.Days);

         //offset is one day
         double offset = _dimensionRepository.Time.UnitValueToBaseUnitValue(dayUnit, 1);
         if (protocolNeedsOffsetForEndTime(protocol))
            endTime += offset;

         return create(endTime, protocol.TimeUnit);
      }

      private OutputSchema create(double endTime, Unit timeDisplaUnit)
      {
         var outputSchema = _objectBaseFactory.Create<OutputSchema>();

         var simIntervalHighResolution = _outputIntervalFactory.Create(0, CoreConstants.HIGH_RESOLUTION_END_TIME_IN_MIN, CoreConstants.HIGH_RESOLUTION_IN_PTS_PER_MIN)
            .WithName(PKSimConstants.UI.SimulationIntervalHighResolution);

         var simIntervalLowResolution = _outputIntervalFactory.Create(CoreConstants.HIGH_RESOLUTION_END_TIME_IN_MIN, endTime, CoreConstants.LOW_RESOLUTION_IN_PTS_PER_MIN)
            .WithName(PKSimConstants.UI.SimulationIntervalLowResolution);

         simIntervalLowResolution.StartTime.DisplayUnit = timeDisplaUnit;
         simIntervalLowResolution.EndTime.DisplayUnit = timeDisplaUnit;
         simIntervalHighResolution.StartTime.DisplayUnit = timeDisplaUnit;
         simIntervalHighResolution.EndTime.DisplayUnit = timeDisplaUnit;
         outputSchema.AddInterval(simIntervalHighResolution);
         outputSchema.AddInterval(simIntervalLowResolution);
         return outputSchema;
      }

      private Protocol longestProtocolIn(Simulation simulation)
      {
         return simulation.AllBuildingBlocks<Protocol>().LongestProtocol();
      }

      private bool protocolNeedsOffsetForEndTime(Protocol protocol)
      {
         var simpleProtocol = protocol as SimpleProtocol;
         return simpleProtocol == null || !simpleProtocol.IsSingleDosing;
      }
   }
}