namespace PKSim.Core.Services
{
   public interface IPersistor<T>
   {
      void Save(T target);
      T Load();
   }
}