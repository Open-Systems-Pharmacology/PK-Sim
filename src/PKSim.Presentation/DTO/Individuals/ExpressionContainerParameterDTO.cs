using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation.DTO.Individuals
{
   public class ExpressionContainerParameterDTO : ValidatableDTO
   {
      public ExpressionContainerParameterDTO()
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
      public string ParameterPath { get; set; }
      public int Sequence { get; set; }

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

      public bool Visible { get; set; }

      public override string ToString()
      {
         return $"{ContainerName} - {CompartmentName} - {ParameterName}";
      }
   }
}