﻿namespace OSPSuite.Core.Qualification
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


   public class ChartMapping
   {
      public int SectionId { get; set; }
      public string RefProject { get; set; }
      public string RefSimulation { get; set; }
      public object Chart { get; set; }
   }

   public class QualificationMapping
   {
      public SimulationMapping[] SimulationMappings { get; set; }
      public ObservedDataMapping[] ObservedDataMappings { get; set; }
      public ChartMapping[] Charts { get; set; }
   }
}