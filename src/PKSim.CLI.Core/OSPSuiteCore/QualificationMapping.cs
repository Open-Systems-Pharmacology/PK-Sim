namespace OSPSuite.Core.Qualification
{
   public class SimulationMapping
   {
      public string RefProject { get; set; }
      public string RefSimulation { get; set; }
      public string Path { get; set; }
   }

   public class ObservedDataMapping
   {
      public string Id { get; set; }
      public string Path { get; set; }
      public string Type { get; set; }
   }

   public class PlotMapping
   {
      public int SectionId { get; set; }
      public string RefProject { get; set; }
      public string RefSimulation { get; set; }
      public object Plot { get; set; }
   }

   public class QualificationMapping
   {
      public SimulationMapping[] SimulationMappings { get; set; }
      public ObservedDataMapping[] ObservedDataMappings { get; set; }
      public PlotMapping[] Plots { get; set; }
      public InputMapping[] Inputs { get; set; }
   }

   public class InputMapping
   {
      public int SectionId { get; set; }
      public string Path { get; set; }
   }
}