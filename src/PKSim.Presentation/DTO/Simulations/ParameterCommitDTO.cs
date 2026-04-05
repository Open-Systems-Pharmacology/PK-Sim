namespace PKSim.Presentation.DTO.Simulations
{
   public class ParameterCommitDTO
   {
      public string Path { get; init; }
      public string DisplayPath { get; set; }
      public double Value { get; init; }
      public bool Selected { get; set; } = true;
   }
}
