using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Services;

namespace PKSim.Core.Batch.Mapper
{
   public interface IModelPropertiesMapper
   {
      ModelProperties MapFrom(SimulationConfiguration configuration, Model.Individual individual);
   }

   public class ModelPropertiesMapper : IModelPropertiesMapper
   {
      private readonly IModelPropertiesTask _modelPropertiesTask;
      private readonly IBatchLogger _batchLogger;

      public ModelPropertiesMapper(IModelPropertiesTask modelPropertiesTask,  IBatchLogger batchLogger)
      {
         _modelPropertiesTask = modelPropertiesTask;
         _batchLogger = batchLogger;
      }

      public ModelProperties MapFrom(SimulationConfiguration configuration, Model.Individual individual)
      {
         var modelProperties = _modelPropertiesTask.DefaultFor(individual.OriginData, configuration.Model);
         _batchLogger.AddDebug("Using model method '{0}'".FormatWith(configuration.Model));
         return modelProperties;
      }
   }
}