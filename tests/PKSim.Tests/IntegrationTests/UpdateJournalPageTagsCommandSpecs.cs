using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Infrastructure.Journal.Commands;

namespace PKSim.IntegrationTests
{
   public class When_updating_the_tags_of_a_journal_page_entry : ContextForJournalDatabase<UpdateJournalItemTagsCommand>
   {
      [Observation]
      public void should_be_able_to_retrieve_the_updated_content()
      {
         var journalPage = _journalPageFactory.Create();
         journalPage.Title = "Test Title";
         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = journalPage});

         journalPage.AddTag("Tag1");
         journalPage.AddTag("Tag2");
         _databaseMediator.ExecuteCommand(new UpdateJournalPageTags {JournalPage = journalPage});

         var journalPageFromDb = JournalPageById(journalPage.Id);
         journalPageFromDb.Tags.ShouldOnlyContain("Tag1", "Tag2");
      }
   }
}