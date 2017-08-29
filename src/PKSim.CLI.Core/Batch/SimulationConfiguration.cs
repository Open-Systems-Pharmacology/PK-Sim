namespace PKSim.Core.Batch
{
   internal class SimulationConfiguration
   {
      public string Model { get; set; }
      public double StartTime { get; set; }
      public double EndTime { get; set; }
      public double Resolution { get; set; }
      public double AbsTol { get; set; }
      public double RelTol { get; set; }
      public bool UseJacobian { get; set; }
      public bool AllowAging { get; set; }
      public bool CheckForNegativeValues { get; set; }

      public SimulationConfiguration()
      {
         StartTime = 0;
         EndTime = CoreConstants.DEFAULT_PROTOCOL_END_TIME_IN_MIN;
         Resolution = CoreConstants.LOW_RESOLUTION_IN_PTS_PER_MIN;
         UseJacobian = true;
         AbsTol = CoreConstants.DEFAULT_ABS_TOL;
         RelTol = CoreConstants.DEFAULT_REL_TOL;
         Model = CoreConstants.Model.FourComp;
         AllowAging = false;
         CheckForNegativeValues = true;
      }
   }
}