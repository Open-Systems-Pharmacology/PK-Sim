using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots
{
   public class PopulationSettings
   {
      [Required]
      public int NumberOfIndividuals { get; set; }
      public int? ProportionOfFemales { get; set; }
      public ParameterRange Age { get; set; }
      public ParameterRange Weight { get; set; }
      public ParameterRange Height { get; set; }
      public ParameterRange GestationalAge { get; set; }
      public ParameterRange BMI { get; set; }

      [Required]
      public Individual Individual { get; set; }
   }
}