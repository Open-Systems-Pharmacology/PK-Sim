using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Repositories;
using OSPSuite.Utility.Extensions;
using ButtonGroupRepository = PKSim.Presentation.Repositories.ButtonGroupRepository;


namespace PKSim.Presentation
{
   public abstract class concern_for_ButtonGroupRepository : ContextSpecification<IButtonGroupRepository>
   {
      private IMenuBarItemRepository _menuBarItemRepository;

      protected override void Context()
      {
         _menuBarItemRepository = A.Fake<IMenuBarItemRepository>();
         sut = new ButtonGroupRepository(_menuBarItemRepository);
      }
   }

   public class When_retrieving_all_available_tool_bars_for_the_application : concern_for_ButtonGroupRepository
   {
      private IList<IButtonGroup> _result;

      [Observation]
      public void should_return_at_least_one_tool_bar()
      {
         _result.Count.ShouldNotBeEqualTo(0);
      }

      [Observation]
      public void each_tool_bar_should_have_at_least_one_button()
      {
         _result.Each(tb => tb.Buttons.Count().ShouldNotBeEqualTo(0));
      }

      protected override void Because()
      {
         _result = sut.All().ToList();
      }
   }
}