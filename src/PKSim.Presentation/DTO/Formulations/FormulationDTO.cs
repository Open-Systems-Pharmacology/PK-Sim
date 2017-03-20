using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.DTO.Formulations
{
   public class FormulationDTO
   {
      public FormulationTypeDTO Type { get; set; }
      public string Description { get; set; }
      public IEnumerable<IParameter> Parameters { get; set; }
   }
}