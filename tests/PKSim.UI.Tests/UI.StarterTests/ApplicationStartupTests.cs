using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Container;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.UI.Starter;
using System.Threading;

namespace PKSim.UI.UI.StarterTests
{
   public class concern_for_ApplicationStartup : StaticContextSpecification
   {
      private IContainer _container;
      private ICreateExpressionProfilePresenter _presenter;

      protected override void Context()
      {
         SynchronizationContext.SetSynchronizationContext(new TestSynchronizationContext());
         _container = ApplicationStartup.Initialize(A.Fake<IShell>());
      }

      protected override void Because()
      {
         _presenter = _container.Resolve<ICreateExpressionProfilePresenter>();
      }

      [Observation]
      public void the_presenter_should_be_resolved()
      {
         _presenter.ShouldBeAnInstanceOf<CreateExpressionProfilePresenter>();
      }
   }

   public class TestSynchronizationContext : SynchronizationContext
   {
   }
}
