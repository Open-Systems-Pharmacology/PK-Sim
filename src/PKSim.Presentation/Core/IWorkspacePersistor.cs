namespace PKSim.Presentation.Core
{
   public interface IWorkspacePersistor
   {
      void SaveSession(IWorkspace workspace, string fileFullPath);
      void LoadSession(IWorkspace workspace, string fileFullPath);
      void CloseSession();
   }
}