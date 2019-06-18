using System;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using PKSim.Assets;
using PKSim.Infrastructure.ORM.DAS;

namespace PKSim.Infrastructure.ORM.Core
{
   public interface IDatabase : IDisposable
   {
      void Connect(string databasePath);
      DAS.DAS DatabaseObject { get; }
      void Disconnect();
      bool IsConnected { get; }
   }

   public abstract class Database : IDatabase
   {
      public DAS.DAS DatabaseObject { get; private set; }
      private readonly string _userName;
      private readonly string _password;

      protected Database(string password, string username)
      {
         _userName = username;
         _password = password;
         //create a default DAS in case the Connect was not called...
         DatabaseObject = new DAS.DAS();
      }

      public void Connect(string databasePath)
      {
         //clear any open connection if already open
         Cleanup();

         if (DatabaseObject == null)
            DatabaseObject = new DAS.DAS();

         if (!FileHelper.FileExists(databasePath))
            throw new OSPSuiteException(PKSimConstants.Error.FileDoesNotExist(databasePath));

         DatabaseObject.Connect(databasePath.ToUNCPath(), _userName, _password, GetProvider());
      }

      protected abstract DataProviders GetProvider();

      public void Disconnect()
      {
         if (!IsConnected) return;

         if (DatabaseObject.IsTransactionOpen)
            DatabaseObject.Rollback();

         try
         {
            DatabaseObject.DisConnect();
         }
         catch (Exception)
         {
            //Do nothing;
         }
      }

      public bool IsConnected => DatabaseObject != null && DatabaseObject.IsConnected;

      protected virtual void Cleanup()
      {
         Disconnect();
      }

      #region Disposable properties

      private bool _disposed;

      public void Dispose()
      {
         if (_disposed) return;

         Cleanup();
         GC.SuppressFinalize(this);
         _disposed = true;
      }

      ~Database()
      {
         Cleanup();
      }

      #endregion
   }
}