using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationCompoundConfigurationCollectorPresenter : ContextSpecification<ISimulationCompoundConfigurationCollectorPresenter>
   {
      protected IApplicationController _applicationController;
      protected IConfigurableLayoutPresenter _configurableLayoutPresenter;
      protected IEventPublisher _eventPublisher;
      private ISimulationCompoundCollectorView _view;
      protected Simulation _simulation;
      protected List<Compound> _allCompounds;
      protected Compound _compound1;
      private Compound _compound2;
      protected List<ISimulationCompoundConfigurationPresenter> _allSubPresenters;

      protected override void Context()
      {
         _applicationController = A.Fake<IApplicationController>();
         _configurableLayoutPresenter = A.Fake<IConfigurableLayoutPresenter>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _view = A.Fake<ISimulationCompoundCollectorView>();

         sut = new SimulationCompoundConfigurationCollectorPresenter(_view, _applicationController, _configurableLayoutPresenter, _eventPublisher);

         _simulation = A.Fake<Simulation>();
         _compound1 = A.Fake<Compound>().WithName("C1");
         _compound2 = A.Fake<Compound>().WithName("C2");
         _allCompounds = new List<Compound> {_compound1, _compound2};
         A.CallTo(() => _simulation.Compounds).Returns(_allCompounds);

         _allSubPresenters = new List<ISimulationCompoundConfigurationPresenter>();
         A.CallTo(() => _applicationController.Start<ISimulationCompoundConfigurationPresenter>()).ReturnsLazily(x =>
         {
            var p = A.Fake<ISimulationCompoundConfigurationPresenter>();
            _allSubPresenters.Add(p);
            return p;
         });
      }
   }

   public class When_editing_the_compound_configurations_for_a_given_simulation : concern_for_SimulationCompoundConfigurationCollectorPresenter
   {
      private IReadOnlyList<IView> _views;

      protected override void Context()
      {
         base.Context();

         A.CallTo(() => _configurableLayoutPresenter.AddViews(A<IEnumerable<IView>>._))
            .Invokes(x => _views = x.GetArgument<IEnumerable<IView>>(0).ToList());
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      [Observation]
      public void should_retrieve_a_configuration_presenter_for_each_compound_used_in_the_simulation()
      {
         _allSubPresenters.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_edit_each_sub_presenter_with_a_single_compound()
      {
         _allCompounds.Each((c, index) => A.CallTo(() => _allSubPresenters[index].EditSimulation(_simulation, _allCompounds[index])).MustHaveHappened());
      }

      [Observation]
      public void should_set_the_caption_of_the_sub_presenter_to_the_name_of_the_compound()
      {
         _allCompounds.Each((c, index) => _allSubPresenters[index].BaseView.Caption.ShouldBeEqualTo(c.Name));
      }

      [Observation]
      public void should_initialize_the_sub_presenter_with_the_presenter_as_parent()
      {
         _allSubPresenters.Each(x=> A.CallTo(() => x.InitializeWith(sut)).MustHaveHappened());
      }

      [Observation]
      public void should_add_the_sub_views_to_the_sub_presenter_collector()
      {
         _allSubPresenters.Select(x => x.BaseView).Each(v => _views.ShouldContain(v));
      }
   }

   public class When_editing_a_simulation_for_the_second_time : concern_for_SimulationCompoundConfigurationCollectorPresenter
   {
      private List<ISimulationCompoundConfigurationPresenter> _allPresentersFirstCall;

      protected override void Context()
      {
         base.Context();
         sut.EditSimulation(_simulation, CreationMode.New);

         //save into another variable so that we can ensure that the first presenter were released
         _allPresentersFirstCall = new List<ISimulationCompoundConfigurationPresenter>(_allSubPresenters);
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      [Observation]
      public void should_release_the_presenter_previously_created_to_avoid_memory_leaks()
      {
         A.CallTo(() => _configurableLayoutPresenter.RemoveViews()).MustHaveHappened();
         _allPresentersFirstCall.Each(p => A.CallTo(() => p.ReleaseFrom(_eventPublisher)).MustHaveHappened());
      }
   }

   public class When_saving_the_configuration_of_all_dynamically_assigned_presenters : concern_for_SimulationCompoundConfigurationCollectorPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      protected override void Because()
      {
         sut.SaveConfiguration();
      }

      [Observation]
      public void should_save_the_configuration_in_each_presenter()
      {
         _allSubPresenters.Each(p => A.CallTo(() => p.SaveConfiguration()).MustHaveHappened());
      }
   }
}