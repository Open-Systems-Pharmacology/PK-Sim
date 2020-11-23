using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Individuals
{
   public class TransporterExpressionParameterDTO : ExpressionParameterDTO
   {
      //Save a property so that it does not have to be computed every time
      public bool IsInOrganWithLumen { get; set; }
      public TransporterExpressionContainer TransporterExpressionContainer { get; set; }
      public TransportDirection TransportDirection { get; set; }
   }
}