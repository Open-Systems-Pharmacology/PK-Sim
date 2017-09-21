using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Assets;
using PKSim.Core.Repositories;

namespace PKSim.Core.Model
{
   public interface IOutputSchemaFactory
   {
      OutputSchema CreateEmpty();
      OutputSchema CreateFor(Simulation simulation);
   }

   public class OutputSchemaFactory : IOutputSchemaFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IOutputIntervalFactory _outputIntervalFactory;
      private readonly IDimensionRepository _dimensionRepository;

      public OutputSchemaFactory(IObjectBaseFactory objectBaseFactory, IOutputIntervalFactory outputIntervalFactory, IDimensionRepository dimensionRepository)
      {
         _objectBaseFactory = objectBaseFactory;
         _outputIntervalFactory = outputIntervalFactory;
         _dimensionRepository = dimensionRepository;
      }

      public OutputSchema CreateEmpty()
      {
         return _objectBaseFactory.Create<OutputSchema>();
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
         var outputSchema = CreateEmpty();

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

   
      private Protocol longestProtocolIn(Simulation simulation) => simulation.AllBuildingBlocks<Protocol>().LongestProtocol();

      private bool protocolNeedsOffsetForEndTime(Protocol protocol)
      {
         var simpleProtocol = protocol as SimpleProtocol;
         return simpleProtocol == null || !simpleProtocol.IsSingleDosing;
      }
   }
}