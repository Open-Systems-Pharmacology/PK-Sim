using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Simulations
{
   public class ParameterCommitDTO : DxValidatableDTO
   {
      public string Path { get; init; }
      public string DisplayPath { get; init; }
      public double Value { get; init; }
      public string Unit { get; init; }
      public string ValueOrigin { get; init; }
      public bool Selected { get; set; } = true;
   }
}
