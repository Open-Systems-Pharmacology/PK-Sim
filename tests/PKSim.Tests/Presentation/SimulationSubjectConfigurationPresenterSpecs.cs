using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationSubjectConfigurationPresenter : ContextSpecification<ISimulationSubjectConfigurationPresenter>
   {
      protected ISimulationSubjectConfigurationView _view;
      protected Simulation _simulation;
      protected Species _species;
      protected IModel _model;
      protected ICreateSimulationPresenter _simulationPresenter;
      protected ISimulationFactory _simulationFactory;
      protected ILazyLoadTask _lazyLoadTask;
      private IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      protected SimulationSubjectDTO _simulationSubjectDTO;
      protected ISimulationSubject _subject;

      protected override void Context()
      {
         _simulationPresenter = A.Fake<ICreateSimulationPresenter>();
         _view = A.Fake<ISimulationSubjectConfigurationView>();
         _simulationFactory = A.Fake<ISimulationFactory>();
         _buildingBlockInProjectManager = A.Fake<IBuildingBlockInProjectManager>();
         _simulation = A.Fake<Simulation>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         sut = new SimulationSubjectConfigurationPresenter(_view, _lazyLoadTask, _buildingBlockInProjectManager);
         A.CallTo(() => _view.BindTo(A<SimulationSubjectDTO>._))
            .Invokes(x => _simulationSubjectDTO = x.GetArgument<SimulationSubjectDTO>(0));

         sut.InitializeWith(_simulationPresenter);

         _subject = A.Fake<ISimulationSubject>();
         var originData = new OriginData { SpeciesPopulation = A.Fake<SpeciesPopulation>() };
         A.CallTo(() => _subject.OriginData).Returns(originData);

         sut.Initialize();
      }
   }

   public class When_the_simulation_model_configuration_presenter_is_told_to_prepare_for_the_creation_of_a_simulation : concern_for_SimulationSubjectConfigurationPresenter
   {
    
      [Observation]
      public void should_bind_the_view_to_the_individual_selection()
      {
         A.CallTo(() => _view.BindTo(A<SimulationSubjectDTO>._)).MustHaveHappened();
      }
   }

   public class When_the_selected_simulation_subject_is_not_age_dependent : concern_for_SimulationSubjectConfigurationPresenter
   {
      protected override void Context()
      {
         base.Context();
         _subject.OriginData.SpeciesPopulation.IsAgeDependent = false;
      }

      protected override void Because()
      {
         sut.UpdateSelectedSubject(_subject);
      }

      [Observation]
      public void should_hide_the_allow_aging_option()
      {
         _view.AllowAgingVisible.ShouldBeFalse();
      }

      [Observation]
      public void should_set_the_allow_aging_option_to_false()
      {
         _simulationSubjectDTO.AllowAging.ShouldBeFalse();
      }
   }

   public class When_the_selected_simulation_subject_is_age_dependent_and_preterm : concern_for_SimulationSubjectConfigurationPresenter
   {

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _subject.IsAgeDependent).Returns(true);
         A.CallTo(() => _subject.IsPreterm).Returns(true);
      }

      protected override void Because()
      {
         sut.UpdateSelectedSubject(_subject);
      }

      [Observation]
      public void should_show_the_allow_aging_option()
      {
         _view.AllowAgingVisible.ShouldBeTrue();
      }

      [Observation]
      public void should_set_the_allow_aging_option_per_default_to_true()
      {
         _simulationSubjectDTO.AllowAging.ShouldBeTrue();
      }
   }

   public class When_the_selected_simulation_subject_is_age_dependent_and_not_preterm : concern_for_SimulationSubjectConfigurationPresenter
   {

      protected override void Context()
      {
         base.Context();

         A.CallTo(() => _subject.IsAgeDependent).Returns(true);
         A.CallTo(() => _subject.IsPreterm).Returns(false);
      }

      protected override void Because()
      {
         sut.UpdateSelectedSubject(_subject);
      }

      [Observation]
      public void should_show_the_allow_aging_option()
      {
         _view.AllowAgingVisible.ShouldBeTrue();
      }

      [Observation]
      public void should_set_the_allow_aging_option_per_default_to_false()
      {
         _simulationSubjectDTO.AllowAging.ShouldBeFalse();
      }
   }

   public class When_the_selected_simulation_subject_is_age_dependent_and_not_preterm_and_the_aging_box_was_selected : concern_for_SimulationSubjectConfigurationPresenter
   {

      protected override void Context()
      {
         base.Context();

         var previousSubject = A.Fake<ISimulationSubject>();
         var previousOriginData = new OriginData { SpeciesPopulation = A.Fake<SpeciesPopulation>() };
         A.CallTo(() => previousSubject.IsPreterm).Returns(true);
         A.CallTo(() => previousSubject.IsAgeDependent).Returns(true);
         A.CallTo(() => previousSubject.OriginData).Returns(previousOriginData);


         A.CallTo(() => _subject.IsAgeDependent).Returns(true);
         A.CallTo(() => _subject.IsPreterm).Returns(false);
         sut.UpdateSelectedSubject(previousSubject);
      }

      protected override void Because()
      {
         sut.UpdateSelectedSubject(_subject);
      }

      [Observation]
      public void should_show_the_allow_aging_option()
      {
         _view.AllowAgingVisible.ShouldBeTrue();
      }

      [Observation]
      public void should_have_changed_the_value_of_aging()
      {
         _simulationSubjectDTO.AllowAging.ShouldBeFalse();
      }
   }
}