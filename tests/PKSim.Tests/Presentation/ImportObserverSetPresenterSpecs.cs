using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Observers;
using PKSim.Presentation.Presenters.Observers;
using PKSim.Presentation.Views.Observers;

namespace PKSim.Presentation
{
   public abstract class concern_for_ImportObserverSetPresenter : ContextSpecification<IImportObserverSetPresenter>
   {
      protected IImportObserverSetView _view;
      protected IObserverSetTask _observerSetTask;
      protected IObserverInfoPresenter _observerInfoPresenter;
      protected IDialogCreator _dialogCreator;
      protected IObserverTask _observerTask;
      protected ObserverSet _observerSet;
      protected IObserverBuilder _observer1;
      protected IObserverBuilder _observer2;
      protected IReadOnlyList<ImportObserverDTO> _allImportObserverDTO;
      protected ICommandCollector _commandCollector;
      protected IEntityTask _entityTask;

      protected override void Context()
      {
         _view = A.Fake<IImportObserverSetView>();
         _observerInfoPresenter = A.Fake<IObserverInfoPresenter>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _observerTask = A.Fake<IObserverTask>();
         _commandCollector = A.Fake<ICommandCollector>();
         _observer1 = new ContainerObserverBuilder().WithName("OBS1");
         _observer2 = new ContainerObserverBuilder().WithName("OBS2");
         _observerSet = new ObserverSet {_observer1, _observer2};
         _entityTask = A.Fake<IEntityTask>();
         A.CallTo(() => _view.BindTo(A<IReadOnlyList<ImportObserverDTO>>._))
            .Invokes(x => _allImportObserverDTO = x.GetArgument<IReadOnlyList<ImportObserverDTO>>(0));

         sut = new ImportObserverSetPresenter(_view, _observerInfoPresenter, _dialogCreator, _observerTask, _entityTask);
         sut.InitializeWith(_commandCollector);
      }
   }

   public class When_the_import_observers_presenter_is_editing_an_observed_data_building_block : concern_for_ImportObserverSetPresenter
   {
      protected override void Because()
      {
         sut.Edit(_observerSet);
      }

      [Observation]
      public void should_update_the_view_with_the_observers_defined_in_the_building_block()
      {
         _allImportObserverDTO.Count.ShouldBeEqualTo(2);
         _allImportObserverDTO[0].Observer.ShouldBeEqualTo(_observer1);
         _allImportObserverDTO[1].Observer.ShouldBeEqualTo(_observer2);
      }
   }

   public class When_the_import_observers_presenter_is_removing_an_observer : concern_for_ImportObserverSetPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Edit(_observerSet);
      }

      protected override void Because()
      {
         sut.RemoveObserver(_allImportObserverDTO[0]);
      }

      [Observation]
      public void should_remove_the_observer_from_the_underlying_collection_of_observer()
      {
         _allImportObserverDTO.Count.ShouldBeEqualTo(1);
         _allImportObserverDTO[0].Observer.ShouldBeEqualTo(_observer2);
      }

      [Observation]
      public void should_remove_the_observer_from_the_building_block_using_a_command()
      {
         A.CallTo(() => _observerTask.RemoveObserver(_observer1, _observerSet)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_command_to_the_command_collector()
      {
         A.CallTo(() => _commandCollector.AddCommand(A<ICommand>._)).MustHaveHappened();
      }

      [Observation]
      public void should_refresh_the_view()
      {
         A.CallTo(() => _view.Rebind()).MustHaveHappened(2, Times.Exactly);
      }
   }

   public class When_the_import_observers_presenter_is_adding_an_observer_that_does_not_exist_by_name_already : concern_for_ImportObserverSetPresenter
   {
      private readonly string _fileFullPath = "OBS_FILE";
      private readonly IObserverBuilder _newObserver = new ContainerObserverBuilder().WithName("OBS3");

      protected override void Context()
      {
         base.Context();
         sut.Edit(_observerSet);
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(_fileFullPath);
         A.CallTo(() => _observerTask.LoadObserverFrom(_fileFullPath)).Returns(_newObserver);
      }

      protected override void Because()
      {
         sut.AddObserver();
      }

      [Observation]
      public void should_add_the_observer_to_the_underlying_collection_of_observer()
      {
         _allImportObserverDTO.Count.ShouldBeEqualTo(3);
         _allImportObserverDTO[2].Observer.ShouldBeEqualTo(_newObserver);
      }

      [Observation]
      public void should_add_the_observer_from_the_building_block_using_a_command()
      {
         A.CallTo(() => _observerTask.AddObserver(_newObserver, _observerSet)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_command_to_the_command_collector()
      {
         A.CallTo(() => _commandCollector.AddCommand(A<ICommand>._)).MustHaveHappened();
      }

      [Observation]
      public void should_refresh_the_view()
      {
         A.CallTo(() => _view.Rebind()).MustHaveHappened(2, Times.Exactly);
      }
   }

   public class When_the_import_observers_presenter_is_adding_an_observer_that_does_exist_by_name_already_and_the_user_does_rename_the_observer : concern_for_ImportObserverSetPresenter
   {
      private readonly string _fileFullPath = "OBS_FILE";
      private readonly IObserverBuilder _newObserver = new ContainerObserverBuilder().WithName("OBS1");

      protected override void Context()
      {
         base.Context();
         sut.Edit(_observerSet);
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(_fileFullPath);
         A.CallTo(() => _observerTask.LoadObserverFrom(_fileFullPath)).Returns(_newObserver);

         A.CallTo(() => _entityTask.NewNameFor(_newObserver, A<IEnumerable<string>>._, null)).Returns("OBS2");
      }

      protected override void Because()
      {
         sut.AddObserver();
      }

      [Observation]
      public void should_have_renamed_the_observer()
      {
         _newObserver.Name.ShouldBeEqualTo("OBS2");
      }

      [Observation]
      public void should_add_the_observer_to_the_list()
      {
         A.CallTo(() => _observerTask.AddObserver(_newObserver, _observerSet)).MustHaveHappened();
      }
   }

   public class When_the_import_observers_presenter_is_adding_an_observer_that_does_exist_by_name_already_and_the_user_cancels_the_rename : concern_for_ImportObserverSetPresenter
   {
      private readonly string _fileFullPath = "OBS_FILE";
      private readonly IObserverBuilder _newObserver = new ContainerObserverBuilder().WithName("OBS1");

      protected override void Context()
      {
         base.Context();
         sut.Edit(_observerSet);
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(_fileFullPath);
         A.CallTo(() => _observerTask.LoadObserverFrom(_fileFullPath)).Returns(_newObserver);

         A.CallTo(() => _entityTask.NewNameFor(_newObserver, A<IEnumerable<string>>._, null)).Returns(string.Empty);
      }

      protected override void Because()
      {
         sut.AddObserver();
      }

      [Observation]
      public void should_add_the_observer_to_the_list()
      {
         A.CallTo(() => _observerTask.AddObserver(_newObserver, _observerSet)).MustNotHaveHappened();
      }
   }
}