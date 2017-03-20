using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_DistributionWithCalculationMethodGroupPresenter : ContextSpecification<DistributionWithCalculationMethodGroupPresenter>
   {
      protected ICalculationMethodSelectionPresenterForCompound _calculationMethodSelectionPresenter;
      protected IOrganPermeabilityGroupPresenter _organPermeabilityGroupPresenter;
      protected IMultiplePanelView _multiplePanelView;
      private ICompoundVSSPresenter _compoundVSSPresenter;

      protected override void Context()
      {
         _calculationMethodSelectionPresenter = A.Fake<ICalculationMethodSelectionPresenterForCompound>();
         _organPermeabilityGroupPresenter = A.Fake<IOrganPermeabilityGroupPresenter>();
         _multiplePanelView = A.Fake<IMultiplePanelView>();
         _compoundVSSPresenter= A.Fake<ICompoundVSSPresenter>();
         sut = new DistributionWithCalculationMethodGroupPresenter(_multiplePanelView, _organPermeabilityGroupPresenter, _calculationMethodSelectionPresenter,_compoundVSSPresenter);
      }
   }

   public class When_releasing_the_compound_distribution_presenter : concern_for_DistributionWithCalculationMethodGroupPresenter
   {
      private IEventPublisher _eventPublisher;

      protected override void Context()
      {
         base.Context();
         _eventPublisher = A.Fake<IEventPublisher>();
      }

      protected override void Because()
      {
         sut.ReleaseFrom(_eventPublisher);
      }

      [Observation]
      public void should_call_permeability_presenter_release_from()
      {
         A.CallTo(() => _organPermeabilityGroupPresenter.ReleaseFrom(_eventPublisher)).MustHaveHappened();
      }

      [Observation]
      public void should_call_calculation_methods_presenter_release_from()
      {
         A.CallTo(() => _calculationMethodSelectionPresenter.ReleaseFrom(_eventPublisher)).MustHaveHappened();
      }
   }

   public class When_editing_the_distribution_parameters_defined_in_a_given_compound : concern_for_DistributionWithCalculationMethodGroupPresenter
   {
      private Compound _compound;
      private Func<CalculationMethodCategory, bool> _predicate;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound();
         A.CallTo(() => _calculationMethodSelectionPresenter.Edit(A<Compound>._, A<Func<CalculationMethodCategory, bool>>._))
            .Invokes(x => _predicate = x.GetArgument<Func<CalculationMethodCategory, bool>>(1));
      }

      protected override void Because()
      {
         sut.EditCompound(_compound);
      }

    
      [Observation]
      public void should_edit_the_calculation_methods_for_distributions_only()
      {
         _predicate(new CalculationMethodCategory().WithName(CoreConstants.Category.DistributionCellular)).ShouldBeTrue();
         _predicate(new CalculationMethodCategory().WithName(CoreConstants.Category.DistributionInterstitial)).ShouldBeTrue();
         _predicate(new CalculationMethodCategory().WithName(CoreConstants.Category.DiffusionIntCell)).ShouldBeTrue();
      }

      [Observation]
      public void should_edit_the_organ_permeability()
      {
         A.CallTo(() => _organPermeabilityGroupPresenter.EditCompound(_compound)).MustHaveHappened();
      }
   }

   public class When_initializing_the_compound_distribution_presenter : concern_for_DistributionWithCalculationMethodGroupPresenter
   {
      private ICommandCollector _commandCollector;

      protected override void Context()
      {
         base.Context();
         _commandCollector = A.Fake<ICommandCollector>();
      }

      protected override void Because()
      {
         sut.InitializeWith(_commandCollector);
      }

      [Observation]
      public void should_call_permeability_presenter_to_initialize_collector()
      {
         A.CallTo(() => _organPermeabilityGroupPresenter.InitializeWith(sut)).MustHaveHappened();
      }

      [Observation]
      public void should_call_calculation_methods_presenter_to_initialize_collector()
      {
         A.CallTo(() => _calculationMethodSelectionPresenter.InitializeWith(sut)).MustHaveHappened();
      }
   }
}
