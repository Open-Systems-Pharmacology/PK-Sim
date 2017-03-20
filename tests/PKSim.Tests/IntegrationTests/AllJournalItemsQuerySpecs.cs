using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Journal.Commands;
using OSPSuite.Infrastructure.Journal.Queries;

namespace PKSim.IntegrationTests
{
   public class When_retrieving_all_journal_pages_defined_in_the_database : ContextForJournalDatabase<AllJournalPagesQuery>
   {
      [Observation]
      public void should_return_all_available_journal_pages_with_tags_and_related_items()
      {
         var workingJournalItem = _journalPageFactory.Create();
         workingJournalItem.Title = "Test Title";
         _databaseMediator.ExecuteCommand(new CreateJournalPage { JournalPage = workingJournalItem });

         workingJournalItem.AddTag("Tag1");
         workingJournalItem.AddTag("Tag2");
         _databaseMediator.ExecuteCommand(new UpdateJournalPage { JournalPage = workingJournalItem });

         var container = new Container { Name = "toto" };
         var relatedItem = _relatedItemFactory.Create(container);
         relatedItem.Description = "ABC";
         relatedItem.Origin = Origins.PKSim;
         relatedItem.Version = "123";
         _databaseMediator.ExecuteCommand(new AddRelatedPageToJournalPage { JournalPage = workingJournalItem, RelatedItem = relatedItem });

         var allWorkingJournalItems = _databaseMediator.ExecuteQuery(new AllJournalPages());
         var workingJournalItemFromDb = allWorkingJournalItems.ElementAt(0);
         workingJournalItemFromDb.Tags.ShouldContain("Tag1", "Tag2");
         workingJournalItemFromDb.RelatedItems.Count.ShouldBeEqualTo(1);
         workingJournalItemFromDb.RelatedItems[0].Origin.ShouldBeEqualTo(relatedItem.Origin);
      }
   }
}