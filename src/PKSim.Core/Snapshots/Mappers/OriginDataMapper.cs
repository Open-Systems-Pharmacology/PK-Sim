using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using ModelOriginData = PKSim.Core.Model.OriginData;
using SnapshotOriginData = PKSim.Core.Snapshots.OriginData;

namespace PKSim.Core.Snapshots.Mappers
{
   public class OriginDataMapper : SnapshotMapperBase<ModelOriginData, SnapshotOriginData>
   {
      private readonly IDimensionRepository _dimensionRepository;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly ParameterMapper _parameterMapper;
      private readonly IOriginDataTask _originDataTask;
      private readonly IIndividualModelTask _individualModelTask;

      public OriginDataMapper(
         ParameterMapper parameterMapper,
         IOriginDataTask originDataTask,
         IDimensionRepository dimensionRepository,
         IIndividualModelTask individualModelTask,
         ISpeciesRepository speciesRepository
      )
      {
         _dimensionRepository = dimensionRepository;
         _speciesRepository = speciesRepository;
         _parameterMapper = parameterMapper;
         _originDataTask = originDataTask;
         _individualModelTask = individualModelTask;
      }

      public override Task<SnapshotOriginData> MapToSnapshot(ModelOriginData originData)
      {
         return SnapshotFrom(originData, x =>
         {
            x.Species = originData.Species.Name;
            x.Population = originData.Species.Populations.Count > 1 ? originData.SpeciesPopulation.Name : null;
            x.Gender = originData.SpeciesPopulation.Genders.Count > 1 ? originData.Gender.Name : null;
            x.Age = parameterFrom(originData.Age, originData.AgeUnit, _dimensionRepository.AgeInYears);
            x.GestationalAge = parameterFrom(originData.GestationalAge, originData.GestationalAgeUnit, _dimensionRepository.AgeInWeeks);
            x.Height = parameterFrom(originData.Height, originData.HeightUnit, _dimensionRepository.Length);
            x.Weight = parameterFrom(originData.Weight, originData.WeightUnit, _dimensionRepository.Mass);
         });
      }

      public override Task<ModelOriginData> MapToModel(SnapshotOriginData snapshot)
      {
         var originData = new ModelOriginData {Species = speciesFrom(snapshot)};

         originData.SpeciesPopulation = speciesPopulationFrom(snapshot, originData.Species);
         originData.Gender = genderFrom(snapshot, originData.SpeciesPopulation);
         originData.SubPopulation = _originDataTask.DefaultSubPopulationFor(originData.Species);

         updateAgeFromSnapshot(snapshot, originData);
         updateCalculationMethodsFromSnapshot(snapshot, originData);

         updateWeightFromSnapshot(snapshot, originData);
         updateHeightFromSnapshot(snapshot, originData);

         return Task.FromResult(originData);
      }

      private void updateWeightFromSnapshot(SnapshotOriginData snapshot, ModelOriginData originData)
      {
         var meanWeightParameter = _individualModelTask.MeanWeightFor(originData);
         originData.Weight = baseParameterValueFrom(snapshot.Weight, meanWeightParameter.Dimension, meanWeightParameter.Value);
         originData.WeightUnit = meanWeightParameter.Dimension.UnitOrDefault(snapshot.Weight.Unit).Name;
      }

      private void updateHeightFromSnapshot(SnapshotOriginData snapshot, ModelOriginData originData)
      {
         if (!originData.SpeciesPopulation.IsHeightDependent)
            return;

         var meanHeightParameter = _individualModelTask.MeanHeightFor(originData);
         originData.Height = baseParameterValueFrom(snapshot.Height, meanHeightParameter.Dimension, meanHeightParameter.Value);
         originData.HeightUnit = meanHeightParameter.Dimension.UnitOrDefault(snapshot.Height.Unit).Name;
      }

      private void updateCalculationMethodsFromSnapshot(SnapshotOriginData snapshot, ModelOriginData originData)
      {
         var calculationMethodCategoryForSpecies = _originDataTask.AllCalculationMethodCategoryFor(originData.Species);
         foreach (var category in calculationMethodCategoryForSpecies)
         {
            string selectedCalculationMethod = snapshot.CalculationMethodFor(category.Name);
            if (string.IsNullOrEmpty(selectedCalculationMethod))
               originData.AddCalculationMethod(category.DefaultItemForSpecies(originData.Species));
            else
            {
               var calculationMethod = category.AllItems().FindByName(selectedCalculationMethod);
               if (calculationMethod == null)
                  throw new PKSimException(PKSimConstants.Error.CouldNotFindCalculationMethodInCategory(selectedCalculationMethod, category.Name, category.AllItems().AllNames()));

               if (calculationMethod.AllSpecies.Contains(originData.Species.Name))
                  originData.AddCalculationMethod(calculationMethod);
               else
                  throw new PKSimException(PKSimConstants.Error.CalculationMethodNotDefinedForSpecies(selectedCalculationMethod, category.Name, originData.Species.Name));
            }
         }
      }

      private void updateAgeFromSnapshot(SnapshotOriginData snapshot, ModelOriginData originData)
      {
         if (!originData.SpeciesPopulation.IsAgeDependent)
            return;

         var meanAgeParameter = _individualModelTask.MeanAgeFor(originData);
         originData.Age = baseParameterValueFrom(snapshot.Age, meanAgeParameter.Dimension, meanAgeParameter.Value);
         originData.AgeUnit = meanAgeParameter.Dimension.UnitOrDefault(snapshot.Age.Unit).Name;

         var meanGestationalAgeFor = _individualModelTask.MeanGestationalAgeFor(originData);
         originData.GestationalAge = baseParameterValueFrom(snapshot.GestationalAge, meanGestationalAgeFor.Dimension, meanGestationalAgeFor.Value);
         originData.GestationalAgeUnit = meanGestationalAgeFor.Dimension.UnitOrDefault(snapshot.GestationalAge.Unit).Name;
      }

      private Gender genderFrom(SnapshotOriginData snapshot, SpeciesPopulation speciesPopulation)
      {
         var gender = speciesPopulation.GenderByName(snapshot.Gender);
         if (gender != null)
            return gender;

         if (string.IsNullOrEmpty(snapshot.Gender))
            return speciesPopulation.Genders.ElementAt(0);

         throw new PKSimException(PKSimConstants.Error.CouldNotFindGenderForPopulation(snapshot.Gender, snapshot.Population, speciesPopulation.Genders.AllNames()));
      }

      private SpeciesPopulation speciesPopulationFrom(SnapshotOriginData snapshot, Species species)
      {
         var population = species.PopulationByName(snapshot.Population);
         if (population != null)
            return population;

         if (string.IsNullOrEmpty(snapshot.Population) && species.Populations.Count() == 1)
            return species.Populations.ElementAt(0);

         throw new PKSimException(PKSimConstants.Error.CouldNotFindPopulationForSpecies(snapshot.Population, snapshot.Species, species.Populations.AllNames()));
      }

      private Species speciesFrom(SnapshotOriginData snapshot)
      {
         var species = _speciesRepository.FindByName(snapshot.Species);
         return species ?? throw new PKSimException(PKSimConstants.Error.CouldNotFindSpecies(snapshot.Species, _speciesRepository.AllNames()));
      }

      private double baseParameterValueFrom(Parameter snapshot, IDimension dimension, double defaultValue)
      {
         if (snapshot?.Value == null)
            return defaultValue;

         var unit = dimension.Unit(ModelValueFor(snapshot.Unit));
         return dimension.UnitValueToBaseUnitValue(unit, snapshot.Value.Value);
      }

      private Parameter parameterFrom(double? parameterBaseValue, string parameterDisplayUnit, IDimension dimension)
      {
         return _parameterMapper.ParameterFrom(parameterBaseValue, parameterDisplayUnit, dimension);
      }
   }
}