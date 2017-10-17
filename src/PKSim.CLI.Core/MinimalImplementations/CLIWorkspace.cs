using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Journal;
using OSPSuite.Presentation.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIWorkspace : IWorkspace
   {
      private readonly IRegistrationTask _registrationTask;
      private PKSimProject _project;

      public CLIWorkspace(IRegistrationTask registrationTask)
      {
         _registrationTask = registrationTask;
      }

      public void AddCommand(ICommand command)
      {
      }

      public IEnumerable<ICommand> All()
      {
         return Enumerable.Empty<ICommand>();
      }

      public IWorkspaceLayout WorkspaceLayout { get; set; } = new WorkspaceLayout();

      public void Clear()
      {
      }

      public void LockFile(string fullPath)
      {
      }

      public void AccessFile(string fileFullPath)
      {
      }

      public void UpdateJournalPathRelativeTo(string projectFileFullPath)
      {
      }

      public Journal Journal { get; set; }
      public bool ProjectIsReadOnly { get; set; }

      public PKSimProject Project
      {
         get => _project;
         set
         {
            _registrationTask.UnregisterProject(_project);
            _project = value;
            _registrationTask.RegisterProject(_project);
         }
      }

      public IHistoryManager HistoryManager { get; set; } = new NullHistoryManager();

      public void CloseProject()
      {
      }

      public void SaveProject(string fileFullPath)
      {
      }

      public void OpenProject(string fileFullPath)
      {
      }

      public void LoadProject(Action projectLoadAction)
      {
      }

      public void LoadProject(PKSimProject project)
      {
         Project = project;
      }

      public bool ProjectLoaded => Project != null;
      public bool ProjectHasChanged => Project?.HasChanged ?? false;
   }
}