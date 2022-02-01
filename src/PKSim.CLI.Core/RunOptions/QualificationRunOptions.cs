namespace PKSim.CLI.Core.RunOptions
{
   public class QualificationRunOptions
   {
      /// <summary>
      ///    Json configuration file that will be converted into a configuration object
      /// </summary>
      public string ConfigurationFile { get; set; }

      /// <summary>
      ///    Specifies a validation run (e.g. no actual calculation will be performed.). Default is <c>false</c>
      /// </summary>
      public bool Validate { get; set; }

      /// <summary>
      ///    Should simulation be performed as part of the run?
      /// </summary>
      public bool Run { get; set; }

      /// <summary>
      /// Specifies if project files (snapshot and project should be exported)
      /// </summary>
      public bool ExportProjectFiles { get; set; }
   }
}