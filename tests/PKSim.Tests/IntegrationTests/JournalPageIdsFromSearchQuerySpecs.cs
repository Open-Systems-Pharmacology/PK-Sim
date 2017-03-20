using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Journal;
using OSPSuite.Infrastructure.Journal.Commands;
using OSPSuite.Infrastructure.Journal.Queries;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_WorkingJournalItemIdsFromSearchQuery : ContextForJournalDatabase<JournalItemIdsFromSearchQuery>
   {
      protected JournalSearch _search;
      protected List<string> _ids;
      protected JournalPage _journalItem1;
      protected JournalPage _journalItem2;

      protected override void Context()
      {
         base.Context();
         _search = new JournalSearch();
         _journalItem1 = _journalPageFactory.Create();
         _journalItem1.Title = "Item1";
         _journalItem1.FullText = "A small house full of fun";

         _journalItem2 = _journalPageFactory.Create();
         _journalItem2.Title = "Item2";
         _journalItem2.FullText = "A big House empty";

         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = _journalItem1});
         _databaseMediator.ExecuteCommand(new CreateJournalPage {JournalPage = _journalItem2});
      }

      protected override void Because()
      {
         _ids = _databaseMediator.ExecuteQuery(new JournalPageIdsFromSearch {Search = _search}).ToList();
      }
   }

   public class When_retrieving_all_journal_ids_matching_an_AND_query : concern_for_WorkingJournalItemIdsFromSearchQuery
   {
      protected override void Context()
      {
         base.Context();
         _search.MatchAll = true;
         _search.Search = "A house full";
      }

      [Observation]
      public void should_return_the_expected_ids()
      {
         _ids.ShouldOnlyContain(_journalItem1.Id);
      }
   }

   public class When_retrieving_all_journal_ids_matching_a_query_case_sensitive_with_match : concern_for_WorkingJournalItemIdsFromSearchQuery
   {
      protected override void Context()
      {
         base.Context();
         _search.MatchAll = true;
         _search.Search = "House";
         _search.MatchCase = true;
      }

      [Observation]
      public void should_return_the_expected_ids()
      {
         _ids.ShouldOnlyContain(_journalItem2.Id);
      }
   }

   public class When_retrieving_all_journal_ids_matching_a_query_case_insensitive : concern_for_WorkingJournalItemIdsFromSearchQuery
   {
      protected override void Context()
      {
         base.Context();
         _search.MatchAll = true;
         _search.Search = "House";
         _search.MatchCase = false;
      }

      [Observation]
      public void should_return_the_expected_ids()
      {
         _ids.ShouldOnlyContain(_journalItem1.Id, _journalItem2.Id);
      }
   }

   public class When_retrieving_all_journal_ids_matching_an_OR_query : concern_for_WorkingJournalItemIdsFromSearchQuery
   {
      protected override void Context()
      {
         base.Context();
         _search.MatchAll = false;
         _search.Search = "house full";
      }

      [Observation]
      public void should_return_the_expected_ids()
      {
         _ids.ShouldOnlyContain(_journalItem1.Id, _journalItem2.Id);
      }
   }
}