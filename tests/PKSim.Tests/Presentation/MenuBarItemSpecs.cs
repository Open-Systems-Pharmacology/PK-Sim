using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Assets;

namespace PKSim.Presentation
{
   public abstract class concern_for_MenuBarItem : ContextSpecificationWithLocalContainer<MenuBarButton>
   {
      protected string _name;
      protected ApplicationIcon _icon;
      protected IUICommand _command;

      protected override void Context()
      {
         _name = "toto";
         _icon = ApplicationIcons.ProjectOpen;
         _command = A.Fake<IUICommand>();
         sut = new MenuBarButton {Caption = _name, Command = _command, Icon = _icon};
      }
   }

   public class When_asked_for_the_menu_bar_item_properties : concern_for_MenuBarItem
   {
      [Observation]
      public void should_return_the_properties_it_was_intialized_with()
      {
         sut.Caption.ShouldBeEqualTo(_name);
         sut.Icon.ShouldBeEqualTo(_icon);
      }
   }

   public class When_the_menu_bar_item_is_clicked : concern_for_MenuBarItem
   {
      protected override void Because()
      {
         sut.Click();
      }

      [Observation]
      public void should_execute_the_underlying_command()
      {
         A.CallTo(() => _command.Execute()).MustHaveHappened();
      }
   }
}