﻿namespace PKSim.Core.Snapshots
{
   public class ParameterIdentification : SnapshotBase
   {
      public string[] Simulations { get; set; }
      public ParameterIdentificationConfiguration Configuration { get; set; }
      public OutputMapping[] OutputMappings { get; set; }
      public IdentificationParameter[] IdentificationParameters { get; set; }
      public ParameterIdentificationAnalysis[] Analyses { get; set; }
   }
}