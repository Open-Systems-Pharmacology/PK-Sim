using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Simulations
{
   public class ParameterCommitDTO : DxValidatableDTO
   {
      public string Path { get; set; }
      public string DisplayPath { get; set; }
      public double Value { get; set; }
      public bool Selected { get; set; } = true;
   }
}
