
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Compounds
{
   public class SolubilityAlternativeDTO : ParameterAlternativeDTO
   {
      public SolubilityAlternativeDTO(PKSim.Core.Model.ParameterAlternative parameterAlternative) : base(parameterAlternative)
      {
      }

      public IParameterDTO SolubilityParameter { get; set; }
      public IParameterDTO RefpHParameter { get; set; }
      public IParameterDTO GainPerChargeParameter { get; set; }

      public double Solubility
      {
         get { return SolubilityParameter.Value; }
         set
         {
            /*nothing to do here*/
         }
      }

      public double RefpH
      {
         get { return RefpHParameter.Value; }
         set
         {
            /*nothing to do here*/
         }
      }

      public double GainPerCharge
      {
         get { return GainPerChargeParameter.Value; }
         set
         {
            /*nothing to do here*/
         }
      }
   }
}