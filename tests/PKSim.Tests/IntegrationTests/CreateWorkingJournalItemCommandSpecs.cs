using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Journal;
using OSPSuite.Infrastructure.Journal.Commands;

namespace PKSim.IntegrationTests
{
   public class When_creating_a_journal_database_and_saving_some_journal_page_with_content : ContextForJournalDatabase<CreateJournalItemCommand>
   {
      private readonly string _data = "My data converted in bytes";
      private JournalPage _journalPage;

      protected override void Context()
      {
         _journalPage = _journalPageFactory.Create();
         _journalPage.Title = "Test Title";
         _journalPage.Origin = Origins.Matlab;
         _journalPage.Content.Data = StringToBytes(_data);
      }

      protected override void Because()
      {
         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = _journalPage});
      }

      [Observation]
      public void should_be_able_to_retrieve_the_content()
      {
         var workingJournalItemFromDb = JournalPageById(_journalPage.Id);

         workingJournalItemFromDb.CreatedBy.ShouldBeEqualTo(_journalPage.CreatedBy);
         _journalPage.UniqueIndex.ShouldBeEqualTo(1);
         _journalPage.Origin.ShouldBeEqualTo(Origins.Matlab);
         workingJournalItemFromDb.UniqueIndex.ShouldBeEqualTo(_journalPage.UniqueIndex);

         workingJournalItemFromDb.IsLoaded.ShouldBeFalse();

         _contendLoader.Load(workingJournalItemFromDb);
         workingJournalItemFromDb.IsLoaded.ShouldBeTrue();
         var stringContent = BytesToString(workingJournalItemFromDb.Content.Data);

         stringContent.ShouldBeEqualTo(_data);
      }
   }
}