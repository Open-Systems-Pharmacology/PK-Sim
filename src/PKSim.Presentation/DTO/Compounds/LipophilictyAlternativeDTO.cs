
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Compounds
{
   public class LipophilictyAlternativeDTO : ParameterAlternativeDTO
   {
      public LipophilictyAlternativeDTO(PKSim.Core.Model.ParameterAlternative parameterAlternative) : base(parameterAlternative)
      {
      }

      public IParameterDTO LipophilictyParameter { get; set; }

      public double Lipophilicty
      {
         get { return LipophilictyParameter.Value; }
         set{/*nothing to do here*/}
      }
   }
}