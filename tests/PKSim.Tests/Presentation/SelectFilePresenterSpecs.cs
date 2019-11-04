using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;

using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_SelectFilePresenter : ContextSpecification<ISelectFilePresenter>
   {
      protected IDialogCreator _dialogCreator;
      protected ISelectFileView _view;

      protected override void Context()
      {
         _dialogCreator = A.Fake<IDialogCreator>();
         _view = A.Fake<ISelectFileView>();
         sut = new SelectFilePresenter(_view, _dialogCreator);
      }
   }

   public class When_selecting_a_file : concern_for_SelectFilePresenter
   {
      private FileSelection _fileSelection;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         _fileSelection = sut.SelectFile("Select file", "filter", "test", "A key");
      }

      [Observation]
      public void should_return_a_file_selection()
      {
         _fileSelection.ShouldNotBeNull();
      }

      [Observation]
      public void Should_set_file_path_caption_for_directory()
      {
         A.CallTo(() => _view.SetFileSelectionCaption(PKSimConstants.UI.FilePath)).MustHaveHappened();
      }
   }

   public class When_performing_the_actual_file_selection : concern_for_SelectFilePresenter
   {
      private FileSelection _fileSelection;
      private const string _path = "Path";
      private const string _caption = "Caption";
      private const string _filter = "filter";
      private const string _default = "toto";
      private const string _directoryKey = "A key";

      protected override void Context()
      {
         base.Context();
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(_path);
         _fileSelection = sut.SelectFile(_caption, _filter, _default, _directoryKey);
      }

      protected override void Because()
      {
         sut.PerformFileSelection();
      }

      [Observation]
      public void should_launch_a_dialog_selection_using_the_parameters_used_to_initialize_the_presenter()
      {
         A.CallTo(() => _dialogCreator.AskForFileToSave(_caption, _filter, _directoryKey, _default,null)).MustHaveHappened();
      }

      [Observation]
      public void should_have_updated_the_path_according_to_the_user_selection()
      {
         _fileSelection.FilePath.ShouldBeEqualTo(_path);
      }
   }

   public class When_selecting_a_directory : concern_for_SelectFilePresenter
   {
      private FileSelection _fileSelection;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         _fileSelection = sut.SelectDirectory("Select Directory",  Constants.DirectoryKey.PROJECT);
      }

      [Observation]
      public void should_return_a_file_selection()
      {
         _fileSelection.ShouldNotBeNull();
      }

      [Observation]
      public void Should_set_file_path_caption_for_directory()
      {
         A.CallTo(() => _view.SetFileSelectionCaption(PKSimConstants.UI.ExportDirectory)).MustHaveHappened();
      }
   }
}