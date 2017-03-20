using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface IContentLoader
   {
      void LoadContentFor<T>(T objectToLoad) where T : IObjectBase;
   }
}