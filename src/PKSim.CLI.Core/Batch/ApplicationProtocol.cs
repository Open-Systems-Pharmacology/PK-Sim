namespace PKSim.Core.Batch
{
   internal class ApplicationProtocol
   {
      public string Name { get; set; }
      public string CompoundName { get; set; }
      public string DosingInterval { get; set; }
      public string ApplicationType { get; set; }
      public double EndTime { get; set; }
      public double Dose { get; set; }
      public string DoseUnit { get; set; }
   }
}