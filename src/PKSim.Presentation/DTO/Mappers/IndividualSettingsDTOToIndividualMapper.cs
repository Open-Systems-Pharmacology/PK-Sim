using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IIndividualSettingsDTOToIndividualMapper : IMapper<IndividualSettingsDTO, Individual>
   {
   }

   public class IndividualSettingsDTOToIndividualMapper : IIndividualSettingsDTOToIndividualMapper
   {
      private readonly IIndividualFactory _individualFactory;
      private readonly IIndividualSettingsDTOToOriginDataMapper _originDataMapper;

      public IndividualSettingsDTOToIndividualMapper(IIndividualFactory individualFactory, IIndividualSettingsDTOToOriginDataMapper originDataMapper)
      {
         _individualFactory = individualFactory;
         _originDataMapper = originDataMapper;
      }

      public Individual MapFrom(IndividualSettingsDTO individualSettingsDTO)
      {
         return _individualFactory.CreateAndOptimizeFor(_originDataMapper.MapFrom(individualSettingsDTO));
      }
   }
}