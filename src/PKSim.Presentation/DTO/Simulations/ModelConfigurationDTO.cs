using System.Collections.Generic;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations
{
   public class ModelConfigurationDTO 
   {
      public ModelConfiguration ModelConfiguration { get; set; }
      public IEnumerable<CategoryCalculationMethodDTO> CalculationMethodDTOs { get; set; }
   }
}