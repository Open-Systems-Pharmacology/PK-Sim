namespace PKSim.Core.Model
{
   public class TransportTemplate
   {
      public TransportDirectionId TransportDirection { get; set; }
      public string ProcessName { get; set; }
      public string SourceOrgan { get; set; }
      public string SourceCompartment { get; set; }
      public string TargetOrgan { get; set; }
      public string TargetCompartment { get; set; }
   }
}