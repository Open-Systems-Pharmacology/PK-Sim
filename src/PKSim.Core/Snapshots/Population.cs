using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class Population : SnapshotBase
   {
      public int Seed { get; set; }
      public int? NumberOfIndividuals { get; set; }
      public double? ProportionOfFemales { get; set; }
      public ParameterRange Age { get; set; }
      public ParameterRange Weight { get; set; }
      public ParameterRange Height { get; set; }
      public ParameterRange GestationalAge { get; set; }
      public ParameterRange BMI { get; set; }
      public List<AdvancedParameter> AdvancedParameters { get; set; }
      public Individual Individual { get; set; }
   }
}