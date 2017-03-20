using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Journal.Commands;

namespace PKSim.IntegrationTests
{
   public class When_adding_a_related_item_to_an_existing_journal_entry : ContextForJournalDatabase<AddRelatedItemToJournalItemCommand>
   {
      [Observation]
      public void should_be_able_to_retrieve_the_related_item()
      {
         var workingJournalItem = _journalPageFactory.Create();
         workingJournalItem.Title = "Test Title";
         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = workingJournalItem});

         var container = new Container {Name = "toto"};
         var relatedItem = _relatedItemFactory.Create(container);
         relatedItem.Description = "ABC";
         relatedItem.Origin = Origins.PKSim;
         relatedItem.Version = "123";
         relatedItem.Discriminator = "SIM";
         relatedItem.FullPath = "FullPath";
         _databaseMediator.ExecuteCommand(new AddRelatedPageToJournalPage {JournalPage = workingJournalItem, RelatedItem = relatedItem});

         var workingJournalItemFromDb = JournalPageById(workingJournalItem.Id);
         workingJournalItemFromDb.RelatedItems.Count.ShouldBeEqualTo(1);
         var relatedItemFromDb = workingJournalItemFromDb.RelatedItems[0];
         relatedItemFromDb.IsLoaded.ShouldBeFalse();
         relatedItemFromDb.Name.ShouldBeEqualTo(relatedItem.Name);
         relatedItemFromDb.Origin.ShouldBeEqualTo(relatedItem.Origin);
         relatedItemFromDb.Description.ShouldBeEqualTo(relatedItem.Description);
         relatedItemFromDb.Version.ShouldBeEqualTo(relatedItem.Version);
         relatedItemFromDb.Discriminator.ShouldBeEqualTo(relatedItem.Discriminator);
         relatedItemFromDb.FullPath.ShouldBeEqualTo(relatedItem.FullPath);
      }
   }
}