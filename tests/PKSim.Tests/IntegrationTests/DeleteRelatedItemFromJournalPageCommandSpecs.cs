using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Journal.Commands;

namespace PKSim.IntegrationTests
{
   public class When_deleting_an_existing_related_item_from_a_journal_page_entry : ContextForJournalDatabase<DeleteRelatedItemFromJournalPage>
   {
      [Observation]
      public void should_remove_the_content_of_the_journal_and_the_content_of_all_related_items_as_well_as_the_item_itself()
      {
         var workingJournalItem = _journalPageFactory.Create();
         workingJournalItem.Title = "Test Title";
         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = workingJournalItem});

         var container = new Container {Name = "toto"};
         var relatedItem = _relatedItemFactory.Create(container);
         relatedItem.Description = "ABC";
         relatedItem.Origin = Origins.PKSim;
         relatedItem.Version = "123";
         _databaseMediator.ExecuteCommand(new AddRelatedPageToJournalPage {JournalPage = workingJournalItem, RelatedItem = relatedItem});
         workingJournalItem.AddRelatedItem(relatedItem);

         _databaseMediator.ExecuteCommand(new DeleteRelatedItemFromJournalPage {RelatedItem = relatedItem});
         _journalSession.Current.Contents.All().Count().ShouldBeEqualTo(1);
         _journalSession.Current.RelatedItems.All().Count().ShouldBeEqualTo(0);
         _journalSession.Current.JournalPages.All().Count().ShouldBeEqualTo(1);
      }
   }
}