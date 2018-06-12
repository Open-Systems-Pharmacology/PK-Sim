using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Matlab.Mappers
{
   public interface IMatlabPopulationSettingsToPopulationSettingsMapper : IMapper<Core.Snapshots.PopulationSettings, RandomPopulationSettings>
   {
   }

   public class MatlabPopulationSettingsToPopulationSettingsMapper : IMatlabPopulationSettingsToPopulationSettingsMapper
   {
      private readonly RandomPopulationSettingsMapper _randomPopulationSettingsMapper;

      public MatlabPopulationSettingsToPopulationSettingsMapper(RandomPopulationSettingsMapper randomPopulationSettingsMapper)
      {
         _randomPopulationSettingsMapper = randomPopulationSettingsMapper;
      }

      public RandomPopulationSettings MapFrom(Core.Snapshots.PopulationSettings matlabPopulationSettings)
      {
         return _randomPopulationSettingsMapper.MapToModel(matlabPopulationSettings).Result;
      }
   }
}