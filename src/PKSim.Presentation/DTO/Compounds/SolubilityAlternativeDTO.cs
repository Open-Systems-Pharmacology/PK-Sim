
using OSPSuite.Core.Domain.Formulas;
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

      public bool IsTable => SolubilityParameter?.FormulaType == FormulaType.Table;

      public double Solubility
      {
         get => SolubilityParameter.Value;
         set
         {
            /*nothing to do here*/
         }
      }

      public double RefpH
      {
         get => RefpHParameter.Value;
         set
         {
            /*nothing to do here*/
         }
      }

      public double GainPerCharge
      {
         get => GainPerChargeParameter.Value;
         set
         {
            /*nothing to do here*/
         }
      }
   }
}