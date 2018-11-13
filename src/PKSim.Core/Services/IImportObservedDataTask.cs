using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IImportObservedDataTask
   {
      void AddObservedDataToProject();
      void AddObservedDataToProjectForCompound(Compound compound);
   }
}