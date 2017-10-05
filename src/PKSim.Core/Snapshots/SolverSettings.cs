namespace PKSim.Core.Snapshots
{
   public class SolverSettings
   {
      public double? AbsTol { get; set; }
      public double? RelTol { get; set; }
      public bool? UseJacobian { get; set; }
      public double? H0 { get; set; }
      public double? HMin { get; set; }
      public double? HMax { get; set; }
      public int? MxStep { get; set; }
   }
}