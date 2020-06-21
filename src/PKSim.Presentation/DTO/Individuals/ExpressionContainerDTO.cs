using OSPSuite.Core.Domain;
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Individuals
{
   public class ExpressionContainerDTO : ValidatableDTO
   {
      public ExpressionContainerDTO()
      {
         GroupingPathDTO = new PathElement();
         ContainerPathDTO = new PathElement();
         RelativeExpressionParameter = new NullParameterDTO();
         RelativeExpressionNormParameter = new NullParameterDTO();
      }

      public string MoleculeName { get; set; }
      public string ContainerName { get; set; }
      public PathElement GroupingPathDTO { get; set; }
      public PathElement ContainerPathDTO { get; set; }
      public IParameterDTO RelativeExpressionParameter { get; set; }
      public IParameterDTO RelativeExpressionNormParameter { get; set; }
      public string ParameterPath { get; set; }
      public int Sequence { get; set; }

      public bool IsFavorite
      {
         get => RelativeExpressionParameter.IsFavorite;
         set => RelativeExpressionParameter.IsFavorite = value;
      }

      public double RelativeExpression
      {
         get => RelativeExpressionParameter.Value;
         set => RelativeExpressionParameter.Value = value;
      }

      public double RelativeExpressionNorm => RelativeExpressionNormParameter.KernelValue * 100;

      public virtual void ClearReferences()
      {
         RelativeExpressionParameter = null;
         RelativeExpressionNormParameter = null;
      }
   }
}