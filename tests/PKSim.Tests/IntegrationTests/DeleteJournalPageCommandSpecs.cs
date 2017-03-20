using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Journal;
using OSPSuite.Infrastructure.Journal.Commands;

namespace PKSim.IntegrationTests
{
   public class When_deleting_an_existing_journal_page_entry : ContextForJournalDatabase<UpdateJournalPage>
   {
      [Observation]
      public void should_remove_the_content_of_the_journal_and_the_content_of_all_related_items_as_well_as_the_item_itself()
      {
         var journalPage = _journalPageFactory.Create();
         journalPage.Title = "Test Title";
         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = journalPage});

         var relatedItem = _relatedItemFactory.Create(new Container {Name = "toto"});
         relatedItem.Description = "ABC";
         relatedItem.Origin = Origins.PKSim;
         relatedItem.Version = "123";
         _databaseMediator.ExecuteCommand(new AddRelatedPageToJournalPage {JournalPage = journalPage, RelatedItem = relatedItem});
         journalPage.AddRelatedItem(relatedItem);

         //One content for working journal item, one for related item
         _journalSession.Current.Contents.All().Count().ShouldBeEqualTo(2);

         _databaseMediator.ExecuteCommand(new DeleteJournalPage {JournalPage = journalPage});
         _journalSession.Current.Contents.All().Count().ShouldBeEqualTo(0);
         _journalSession.Current.RelatedItems.All().Count().ShouldBeEqualTo(0);
         _journalSession.Current.JournalPages.All().Count().ShouldBeEqualTo(0);
      }
   }

   public class When_deleting_a_journal_page_that_is_the_parent_of_another_journal_page : ContextForJournalDatabase<UpdateJournalPage>
   {
      private JournalPage _parentPage;
      private JournalPage _childPage;

      protected override void Context()
      {
         base.Context();
         _childPage = _journalPageFactory.Create();
         _childPage.Title = "Child";
         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = _childPage});

         _parentPage = _journalPageFactory.Create();
         _parentPage.Title = "Parent";
         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = _parentPage});

         _childPage.ParentId = _parentPage.Id;
         _databaseMediator.ExecuteCommand(new UpdateJournalPage() {JournalPage = _childPage});
      }

      protected override void Because()
      {
         _databaseMediator.ExecuteCommand(new DeleteJournalPage() {JournalPage = _parentPage});
      }

      [Observation]
      public void should_have_remove_the_parent_relation_ship_from_the_child_page()
      {
         var childPageFromDb = JournalPageById(_childPage.Id);
         childPageFromDb.ParentId.ShouldBeNull();
      }
   }
}