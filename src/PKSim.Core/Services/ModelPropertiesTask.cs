using System;
using System.Linq;
using OSPSuite.Core.Domain;
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

      /// <summary>
      ///    Checks if the old and new model properties are compatible (same model and species). In that case, return the old one
      ///    else
      ///    we try to update as many properties as possible in the old one (such as cm , model etc.)
      /// </summary>
      ModelProperties Update(ModelProperties oldModelProperties, ModelProperties newModelProperties, OriginData originData);

      /// <summary>
      ///    Make sure that the model properties are up-to-date with the definition in the database. This should be called once
      ///    the simulation is being loaded
      /// </summary>
      void UpdateCategoriesIn(ModelProperties modelProperties, OriginData originData);
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

      public ModelProperties Update(ModelProperties oldModelProperties, ModelProperties newModelProperties, OriginData originData)
      {
         if (oldModelProperties == null)
         {
            UpdateCategoriesIn(newModelProperties, originData);
            return newModelProperties;
         }

         var oldModelConfig = oldModelProperties.ModelConfiguration;
         var newModelConfig = newModelProperties.ModelConfiguration;

         //same species and same model=>we can return the old one
         if (areCompatible(oldModelConfig, newModelConfig))
         {
            UpdateCategoriesIn(oldModelProperties, originData);
            return oldModelProperties;
         }

         //in that case. Try to update as much CM as we can
         foreach (var calculationMethod in newModelProperties.AllCalculationMethods().ToList())
         {
            var category = calculationMethod.Category;
            var oldCalculationMethod = oldModelProperties.CalculationMethodFor(category);
            var newCategory = newModelConfig.CalculationMethodCategories.FindByName(category);

            if (oldCalculationMethod == null || newCategory == null || !newCategory.AllItems().Contains(oldCalculationMethod))
               continue;

            newModelProperties.RemoveCalculationMethod(calculationMethod);
            newModelProperties.AddCalculationMethod(oldCalculationMethod);
         }

         return newModelProperties;
      }

      //Make sure that newly added categories in the pksim db are available
      public void UpdateCategoriesIn(ModelProperties modelProperties, OriginData originData)
      {
         var defaultModelProperties = DefaultFor(modelProperties.ModelConfiguration, originData);
         foreach (var defaultCalculationMethod in defaultModelProperties.AllCalculationMethods())
         {
            var calculationMethod = modelProperties.CalculationMethodFor(defaultCalculationMethod.Category);
            if (calculationMethod != null)
               continue;

            //cm does not exist in model properties. Yet it's defined in the default=> just add the new value
            modelProperties.AddCalculationMethod(defaultCalculationMethod);
         }
      }

      private bool areCompatible(ModelConfiguration oldModelConfiguration, ModelConfiguration newModelConfiguration)
      {
         return areModelEquals(oldModelConfiguration, newModelConfiguration) &&
                areSpeciesEquals(oldModelConfiguration, newModelConfiguration);
      }

      private bool areModelEquals(ModelConfiguration oldModelConfiguration, ModelConfiguration newModelConfiguration)
      {
         return string.Equals(oldModelConfiguration.ModelName, newModelConfiguration.ModelName);
      }

      private bool areSpeciesEquals(ModelConfiguration oldModelConfiguration, ModelConfiguration newModelConfiguration)
      {
         return string.Equals(oldModelConfiguration.SpeciesName, newModelConfiguration.SpeciesName);
      }
   }
}