using OSPSuite.Utility;
using PKSim.Core.Model;

using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IModelConfigurationDTOToModelPropertiesMapper : IMapper<ModelConfigurationDTO, ModelProperties>
   {
   }

   public class ModelConfigurationDTOToModelPropertiesMapper : IModelConfigurationDTOToModelPropertiesMapper
   {
      public ModelProperties MapFrom(ModelConfigurationDTO modelConfigurationDTO)
      {
         if (modelConfigurationDTO == null)
            return null;

         var modelProperties = new ModelProperties {ModelConfiguration = modelConfigurationDTO.ModelConfiguration};
         foreach (var calculationMethodDTO in modelConfigurationDTO.CalculationMethodDTOs)
         {
            modelProperties.AddCalculationMethod(calculationMethodDTO.CalculationMethod);
         }
         return modelProperties;
      }
   }
}