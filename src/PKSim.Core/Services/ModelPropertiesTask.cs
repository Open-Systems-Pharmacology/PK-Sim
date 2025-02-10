using System;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IModelPropertiesTask
   {
      /// <summary>
      ///    Creates the default model properties based on origin data
      /// </summary>
      ModelProperties DefaultFor(OriginData originData);

      /// <summary>
      ///    Creates the default model properties based on origin data and model name
      /// </summary>
      ModelProperties DefaultFor(OriginData originData, string modelName);

      /// <summary>
      ///    Creates the default model properties for a predefined model configuration
      /// </summary>
      ModelProperties DefaultFor(ModelConfiguration modelConfiguration, OriginData originData);
   }

   public class ModelPropertiesTask : IModelPropertiesTask
   {
      private readonly IModelConfigurationRepository _modelConfigurationRepository;

      public ModelPropertiesTask(IModelConfigurationRepository modelConfigurationRepository)
      {
         _modelConfigurationRepository = modelConfigurationRepository;
      }

      public ModelProperties DefaultFor(OriginData originData)
      {
         var defaultModel = _modelConfigurationRepository.DefaultFor(originData.Species);
         var modelProperties = DefaultFor(defaultModel, originData);
         return modelProperties;
      }

      public ModelProperties DefaultFor(OriginData originData, string modelName)
      {
         var modelsForSpecies = _modelConfigurationRepository.AllFor(originData.Species);

         foreach (var modelConfiguration in modelsForSpecies)
         {
            if (!modelConfiguration.ModelName.Equals(modelName))
               continue;

            return DefaultFor(modelConfiguration, originData);
         }

         //model not found for given species
         throw new ArgumentException(PKSimConstants.Error.ModelNotAvailableForSpecies(modelName, originData.Species.Name));
      }

      public ModelProperties DefaultFor(ModelConfiguration modelConfiguration, OriginData originData)
      {
         var modelProperties = new ModelProperties();

         foreach (var cmCategory in modelConfiguration.CalculationMethodCategories.Where(x => !x.IsIndividual))
            modelProperties.AddCalculationMethod(cmCategory.DefaultItemForSpecies(originData.Species));

         //now add origin data calculation methods
         originData.AllCalculationMethods().Each(modelProperties.AddCalculationMethod);

         modelProperties.ModelConfiguration = modelConfiguration;
         return modelProperties;
      }
   }
}