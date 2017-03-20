using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IImportObservedDataTask
   {
      void AddConcentrationDataToProject();
      void AddConcentrationDataToProjectForCompound(Compound compound);
      void AddFractionDataToProject();
   }
}