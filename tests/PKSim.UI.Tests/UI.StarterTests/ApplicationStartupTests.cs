using System.Threading;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.UI.Views;
using OSPSuite.Utility.Container;
using PKSim.Core.Mappers;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.UI.Starter;

namespace PKSim.UI.UI.StarterTests
{
   public class concern_for_ApplicationStartup : StaticContextSpecification
   {
      protected IContainer _container;

      protected override void Context()
      {
         SynchronizationContext.SetSynchronizationContext(new When_resolving_the_individual_presenter.TestSynchronizationContext());
         _container = ApplicationStartup.Initialize(new BaseShell());
      }
   }

   public class When_resolving_the_expression_profile_presenter : concern_for_ApplicationStartup
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

   public class When_resolving_the_individual_presenter : concern_for_ApplicationStartup
   {
      private ICreateIndividualPresenter _presenter;

      protected override void Because()
      {
         _presenter = _container.Resolve<ICreateIndividualPresenter>();
      }

      [Observation]
      public void the_presenter_should_be_resolved()
      {
         _presenter.ShouldBeAnInstanceOf<CreateIndividualPresenter>();
      }

      public class When_resolving_the_individual_mapper : concern_for_ApplicationStartup
      {
         private IIndividualToIndividualBuildingBlockMapper _mapper;

         protected override void Because()
         {
            _mapper = _container.Resolve<IIndividualToIndividualBuildingBlockMapper>();
         }

         [Observation]
         public void the_mapper_should_be_resolved()
         {
            _mapper.ShouldBeAnInstanceOf<IndividualToIndividualBuildingBlockMapper>();
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
}