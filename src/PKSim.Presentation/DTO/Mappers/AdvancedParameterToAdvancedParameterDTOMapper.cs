using OSPSuite.Utility;
using PKSim.Core.Model;

using PKSim.Presentation.DTO.Populations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IAdvancedParameterToAdvancedParameterDTOMapper : IMapper<IAdvancedParameter, AdvancedParameterDTO>
   {
   }

   public class AdvancedParameterToAdvancedParameterDTOMapper : IAdvancedParameterToAdvancedParameterDTOMapper
   {
      public AdvancedParameterDTO MapFrom(IAdvancedParameter advancedParameter)
      {
         return new AdvancedParameterDTO
                   {
                      DistributionType = advancedParameter.DistributionType,
                      ParameterFullDisplayName = advancedParameter.FullDisplayName,
                      Parameters = advancedParameter.AllParameters
                   };
      }
   }
}