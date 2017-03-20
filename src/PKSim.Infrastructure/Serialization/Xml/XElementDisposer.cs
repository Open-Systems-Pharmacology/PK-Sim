using System;
using System.Xml.Linq;

namespace PKSim.Infrastructure.Serialization.Xml
{
   public class XElementDisposer : IDisposable
   {
      public XElement Element { get; private set; }

      public XElementDisposer(XElement element)
      {
         Element = element;
      }

      protected virtual void Cleanup()
      {
         if(Element==null) return;
         Element.RemoveAll();
         Element = null;
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

      ~XElementDisposer()
      {
         Cleanup();
      }

     

      #endregion
   }
}