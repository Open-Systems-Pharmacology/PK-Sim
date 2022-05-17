using System.Linq;
using OSPSuite.Core;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility;

namespace PKSim.Presentation
{
   public enum StartOptionMode
   {
      Unspecified,

      //argument /p or /project
      Project,

      //argument /pop
      Population,

      //argument /j
      Journal
   }

   public class StartOptions : IStartOptions
   {
      private readonly string[] _developerFlags = {"/dev", "--dev"};

      /// <summary>
      ///    File that should be loaded automatically
      /// </summary>
      public virtual string FileToLoad { get; private set; }

      /// <summary>
      ///    Specifies if the app should be started in developer mode. Default is <c>false</c>
      /// </summary>
      public bool IsDeveloperMode { get; private set; }

      /// <summary>
      ///    Loading a project or a simulation file
      /// </summary>
      public virtual StartOptionMode StartOptionMode { get; private set; }

      public void InitializeFrom(string[] args)
      {
         if (args.Length == 0)
            return;

         updateDeveloperMode(args);

         var arguments = stripDeveloperFlagsFrom(args);
         if (arguments.Length == 0)
            return;

         if (arguments.Length == 1)
         {
            //only one argument, this is the project file
            startProject(args[0]);
            return;
         }

         var command = arguments[0];
         var file = arguments[1];

         if (isProject(command))
            startProject(file);

         else if (isPoplationSimulation(command))
            startPopulation(file);

         else if (isJournal(command))
            startJournal(file);
      }

      private string[] stripDeveloperFlagsFrom(string[] args)
      {
         return args.Except(_developerFlags).ToArray();
      }

      private void updateDeveloperMode(string[] args)
      {
         IsDeveloperMode = args.ContainsAny(_developerFlags);
      }

      private void startJournal(string journalFile) => startAs(journalFile, StartOptionMode.Journal);

      private void startPopulation(string populationFile) => startAs(populationFile, StartOptionMode.Population);

      private void startProject(string projectFile) => startAs(projectFile, StartOptionMode.Project);

      private void startAs(string fileToLoad, StartOptionMode startOptionMode)
      {
         FileToLoad = fileToLoad;
         StartOptionMode = startOptionMode;
      }

      private static bool isPoplationSimulation(string firstArgument)
      {
         return firstArgumentIsOneOf(firstArgument, "/pop", "--pop");
      }

      private static bool isProject(string firstArgument)
      {
         return firstArgumentIsOneOf(firstArgument, "/p", "/project", "-p", "--project");
      }

      private static bool isJournal(string firstArgument)
      {
         return firstArgumentIsOneOf(firstArgument, "/j", "/journal", "-j", "--journal");
      }

      private static bool firstArgumentIsOneOf(string firstArgument, params string[] listOfValues)
      {
         if (string.IsNullOrEmpty(firstArgument))
            return false;

         var lowerCaseArgument = firstArgument.ToLower();
         return lowerCaseArgument.IsOneOf(listOfValues);
      }

      public virtual bool IsValid()
      {
         return FileHelper.FileExists(FileToLoad) && StartOptionMode != StartOptionMode.Unspecified;
      }
   }
}