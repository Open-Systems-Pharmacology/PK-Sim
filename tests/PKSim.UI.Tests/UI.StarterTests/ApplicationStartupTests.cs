using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Container;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.UI.Starter;
using System.Threading;
using PKSim.Core.Mappers;

namespace PKSim.UI.UI.StarterTests
{
   public class concern_for_ApplicationStartup : StaticContextSpecification
   {
      protected IContainer _container;

      protected override void Context()
      {
         SynchronizationContext.SetSynchronizationContext(new TestSynchronizationContext());
         _container = ApplicationStartup.Initialize(A.Fake<IShell>());
      }
   }

   public class When_resolving_the_presenter: concern_for_ApplicationStartup
   {
      private ICreateExpressionProfilePresenter _presenter;

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

   public class When_resolving_the_building_block_mapper : concern_for_ApplicationStartup
   {
      private IExpressionProfileToExpressionProfileBuildingBlockMapper _mapper;

      protected override void Because()
      {
         _mapper = _container.Resolve<IExpressionProfileToExpressionProfileBuildingBlockMapper>();
      }

      [Observation]
      public void the_mapper_should_be_resolved()
      {
         _mapper.ShouldBeAnInstanceOf<ExpressionProfileToExpressionProfileBuildingBlockMapper>();
      }
   }

   public class TestSynchronizationContext : SynchronizationContext
   {
   }
}
