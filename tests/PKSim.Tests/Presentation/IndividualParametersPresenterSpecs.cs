using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Individuals;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualParametersPresenter : ContextSpecification<IIndividualParametersPresenter>
   {
      protected IIndividualParametersView _view;
      protected Individual _individual;
      protected IIndividualPresenter _individualPresenter;
      protected Organism _organism;
      protected IParameterGroupsPresenter _parameterGroupPresenter;

      protected override void Context()
      {
         _view = A.Fake<IIndividualParametersView>();
         _parameterGroupPresenter = A.Fake<IParameterGroupsPresenter>();
         A.CallTo(() => _parameterGroupPresenter.View).Returns(A.Fake<IParameterGroupsView>());
         _organism = A.Fake<Organism>();
         _individual = new Individual();
         _individual.Add(_organism);
         _individualPresenter = A.Fake<IIndividualPresenter>();
         A.CallTo(() => _individualPresenter.Individual).Returns(_individual);
         sut = new IndividualParametersPresenter(_view, _parameterGroupPresenter);
         sut.InitializeWith(_individualPresenter);
      }
   }

   public class When_the_individual_parameter_view_is_dirty : concern_for_IndividualParametersPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.HasError).Returns(true);
      }

      [Observation]
      public void it_should_not_be_able_to_clse()
      {
         sut.CanClose.ShouldBeFalse();
      }
   }

   public class When_the_individual_parameter_presenter_is_asked_for_the_its_view : concern_for_IndividualParametersPresenter
   {
      [Observation]
      public void should_return_the_view()
      {
         sut.BaseView.ShouldBeEqualTo(_view);
      }
   }

   public class When_the_individual_parameter_is_being_constructed : concern_for_IndividualParametersPresenter
   {
      [Observation]
      public void should_update_the_view_with_the_content_of_th_parameter_group_presenter()
      {
         A.CallTo(() => _view.AddParametersView(_parameterGroupPresenter.View)).MustHaveHappened();
      }
   }

   public class When_editing_the_parameter_of_an_individual : concern_for_IndividualParametersPresenter
   {
      protected override void Because()
      {
         sut.EditIndividual(_individual);
      }

      [Observation]
      public void should_display_all_the_parameters_define_in_its_root_container_that_are_not_protein_parameters()
      {
         A.CallTo(() => _parameterGroupPresenter.InitializeWith(_individual, A<Func<IParameter, bool>>.Ignored)).MustHaveHappened();
      }
   }
}