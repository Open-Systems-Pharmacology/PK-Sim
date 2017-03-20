using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Journal;
using OSPSuite.Infrastructure.Journal.Commands;

namespace PKSim.IntegrationTests
{
   public class When_updating_a_journal_page_entry : ContextForJournalDatabase<UpdateJournalItemCommand>
   {
      private JournalPage _journalPage;
      private JournalPage _parentJournalPage;

      protected override void Context()
      {
         base.Context();
         _journalPage = _journalPageFactory.Create();
         _journalPage.Title = "Test Title";

         _parentJournalPage = _journalPageFactory.Create();
         _parentJournalPage.Title = "ParentTitle";

         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = _journalPage});
         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = _parentJournalPage});

         _journalPage.Title = "New Title";
         _journalPage.FullText = "AAAA";
         _journalPage.Origin = Origins.R;
         _journalPage.AddTag("Tag1");
         _journalPage.AddTag("Tag2");
         _journalPage.ParentId = _parentJournalPage.Id;
      }

      protected override void Because()
      {
         _databaseMediator.ExecuteCommand(new UpdateJournalPage {JournalPage = _journalPage});
      }

      [Observation]
      public void should_be_able_to_retrieve_the_updated_content()
      {
         var workingJournalItemFromDb = JournalPageById(_journalPage.Id);
         workingJournalItemFromDb.Title.ShouldBeEqualTo("New Title");
         workingJournalItemFromDb.OriginId.ShouldBeEqualTo(OriginId.R);
         workingJournalItemFromDb.Tags.ShouldContain("Tag1", "Tag2");
         workingJournalItemFromDb.ParentId.ShouldBeEqualTo(_parentJournalPage.Id);
         workingJournalItemFromDb.FullText.ShouldBeEqualTo("AAAA");
      }
   }
}