using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Individuals
{
   public class TransporterExpressionParameterDTO : ExpressionParameterDTO
   {
      public TransporterExpressionContainer TransporterExpressionContainer { get; set; }
      public TransportDirection TransportDirection { get; set; }
   }
}