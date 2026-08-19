using OSPSuite.Core.Domain;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IParameterToParameterCommitDTOMapper
   {
      ParameterCommitDTO MapFrom(string path, IParameter parameter);
   }

   public class ParameterToParameterCommitDTOMapper : IParameterToParameterCommitDTOMapper
   {
      public ParameterCommitDTO MapFrom(string path, IParameter parameter)
      {
         return new ParameterCommitDTO
         {
            Path = path,
            DisplayPath = path,
            Value = parameter?.ValueInDisplayUnit ?? double.NaN,
            Unit = parameter?.DisplayUnit?.Name,
            ValueOrigin = parameter?.ValueOrigin?.Display
         };
      }
   }
}
