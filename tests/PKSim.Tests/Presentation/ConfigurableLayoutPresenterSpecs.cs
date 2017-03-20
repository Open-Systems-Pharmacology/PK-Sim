using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation
{
   public abstract class concern_for_ConfigurableLayoutPresenter : ContextSpecification<ConfigurableLayoutPresenter>
   {
      protected IConfigurableLayoutView _configurableLayoutView;
      protected IConfigurableContainerLayoutView _configurableContainerLayoutView;
      private IConfigurableContainerLayoutViewFactory _configurableContainerLayoutViewFactory;

      protected override void Context()
      {
         _configurableLayoutView = A.Fake<IConfigurableLayoutView>();
         _configurableContainerLayoutViewFactory= A.Fake<IConfigurableContainerLayoutViewFactory>();
         _configurableContainerLayoutView= A.Fake<IConfigurableContainerLayoutView>();
         A.CallTo(() => _configurableContainerLayoutViewFactory.Create()).Returns(_configurableContainerLayoutView);
         sut = new ConfigurableLayoutPresenter(_configurableLayoutView,_configurableContainerLayoutViewFactory);
      }
   }

   public class when_adding_multiple_views : concern_for_ConfigurableLayoutPresenter
   {
      private List<IView> _views;

      protected override void Context()
      {
         base.Context();
         _views = new List<IView> { A.Fake<IView>(), A.Fake<IView>() }; 
      }

      protected override void Because()
      {
         sut.AddViews(_views);
      }

      [Observation]
      public void views_must_each_be_added_to_the_layout()
      {
         A.CallTo(() => _configurableContainerLayoutView.AddView(_views[0])).MustHaveHappened();
         A.CallTo(() => _configurableContainerLayoutView.AddView(_views[1])).MustHaveHappened();
      }
   }

   public class when_adding_single_view : concern_for_ConfigurableLayoutPresenter
   {
      private List<IView> _views;

      protected override void Context()
      {
         base.Context();
         _views = new List<IView> { A.Fake<IView>() };
      }

      protected override void Because()
      {
         sut.AddViews(_views);
      }

      [Observation]
      public void should_replace_collection_view_with_just_the_single_view()
      {
         A.CallTo(() => _configurableLayoutView.SetView(_views.First())).MustHaveHappened();
      }

      [Observation]
      public void should_not_add_views_to_collection_control()
      {
         A.CallTo(() => _configurableContainerLayoutView.AddView(A<IView>.Ignored)).MustNotHaveHappened();
      }
   }

}
