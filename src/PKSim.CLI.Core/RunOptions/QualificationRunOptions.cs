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
   }
}