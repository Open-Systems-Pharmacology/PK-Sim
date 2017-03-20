
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Compounds
{
   public class PermeabilityAlternativeDTO : ParameterAlternativeDTO
   {
      public PermeabilityAlternativeDTO(PKSim.Core.Model.ParameterAlternative parameterAlternative) : base(parameterAlternative)
      {
      }

      public IParameterDTO PermeabilityParameter { get; set; }

      public double Permeability
      {
         get { return PermeabilityParameter.Value; }
         set { PermeabilityParameter.Value = value; }
      }
   }
}