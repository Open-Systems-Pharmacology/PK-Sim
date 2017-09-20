using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public interface IOutputIntervalFactory
   {
      OutputInterval CreateDefault();
      OutputInterval Create(double startTimeInMinute, double endTimeInMinute, double resolutionInPtsPerHour);
   }

   public class OutputIntervalFactory : IOutputIntervalFactory
   {
      private readonly OSPSuite.Core.Domain.IOutputIntervalFactory _outputIntervalFactory;

      public OutputIntervalFactory(OSPSuite.Core.Domain.IOutputIntervalFactory outputIntervalFactory)
      {
         _outputIntervalFactory = outputIntervalFactory;
      }

      public OutputInterval CreateDefault()
      {
         var defaultInterval = _outputIntervalFactory.CreateDefault();
         return updated(defaultInterval);
      }

      public OutputInterval Create(double startTimeInMinute, double endTimeInMinute, double resolutionInPtsPerHour)
      {
         var interval = _outputIntervalFactory.Create(startTimeInMinute, endTimeInMinute, resolutionInPtsPerHour);
         return updated(interval);
      }

      private OutputInterval updated(OutputInterval interval)
      {
         interval.Name = PKSimConstants.UI.SimulationInterval;
         interval.AllParameters().Each(p => p.BuildingBlockType = PKSimBuildingBlockType.Simulation);
         return interval;
      }
   }
}