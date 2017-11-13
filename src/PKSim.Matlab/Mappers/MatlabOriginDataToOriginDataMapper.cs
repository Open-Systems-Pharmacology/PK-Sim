using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Matlab.Mappers
{
   public interface IMatlabOriginDataToOriginDataMapper : IMapper<OriginData, Core.Model.OriginData>
   {
      Core.Model.OriginData MapFrom(PopulationSettings matlabPopulationSettings);
   }

   public class MatlabOriginDataToOriginDataMapper : IMatlabOriginDataToOriginDataMapper
   {
      private readonly OriginDataMapper _originDataMapper;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IMatlabParameterToSnapshotParameterMapper _parameterMapper;

      public MatlabOriginDataToOriginDataMapper(OriginDataMapper originDataMapper, IDimensionRepository dimensionRepository, IMatlabParameterToSnapshotParameterMapper parameterMapper)
      {
         _originDataMapper = originDataMapper;
         _dimensionRepository = dimensionRepository;
         _parameterMapper = parameterMapper;
      }

      public Core.Model.OriginData MapFrom(OriginData matlabOriginData)
      {
         var snapshotOriginData = new Core.Snapshots.OriginData
         {
            Species = matlabOriginData.Species,
            Population = matlabOriginData.Population,
            Gender = matlabOriginData.Gender,
            Age = parameterFrom(matlabOriginData.Age, _dimensionRepository.AgeInYears),
            Weight = parameterFrom(matlabOriginData.Weight, _dimensionRepository.Mass),
            Height = parameterFrom(matlabOriginData.Height, _dimensionRepository.Length),
            GestationalAge = parameterFrom(matlabOriginData.GestationalAge, _dimensionRepository.AgeInWeeks),
         };

         matlabOriginData.AllCalculationMethods.KeyValues.Each(kv => snapshotOriginData.AddCalculationMethod(kv.Key, kv.Value));

         return _originDataMapper.MapToModel(snapshotOriginData).Result;
      }

      public Core.Model.OriginData MapFrom(PopulationSettings matlabPopulationSettings)
      {
         //create default individual based on given data
         var matlabOriginData = new OriginData
         {
            Species = matlabPopulationSettings.Species,
            Population = matlabPopulationSettings.Population
         };

         matlabPopulationSettings.AllCalculationMethods.KeyValues.Each(kv => matlabOriginData.AddCalculationMethod(kv.Key, kv.Value));

         return MapFrom(matlabOriginData);
      }

      private Parameter parameterFrom(double value, IDimension dimension) => _parameterMapper.MapFrom(value, dimension);
   }
}