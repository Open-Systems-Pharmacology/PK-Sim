namespace PKSim.Core.Snapshots
{
   public class PopulationSettings
   {
      public int NumberOfIndividuals { get; set; }
      public int? ProportionOfFemales { get; set; }
      public ParameterRange Age { get; set; }
      public ParameterRange Weight { get; set; }
      public ParameterRange Height { get; set; }
      public ParameterRange GestationalAge { get; set; }
      public ParameterRange BMI { get; set; }
      public Individual Individual { get; set; }
   }
}