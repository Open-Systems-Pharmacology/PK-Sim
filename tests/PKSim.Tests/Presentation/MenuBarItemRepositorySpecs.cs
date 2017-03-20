using System.Linq;
using System.Windows.Forms;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Container;
using FakeItEasy;
using PKSim.Presentation.Core;
using OSPSuite.Presentation.Repositories;
using MenuBarItemRepository = PKSim.Presentation.Repositories.MenuBarItemRepository;

namespace PKSim.Presentation
{
   public abstract class concern_for_MenuBarItemRepository : ContextSpecificationWithLocalContainer<IMenuBarItemRepository>
   {
      protected override void Context()
      {
         _container = A.Fake<IContainer>();
         sut = new MenuBarItemRepository(_container);
      }
   }

   public class When_retrieving_a_menu_bar_item_for_a_valid_name : concern_for_MenuBarItemRepository
   {
      private IMenuBarItem _result;

      protected override void Because()
      {
         _result = sut[MenuBarItemIds.NewProject];
      }

      [Observation]
      public void should_return_a_valid_bar_item()
      {
         _result.ShouldNotBeNull();
      }

      [Observation]
      public void the_returnedbar_item_should_be_in_the_collection_of_all_available_menus()
      {
         sut.All().Any(item => item.Caption.Equals(_result.Caption)).ShouldBeTrue();
      }
   }

   public class When_creating_all_menu_bar_items_defined_in_the_appolication : concern_for_MenuBarItemRepository
   {
      private Cache<Keys, IMenuBarItem> _allEntriesWithShortCuts;

      protected override void Context()
      {
         base.Context();
         _allEntriesWithShortCuts = new Cache<Keys, IMenuBarItem>(x => x.Shortcut);
      }

      protected override void Because()
      {
         var allMenuWithShortCuts = sut.All().Where(x => x.Shortcut != Keys.None);
         _allEntriesWithShortCuts.AddRange(allMenuWithShortCuts);
      }

      [Observation]
      public void should_not_create_two_entries_with_the_same_shortuct()
      {
         _allEntriesWithShortCuts.ShouldNotBeEmpty();
      }

      [Observation]
      public void should_not_use_reserved_shortcuts()
      {
         _allEntriesWithShortCuts.Contains(Keys.Control | Keys.Shift | Keys.C).ShouldBeFalse();
         _allEntriesWithShortCuts.Contains(Keys.Control | Keys.C).ShouldBeFalse();
         _allEntriesWithShortCuts.Contains(Keys.Control | Keys.A).ShouldBeFalse();
         _allEntriesWithShortCuts.Contains(Keys.Control | Keys.F).ShouldBeFalse();
      }
   }
}