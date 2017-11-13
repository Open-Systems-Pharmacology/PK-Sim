using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Services;

namespace PKSim.Core.Batch.Mapper
{
   internal interface IModelPropertiesMapper
   {
      ModelProperties MapFrom(SimulationConfiguration configuration, Model.Individual individual);
   }

   internal class ModelPropertiesMapper : IModelPropertiesMapper
   {
      private readonly IModelPropertiesTask _modelPropertiesTask;
      private readonly ILogger _batchLogger;

      public ModelPropertiesMapper(IModelPropertiesTask modelPropertiesTask, ILogger batchLogger)
      {
         _modelPropertiesTask = modelPropertiesTask;
         _batchLogger = batchLogger;
      }

      public ModelProperties MapFrom(SimulationConfiguration configuration, Model.Individual individual)
      {
         var modelProperties = _modelPropertiesTask.DefaultFor(individual.OriginData, configuration.Model);
         _batchLogger.AddDebug($"Using model method '{configuration.Model}'");
         return modelProperties;
      }
   }
}