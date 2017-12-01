using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using BatchOriginData = PKSim.Core.Batch.OriginData;

namespace PKSim.Core.Batch.Mapper
{
   internal interface IOriginDataMapper : IMapper<BatchOriginData, Model.OriginData>
   {
   }

   internal class OriginDataMapper : IOriginDataMapper
   {
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IOriginDataTask _originDataTask;
      private readonly IIndividualModelTask _individualModelTask;

      public OriginDataMapper(ISpeciesRepository speciesRepository, IOriginDataTask originDataTask, IIndividualModelTask individualModelTask)
      {
         _speciesRepository = speciesRepository;
         _originDataTask = originDataTask;
         _individualModelTask = individualModelTask;
      }

      public Model.OriginData MapFrom(BatchOriginData batchOriginData)
      {
         var originData = new Model.OriginData();
         var species = _speciesRepository.FindByName(batchOriginData.Species);
         if (species == null)
            throw new PKSimException(PKSimConstants.Error.CouldNotFindSpecies(batchOriginData.Species, _speciesRepository.AllNames()));

         originData.Species = species;

         var population = species.PopulationByName(batchOriginData.Population);
         if (population == null)
         {
            if (string.IsNullOrEmpty(batchOriginData.Population) && species.Populations.Count() == 1)
               population = species.Populations.ElementAt(0);
            else
               throw new PKSimException(PKSimConstants.Error.CouldNotFindPopulationForSpecies(batchOriginData.Population, batchOriginData.Species, species.Populations.AllNames()));
         }

         originData.SpeciesPopulation = population;

         var gender = population.GenderByName(batchOriginData.Gender);
         if (gender == null)
         {
            if (string.IsNullOrEmpty(batchOriginData.Gender))
               gender = population.Genders.ElementAt(0);
            else
               throw new PKSimException(PKSimConstants.Error.CouldNotFindGenderForPopulation(batchOriginData.Gender, batchOriginData.Population, population.Genders.AllNames()));
         }

         originData.Gender = gender;
         //this is not defined in matlab yet
         originData.SubPopulation = _originDataTask.DefaultSubPopulationFor(species);
         
         if (originData.SpeciesPopulation.IsAgeDependent)
         {
            originData.Age = batchOriginData.Age;
            var meanAgeParameter = _individualModelTask.MeanAgeFor(originData);
            originData.Age = valueFrom(batchOriginData.Age, meanAgeParameter.Value);
            originData.AgeUnit = meanAgeParameter.Dimension.DefaultUnit.Name;
            originData.GestationalAge = valueFrom(batchOriginData.GestationalAge, CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS);
            originData.GestationalAgeUnit = CoreConstants.Units.Weeks;
         }

         var calculationMethodCategoryForSpecies = _originDataTask.AllCalculationMethodCategoryFor(species);
         foreach (var category in calculationMethodCategoryForSpecies)
         {
            string selectedCalculationMethod = batchOriginData.CalculationMethodFor(category.Name);
            if (string.IsNullOrEmpty(selectedCalculationMethod))
               originData.AddCalculationMethod(category.DefaultItemForSpecies(species));
            else
            {
               var calculationMethod = category.AllItems().FindByName(selectedCalculationMethod);
               if (calculationMethod == null)
                  throw new PKSimException(PKSimConstants.Error.CouldNotFindCalculationMethodInCategory(selectedCalculationMethod, category.Name, category.AllItems().AllNames()));
               if (calculationMethod.AllSpecies.Contains(species.Name))
                  originData.AddCalculationMethod(calculationMethod);
               else
                  throw new PKSimException(PKSimConstants.Error.CalculationMethodNotDefinedForSpecies(selectedCalculationMethod, category.Name, species.Name));
            }
         }

         var meanWeightParameter = _individualModelTask.MeanWeightFor(originData);
         originData.Weight = valueFrom(batchOriginData.Weight, meanWeightParameter.Value).Value;
         originData.WeightUnit = meanWeightParameter.Dimension.DefaultUnit.Name;

         if (originData.SpeciesPopulation.IsHeightDependent)
         {
            var meanHeightParameter = _individualModelTask.MeanHeightFor(originData);
            originData.Height = valueFrom(batchOriginData.Height, meanHeightParameter.Value);
            originData.HeightUnit = meanHeightParameter.Dimension.DefaultUnit.Name;
         }
       
         return originData;
      }

      //matlab might not support nullable. Hence we adopt the convention that NaN in matlab is null in .NET
      private double? valueFrom(double valueToConvert, double? defaultValue=null)
      {
         if (double.IsNaN(valueToConvert))
            return defaultValue;

         return valueToConvert;
      }
   }
}