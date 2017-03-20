using OSPSuite.BDDHelper;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundAdvancedParametersPresenter : ContextSpecification<ICompoundAdvancedParametersPresenter>
   {
      protected ICompoundAdvancedParametersView _view;
      protected ISubPresenterItemManager<ICompoundAdvancedParameterGroupPresenter> _subPresenterManager;
      protected IParticleDissolutionGroupPresenter _p1;
      protected ITwoPoreGroupPresenter _p2;

      protected override void Context()
      {
         _view = A.Fake<ICompoundAdvancedParametersView>();
         _subPresenterManager = A.Fake<ISubPresenterItemManager<ICompoundAdvancedParameterGroupPresenter>>();
         _p1 = A.Fake<IParticleDissolutionGroupPresenter>();
         _p2 = A.Fake<ITwoPoreGroupPresenter>();
         A.CallTo(() => _subPresenterManager.AllSubPresenters).Returns(new ICompoundAdvancedParameterGroupPresenter[] {_p1, _p2});
         sut = new CompoundAdvancedParametersPresenter(_view, _subPresenterManager);
      }
   }

   public class When_the_compound_advanced_parameters_presenter_is_editing_a_compound : concern_for_CompoundAdvancedParametersPresenter
   {
      private ICommandCollector _commandRegister;

      protected override void Context()
      {
         base.Context();
         _commandRegister = A.Fake<ICommandCollector>();
      }

      protected override void Because()
      {
         sut.InitializeWith(_commandRegister);
      }

      [Observation]
      public void should_create_a_sub_region_in_the_view_for_each_advanced_parameter_groups()
      {
         A.CallTo(() => _subPresenterManager.InitializeWith(sut, CompoundAdvancedParameterGroupItems.All)).MustHaveHappened();
      }

      [Observation]
      public void should_add_an_empty_place_holder_to_allow_resize()
      {
         A.CallTo(() => _view.AddEmptyPlaceHolder()).MustHaveHappened();
      }
   }

   public class When_the_compound_advance_parmaeter_presenter_is_editing_a_compound : concern_for_CompoundAdvancedParametersPresenter
   {
      private Compound _compound;

      protected override void Context()
      {
         base.Context();
         _compound = A.Fake<Compound>();
      }

      protected override void Because()
      {
         sut.EditCompound(_compound);
      }

      [Observation]
      public void should_let_the_sub_presenter_edit_the_compound_parameters_belonging_to_their_advanced_group()
      {
         A.CallTo(() => _p1.EditCompound(_compound)).MustHaveHappened();
         A.CallTo(() => _p2.EditCompound(_compound)).MustHaveHappened();
      }
   }
}