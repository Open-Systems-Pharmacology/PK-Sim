using System;
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
      ///    Opens the project located at <paramref name="fileFullPath"/>
      /// </summary>
      /// <param name="fileFullPath">Full path of the file where the project is located</param>
      void OpenProject(string fileFullPath);

      /// <summary>
      ///    Loads the project using the <paramref name="projectLoadAction"/>. The <paramref name="projectLoadAction"/> is responsible to 
      /// call <c>Workspace.Project = project</c>
      /// </summary>
      void LoadProject(Action projectLoadAction);

      /// <summary>
      ///    Loads the project <paramref name="project"/> given as parameter
      /// </summary>
      void LoadProject(PKSimProject project);

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