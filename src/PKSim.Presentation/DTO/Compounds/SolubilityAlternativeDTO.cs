using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Compounds
{
   public class SolubilityAlternativeDTO : ParameterAlternativeDTO
   {
      public IParameterDTO SolubilityParameter { get; set; }
      public IParameterDTO RefpHParameter { get; set; }
      public IParameterDTO GainPerChargeParameter { get; set; }

      public SolubilityAlternativeDTO(ParameterAlternative parameterAlternative, ValueOrigin valueOrigin) : base(parameterAlternative, valueOrigin)
      {
      }

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