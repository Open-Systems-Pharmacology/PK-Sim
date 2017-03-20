namespace PKSim.BatchTool
{
   public class BatchStartOptions
   {
      public string InputFolder { get; private set; }
      public string OutputFolder { get; private set; }

       public static BatchStartOptions From(string[] args)
       {
          var options= new BatchStartOptions();
          if (args.Length != 4)
             return options;

          for (int i = 0; i < args.Length-1; i++)
          {
             if (args[i] == "--input")
                options.InputFolder = args[i + 1];

             else if (args[i] == "--output")
                options.OutputFolder = args[i + 1];
          }

          return options;
       }

       public virtual bool IsValid => !string.IsNullOrEmpty(InputFolder) && !string.IsNullOrEmpty(OutputFolder);
   }
}