using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;

using PKSim.Presentation.DTO.Populations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IPopulationSettingsDTOMapper : IMapper<PopulationSettingsDTO, RandomPopulationSettings>,
                                                   IMapper<RandomPopulationSettings, PopulationSettingsDTO>,
                                                   IMapper<PKSim.Core.Model.Individual, PopulationSettingsDTO>
   {
   }

   public class PopulationSettingsDTOMapper : IPopulationSettingsDTOMapper
   {
      private readonly IIndividualToPopulationSettingsMapper _individualToPopulationSettingsMapper;
      private readonly ICloner _cloner;

      public PopulationSettingsDTOMapper(IIndividualToPopulationSettingsMapper individualToPopulationSettingsMapper, ICloner cloner)
      {
         _individualToPopulationSettingsMapper = individualToPopulationSettingsMapper;
         _cloner = cloner;
      }

      public RandomPopulationSettings MapFrom(PopulationSettingsDTO populationSettingsDTO)
      {
         var populationSettings = new RandomPopulationSettings();
         populationSettings.BaseIndividual = _cloner.Clone(populationSettingsDTO.Individual);
         populationSettings.NumberOfIndividuals = populationSettingsDTO.NumberOfIndividuals.ConvertedTo<int>();

         //first add one gender with ration 100 for each available gender
         populationSettingsDTO.AvailableGenders().Each(g => populationSettings.AddGenderRatio(new GenderRatio {Gender = g, Ratio = 100}));

         //in case of multiple gender, adjust the ration according to the feamales proportion
         if (populationSettingsDTO.HasMultipleGenders)
         {
            populationSettings.GenderRatio(populationSettingsDTO.Female).Ratio = populationSettingsDTO.ProportionOfFemales;
            populationSettings.GenderRatio(populationSettingsDTO.Male).Ratio = 100 - populationSettingsDTO.ProportionOfFemales;
         }

         populationSettingsDTO.Parameters.Each(p => populationSettings.AddParameterRange(p.ParameterRange));
         return populationSettings;
      }

      public PopulationSettingsDTO MapFrom(RandomPopulationSettings populationSettings)
      {
         var populationSettingsDTO = new PopulationSettingsDTO();
         populationSettingsDTO.Individual = populationSettings.BaseIndividual;
         populationSettingsDTO.NumberOfIndividuals = populationSettings.NumberOfIndividuals.ConvertedTo<uint>();
         //use clone since we do not want to override the value if the user clicks cancel
         //in case of multiple gender, adjust the ration according to the feamales proportion
         if (populationSettingsDTO.HasMultipleGenders)
         {
            var femaleRatio = populationSettings.GenderRatio(populationSettingsDTO.Female);
            populationSettingsDTO.ProportionOfFemales = femaleRatio.Ratio;
         }
         populationSettings.ParameterRanges.Each(pr => populationSettingsDTO.Parameters.Add(new ParameterRangeDTO(pr.Clone())));

         return populationSettingsDTO;
      }

      public PopulationSettingsDTO MapFrom(PKSim.Core.Model.Individual individual)
      {
         return MapFrom(_individualToPopulationSettingsMapper.MapFrom(individual));
      }
   }
}