namespace PKSim.Core.Services
{
   public interface IWorkspacePersistor
   {
      void SaveSession(ICoreWorkspace workspace, string fileFullPath);
      void LoadSession(ICoreWorkspace workspace, string fileFullPath);
      void CloseSession();
   }
}