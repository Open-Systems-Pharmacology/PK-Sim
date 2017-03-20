using System.Text;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using OSPSuite.Core.Journal;
using OSPSuite.Infrastructure.Journal;
using OSPSuite.Infrastructure.Journal.Queries;

namespace PKSim.IntegrationTests
{
   public abstract class ContextForJournalDatabase<T> : ContextForIntegration<T>
   {
      private string _databaseFile;
      protected IJournalPageFactory _journalPageFactory;
      protected IContentLoader _contendLoader;
      protected IDatabaseMediator _databaseMediator;
      protected IJournalSession _journalSession;
      protected IRelatedItemFactory _relatedItemFactory;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _databaseFile = FileHelper.GenerateTemporaryFileName();
         _journalSession = IoC.Resolve<IJournalSession>();
         _journalPageFactory = IoC.Resolve<IJournalPageFactory>();
         _contendLoader = IoC.Resolve<IContentLoader>();
         _databaseMediator = IoC.Resolve<IDatabaseMediator>();
         _relatedItemFactory = IoC.Resolve<IRelatedItemFactory>();
         _journalSession.Create(_databaseFile);
      }

      protected override void Context()
      {
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         _journalSession.Close();
         FileHelper.DeleteFile(_databaseFile);
      }

      protected byte[] StringToBytes(string stringToConvert)
      {
         return Encoding.UTF8.GetBytes(stringToConvert);
      }

      protected string BytesToString(byte[] bytes)
      {
         return Encoding.UTF8.GetString(bytes);
      }

      protected JournalPage JournalPageById(string id)
      {
         return _databaseMediator.ExecuteQuery(new JournalPageById {Id = id});
      }
   }
}