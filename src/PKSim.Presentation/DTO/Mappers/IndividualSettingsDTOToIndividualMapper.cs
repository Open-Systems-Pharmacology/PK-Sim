using OSPSuite.Utility;
using PKSim.Core.Model;

using PKSim.Presentation.DTO.Individuals;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IIndividualSettingsDTOToIndividualMapper : IMapper<IndividualSettingsDTO, PKSim.Core.Model.Individual>
   {
   }

   public class IndividualSettingsDTOToIndividualMapper : IIndividualSettingsDTOToIndividualMapper
   {
      private readonly IIndividualFactory _individualFactory;
      private readonly IIndividualSettingsDTOToOriginDataMapper _mapper;

      public IndividualSettingsDTOToIndividualMapper(IIndividualFactory individualFactory, IIndividualSettingsDTOToOriginDataMapper mapper)
      {
         _individualFactory = individualFactory;
         _mapper = mapper;
      }

      public PKSim.Core.Model.Individual MapFrom(IndividualSettingsDTO individualSettingsDTO)
      {
         return _individualFactory.CreateAndOptimizeFor(_mapper.MapFrom(individualSettingsDTO));
      }
   }
}