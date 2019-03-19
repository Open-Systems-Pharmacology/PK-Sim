using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using FakeItEasy;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Services;
using OSPSuite.UI.Services;


namespace PKSim.UI.Tests
{
   public abstract class concern_for_JournalExportTask : ContextSpecification<JournalExportTask>
   {
      protected IProjectRetriever _projectRetriever;
      protected IDialogCreator _dialogCreator;
      protected IContentLoader _contentLoader;
      protected string _triedFileName;
      private IRichEditDocumentServerFactory _richEditDocumentServerFactory;
      protected IRichEditDocumentServer _documentServer;
      private Document _document;
      private int _sectionCount;

      protected override void Context()
      {
         _projectRetriever = A.Fake<IProjectRetriever>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _contentLoader = A.Fake<IContentLoader>();
         FileHelper.TryOpenFile = s => _triedFileName = s;
         _richEditDocumentServerFactory = A.Fake<IRichEditDocumentServerFactory>();
         _documentServer = A.Fake<IRichEditDocumentServer>();
         _document = A.Fake<Document>();
         A.CallTo(() => _document.AppendSection()).Invokes(() => _sectionCount++).ReturnsLazily<Section>(A.Fake<Section>);
         A.CallTo(() => _document.Sections.Count).ReturnsLazily(() => _sectionCount);
         A.CallTo(() => _richEditDocumentServerFactory.Create()).Returns(_documentServer);
         A.CallTo(() => _documentServer.Document).Returns(_document);

         // document starts with one section always
         _sectionCount = 1;
         sut = new JournalExportTask(_contentLoader, _dialogCreator, _projectRetriever, _richEditDocumentServerFactory);
      }

      protected static JournalPage CreatePage()
      {
         var journalPage = new JournalPage {Content = new Content {Data = new byte[] {}}};
         return journalPage;
      }
   }

   public class When_exporting_an_empty_journal_to_word : concern_for_JournalExportTask
   {
      private Journal _journal;

      protected override void Context()
      {
         base.Context();
         _journal = new Journal();
      }

      protected override void Because()
      {
         sut.ExportJournalToWordFile(_journal);
      }

      [Observation]
      public void a_call_to_show_error_dialog_should_have_happened()
      {
         A.CallTo(() => _dialogCreator.MessageBoxError(A<string>._)).MustHaveHappened();
      }
   }

   public class When_exporting_journal_parts_to_word : concern_for_JournalExportTask
   {
      private Journal _journal;

      protected override void Context()
      {
         base.Context();
         _journal = new Journal();
         _journal.AddJournalPages(new[] { CreatePage(), CreatePage() });
         A.CallTo( _dialogCreator).WithReturnType<string>().Returns("fileName");
         _triedFileName = string.Empty;
      }

      protected override void Because()
      {
         sut.ExportSelectedPagesToWordFile(_journal.JournalPages);
      }

      [Observation]
      public void document_should_contain_one_section_per_journal_page()
      {
         _documentServer.Document.Sections.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_try_to_open_the_file_after_saving()
      {
         _triedFileName.ShouldBeEqualTo("fileName");
      }
   }

   public class When_exporting_whole_journal_to_word : concern_for_JournalExportTask
   {
      private Journal _journal;

      protected override void Context()
      {
         base.Context();
         _journal = new Journal();
         _journal.AddJournalPage(CreatePage());
         _journal.AddJournalPage(CreatePage());
         _journal.AddJournalPage(CreatePage());
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns("fileName");
         _triedFileName = string.Empty;
      }

      protected override void Because()
      {
         sut.ExportJournalToWordFile(_journal);
      }

      [Observation]
      public void the_word_document_should_contain_the_correct_number_of_sections()
      {
         _documentServer.Document.Sections.Count.ShouldBeEqualTo(3);  
      }

      [Observation]
      public void should_try_to_open_the_file_after_saving()
      {
         _triedFileName.ShouldBeEqualTo("fileName");
      }
   }
}
