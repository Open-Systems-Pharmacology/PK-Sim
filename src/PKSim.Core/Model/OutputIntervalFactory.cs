using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface IOutputIntervalFactory
   {
      OutputInterval CreateDefaultFor(OutputSchema outputSchema);
      OutputInterval Create(double startTimeInMinute, double endTimeInMinute, double resolutionInPtsPerHour);
   }

   public class OutputIntervalFactory : IOutputIntervalFactory
   {
      private readonly OSPSuite.Core.Domain.IOutputIntervalFactory _outputIntervalFactory;
      private readonly IContainerTask _containerTask;

      public OutputIntervalFactory(OSPSuite.Core.Domain.IOutputIntervalFactory outputIntervalFactory, IContainerTask containerTask)
      {
         _outputIntervalFactory = outputIntervalFactory;
         _containerTask = containerTask;
      }

      public OutputInterval CreateDefaultFor(OutputSchema outputSchema)
      {
         var defaultInterval = updated(_outputIntervalFactory.CreateDefault());
         defaultInterval.Name = _containerTask.CreateUniqueName(outputSchema, defaultInterval.Name);
         return defaultInterval;
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