using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IParameterDTOToOriginDataParameterMapper
   {
      OriginDataParameter MapFrom(IParameterDTO parameterDTO, bool addName = false);
   }

   public class ParameterDTOToOriginDataParameterMapper : IParameterDTOToOriginDataParameterMapper
   {
      public OriginDataParameter MapFrom(IParameterDTO parameterDTO, bool addName = false)
      {
         var originDataParameter = new OriginDataParameter(parameterDTO.KernelValue, displayUnit(parameterDTO));
         if (addName)
            originDataParameter.Name = parameterDTO.Name;

         return originDataParameter;
      }

      private static string displayUnit(IParameterDTO parameterDTO)
      {
         if (parameterDTO.DisplayUnit == null)
            return string.Empty;

         return parameterDTO.DisplayUnit.Name;
      }
   }
}