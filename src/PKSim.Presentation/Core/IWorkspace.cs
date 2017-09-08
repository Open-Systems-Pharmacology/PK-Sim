using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Services;
using PKSim.Core.Model;

namespace PKSim.Presentation.Core
{
   public interface IWorkspace : ICommandCollector, IWithWorkspaceLayout, OSPSuite.Core.IWorkspace
   {
      /// <summary>
      ///    Returns/sets the current project
      /// </summary>
      PKSimProject Project { get; set; }

      /// <summary>
      ///    Returns/sets  the history manager
      /// </summary>
      IHistoryManager HistoryManager { get; set; }

      /// <summary>
      ///    Close the current project
      /// </summary>
      void CloseProject();

      /// <summary>
      ///    Save the current project
      /// </summary>
      /// <param name="fileFullPath">Full path of the file where the project should be saved</param>
      void SaveProject(string fileFullPath);

      /// <summary>
      ///    Save the current project
      /// </summary>
      /// <param name="fileFullPath">Full path of the file where the project should be saved</param>
      void OpenProject(string fileFullPath);

      /// <summary>
      ///    Returns true if the project has been defined
      /// </summary>
      bool ProjectLoaded { get; }

      /// <summary>
      ///    Returns true if the project has changed and thus should be for instance saved otherwise false
      /// </summary>
      bool ProjectHasChanged { get; }
   }
}