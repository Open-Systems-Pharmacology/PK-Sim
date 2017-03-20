using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using OSPSuite.Core.Journal;
using OSPSuite.Presentation.Core;

namespace PKSim.Matlab
{
   public class MatlabWorkspace : IWorkspace
   {
      public void AddCommand(ICommand command)
      {
         /*nothing to do*/
      }

      public IEnumerable<ICommand> All()
      {
         return Enumerable.Empty<ICommand>();
      }

      public IPKSimProject Project { get; set; }
      public void Clear()
      {
         /*nothing to do*/
      }

      public void AccessFile(string fileFullPath)
      {
         
      }

      public void UpdateJournalPathRelativeTo(string projectFileFullPath)
      {
         /*nothing to do*/
      }

      public Journal Journal { get; set; }
      public IHistoryManager HistoryManager { get; set; }
      public IWorkspaceLayout WorkspaceLayout { get; set; }
      public bool ProjectIsReadOnly { get; set; }

      public void CloseProject()
      {
         /*nothing to do*/
      }

      public void SaveProject(string fileFullPath)
      {
         /*nothing to do*/
      }

      public void OpenProject(string fileFullPath)
      {
         /*nothing to do*/
      }

      public bool ProjectLoaded
      {
         get { return true; }
      }


      public void LockFile(string fullPath)
      {
         /*nothing to do*/
      }

      public bool ProjectHasChanged
      {
         get { return false; }
      }
   }
}