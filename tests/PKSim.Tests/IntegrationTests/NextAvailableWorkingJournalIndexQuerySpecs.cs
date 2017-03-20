using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Infrastructure.Journal.Commands;
using OSPSuite.Infrastructure.Journal.Queries;

namespace PKSim.IntegrationTests
{
   public class When_retrieving_a_new_unique_index : ContextForJournalDatabase<NextAvailableJournalPageIndexQuery>
   {
      protected override void Context()
      {
         var workingJournalItem = _journalPageFactory.Create();
         workingJournalItem.Title = "Test Title";
         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = workingJournalItem});

         workingJournalItem = _journalPageFactory.Create();
         workingJournalItem.Title = "Test Title";
         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = workingJournalItem});
      }

      [Observation]
      public void should_return_the_next_available_index()
      {
         _databaseMediator.ExecuteQuery(new NextAvailableJournalPageIndex()).ShouldBeEqualTo(3);
      }
   }
}