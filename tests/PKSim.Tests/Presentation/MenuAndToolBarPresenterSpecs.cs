using OSPSuite.BDDHelper;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Presentation.Presenters.Main;
using OSPSuite.Core;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Repositories;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;
using IWorkspace = PKSim.Presentation.Core.IWorkspace;

namespace PKSim.Presentation
{
   public abstract class concern_for_MenuAndToolBarPresenter : ContextSpecification<IMenuAndToolBarPresenter>
   {
      protected IMenuAndToolBarView _view;
      protected IMenuBarItemRepository _menuBarItemRepository;
      protected IButtonGroupRepository _buttonGroupRepository;
      protected IMRUProvider _mruProvider;
      protected ISkinManager _skinManager;
      protected IWorkspace _workspace;
      protected IActiveSubjectRetriever _activeSubjectRetriever;
      private IStartOptions _startOptions;

      protected override void Context()
      {
         _view = A.Fake<IMenuAndToolBarView>();
         _menuBarItemRepository = A.Fake<IMenuBarItemRepository>();
         _buttonGroupRepository = A.Fake<IButtonGroupRepository>();
         _mruProvider = A.Fake<IMRUProvider>();
         _skinManager = A.Fake<ISkinManager>();
         _workspace = A.Fake<IWorkspace>();
         _activeSubjectRetriever = A.Fake<IActiveSubjectRetriever>();
         _startOptions= A.Fake<IStartOptions>();

         sut = new MenuAndToolBarPresenter(_view, _menuBarItemRepository, _buttonGroupRepository, _mruProvider, _skinManager, _startOptions, _workspace, _activeSubjectRetriever);


         A.CallTo(() => _menuBarItemRepository[A<MenuBarItemId>._]).ReturnsLazily(item =>
         {
            {
               var id = item.Arguments[0].DowncastTo<MenuBarItemId>();
               return FindMenuById(id);
            }
         });
      }

      protected virtual MenuBarButton FindMenuById(MenuBarItemId id)
      {
         return new MenuBarButton();
      }
   }
  
}