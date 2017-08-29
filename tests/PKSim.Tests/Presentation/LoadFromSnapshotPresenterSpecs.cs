using System;
using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.DTO.Snapshots;
using PKSim.Presentation.Presenters.Snapshots;
using PKSim.Presentation.Views.Snapshots;

namespace PKSim.Presentation
{
   public abstract class concern_for_LoadFromSnapshotPresenter : ContextSpecification<ILoadFromSnapshotPresenter<Individual>>
   {
      protected ILoadFromSnapshotView _view;
      protected ISnapshotTask _snapshotTask;
      protected IDialogCreator _dialogCreator;
      protected IObjectTypeResolver _objectTypeResolver;
      protected ILogger _logger;
      protected IEventPublisher _eventPublisher;
      protected ILogPresenter _logPresenter;
      protected LoadFromSnapshotDTO _loadFromSnapshotDTO;
      protected string _objectType = "Ind";

      protected override void Context()
      {
         _view = A.Fake<ILoadFromSnapshotView>();
         _snapshotTask = A.Fake<ISnapshotTask>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         _logger = A.Fake<ILogger>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _logPresenter = A.Fake<ILogPresenter>();

         A.CallTo(() => _view.BindTo(A<LoadFromSnapshotDTO>._))
            .Invokes(x => _loadFromSnapshotDTO = x.GetArgument<LoadFromSnapshotDTO>(0));

         A.CallTo(() => _logPresenter.CanClose).Returns(true);

         A.CallTo(() => _objectTypeResolver.TypeFor<Individual>()).Returns(_objectType);
         sut = new LoadFromSnapshotPresenter<Individual>(_view, _logPresenter, _snapshotTask, _dialogCreator, _objectTypeResolver, _logger, _eventPublisher);
      }
   }

   public class When_initializing_the_load_from_snapshot_presenter : concern_for_LoadFromSnapshotPresenter
   {
      [Observation]
      public void should_bind_to_the_view()
      {
         _loadFromSnapshotDTO.ShouldNotBeNull();
      }

      [Observation]
      public void should_update_the_caption_based_on_the_snapshot_type()
      {
         _view.Caption.ShouldBeEqualTo(PKSimConstants.UI.LoadObjectFromSnapshot(_objectType));
      }
   }

   public class When_loading_a_models_from_snapshot_and_the_user_cancels_the_action : concern_for_LoadFromSnapshotPresenter
   {
      private IEnumerable<Individual> _individuals;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override void Because()
      {
         _individuals = sut.LoadModelFromSnapshot();
      }

      [Observation]
      public void should_return_null()
      {
         _individuals.ShouldBeNull();
      }
   }

   public class When_the_user_selects_a_snapshot_file_in_load_from_snapshot_presenter : concern_for_LoadFromSnapshotPresenter
   {
      private readonly string _snapshotFile = "SnaphsotFileName";

      protected override void Context()
      {
         base.Context();
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(_snapshotFile);
      }

      protected override void Because()
      {
         sut.SelectFile();
      }

      [Observation]
      public void should_display_the_selected_file_into_the_view()
      {
         _loadFromSnapshotDTO.SnapshotFile.ShouldBeEqualTo(_snapshotFile);
      }
   }

   public class When_loading_models_from_snapshot_and_the_load_action_was_succesful : concern_for_LoadFromSnapshotPresenter
   {
      private IEnumerable<Individual> _individuals;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _individual = new Individual();
         A.CallTo(() => _view.Canceled).Returns(false);
         var snapshotFile = "SnapshotFile";
         _loadFromSnapshotDTO.SnapshotFile = snapshotFile;
         A.CallTo(() => _snapshotTask.LoadModelFromSnapshot<Individual>(snapshotFile)).Returns(new[] {_individual});
      }

      protected override void Because()
      {
         sut.Start().Wait();
         _individuals = sut.LoadModelFromSnapshot();
      }

      [Observation]
      public void should_have_cleared_the_log()
      {
         A.CallTo(() => _logPresenter.ClearLog()).MustHaveHappened();
      }

      [Observation]
      public void should_return_the_loaded_model_from_snapshot()
      {
         _individuals.ShouldContain(_individual);
      }

      [Observation]
      public void should_returns_that_a_model_is_defined()
      {
         sut.ModelIsDefined.ShouldBeTrue();  
      }

      [Observation]
      public void should_have_disable_all_ui_buttons_and_enabled_them_once_the_run_was_finished()
      {
         A.CallTo(() => _view.EnableButtons(false, false, false)).MustHaveHappened();
         A.CallTo(() => _view.EnableButtons(true, true, true)).MustHaveHappened();
      }
   }

   public class When_loading_models_from_snapshot_throws_an_exception : concern_for_LoadFromSnapshotPresenter
   {
      private Exception _exception;

      protected override void Context()
      {
         base.Context();
         _exception = new Exception();
         A.CallTo(() => _snapshotTask.LoadModelFromSnapshot<Individual>(A<string>._)).Throws(_exception);
      }

      protected override void Because()
      {
         sut.Start().Wait();
      }

      [Observation]
      public void should_returns_that_a_model_is_not_defined()
      {
         sut.ModelIsDefined.ShouldBeFalse();
      }

      [Observation]
      public void should_log_the_exception_into_the_logger()
      {
         A.CallTo(() => _logger.AddToLog(A<string>._, NotificationType.Error)).MustHaveHappened();
      }

      [Observation]
      public void should_not_enable_the_ok_button()
      {
         A.CallTo(() => _view.EnableButtons(true, false, true)).MustHaveHappened();
      }
   }
}