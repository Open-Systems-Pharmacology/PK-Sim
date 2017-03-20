using System;
using System.IO;
using System.Text;
using LumenWorks.Framework.IO.Csv;

namespace PKSim.Infrastructure.Services
{
   public class CsvReaderDisposer : IDisposable
   {
      private readonly FileStream _fsReader;
      private readonly CsvReader _csv;

      public CsvReader Csv
      {
         get { return _csv; }
      }

      public CsvReaderDisposer(string fileFullPath, char delimiter = ';')
      {
         _fsReader = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
         _csv = new CsvReader(new StreamReader(_fsReader, Encoding.Default), hasHeaders: true, delimiter: delimiter);
      }

      protected virtual void Cleanup()
      {
         if (_csv != null)
            _csv.Dispose();

         if (_fsReader != null)
            _fsReader.Dispose();
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

      ~CsvReaderDisposer()
      {
         Cleanup();
      }

      #endregion
   }
}