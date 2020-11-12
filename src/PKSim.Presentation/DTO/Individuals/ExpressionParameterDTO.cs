using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation.DTO.Individuals
{
   public class ExpressionParameterDTO : ValidatableDTO
   {
      public ExpressionParameterDTO()
      {
         GroupingPathDTO = new PathElement();
         ContainerPathDTO = new PathElement();
         CompartmentPathDTO = new PathElement();
         Parameter = new NullParameterDTO();
      }

      public string MoleculeName { get; set; }
      public string ContainerName { get; set; }
      public string CompartmentName { get; set; }
      public PathElement GroupingPathDTO { get; set; }
      public PathElement ContainerPathDTO { get; set; }
      public PathElement CompartmentPathDTO { get; set; }
      public IParameterDTO Parameter { get; set; }

      public string ParameterName => Parameter?.Name;
      public string GroupName { get; set; }
      public string ParameterPath { get; set; }
      public int Sequence { get; set; }

      /// <summary>
      /// Normalized expression. This is only used for display and is only available for expression parameters
      /// </summary>
      public double? NormalizedExpression { get; set; }

      /// <summary>
      /// Normalized expression. This is only used for display and is only available for expression parameters
      /// </summary>
      public double? NormalizedExpressionPercent => NormalizedExpression * 100;

      public bool IsFavorite
      {
         get => Parameter.IsFavorite;
         set => Parameter.IsFavorite = value;
      }

      public double Value
      {
         get => Parameter.Value;
         set => Parameter.Value = value;
      }

      public bool Visible
      {
         get => Parameter.Parameter.Visible;
         set => Parameter.Parameter.Visible = value;
      }

      public override string ToString()
      {
         return $"{ContainerName} - {CompartmentName} - {ParameterName}";
      }
   }
}