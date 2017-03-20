using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Infrastructure.Journal.Commands;
using OSPSuite.Infrastructure.Journal.Queries;

namespace PKSim.IntegrationTests
{
   public class When_querying_all_available_tags_in_the_database : ContextForJournalDatabase<AllKnownTagsQuery>
   {
      [Observation]
      public void should_return_the_expected_tags()
      {
         var workingJournalItem = _journalPageFactory.Create();
         workingJournalItem.Title = "Test Title";
         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = workingJournalItem});

         workingJournalItem.AddTag("Tag1");
         workingJournalItem.AddTag("Tag2");
         _databaseMediator.ExecuteCommand(new UpdateJournalPageTags {JournalPage = workingJournalItem});

         var tags = _databaseMediator.ExecuteQuery(new AllKnownTags());
         tags.ShouldOnlyContain("Tag1", "Tag2");
      }
   }
}