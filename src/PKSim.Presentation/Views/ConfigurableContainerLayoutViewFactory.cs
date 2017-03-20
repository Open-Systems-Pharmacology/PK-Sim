using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Views
{
   public interface IConfigurableContainerLayoutViewFactory
   {
      IConfigurableContainerLayoutView Create();
   }

   public class ConfigurableContainerLayoutViewFactory : IConfigurableContainerLayoutViewFactory
   {
      private readonly IContainer _container;
      private readonly IUserSettings _userSettings;

      public ConfigurableContainerLayoutViewFactory(IContainer container, IUserSettings userSettings)
      {
         _container = container;
         _userSettings = userSettings;
      }

      public IConfigurableContainerLayoutView Create()
      {
         return _container.Resolve<IConfigurableContainerLayoutView>(_userSettings.PreferredViewLayout.Id);
      }
   }
}