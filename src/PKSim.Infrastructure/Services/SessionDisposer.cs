using System;
using NHibernate;

namespace PKSim.Infrastructure.Services
{
   public interface ISessionDisposer : IDisposable
   {
      ISession Session { get; }
      bool Disposed { get; }
   }

   public class SessionDisposer : ISessionDisposer
   {
      public ISession Session { get; private set; }

      public bool Disposed
      {
         get { return Session == null || !Session.IsOpen; }
      }

      public SessionDisposer(ISession session)
      {
         Session = session;
      }

      public void Dispose()
      {
         Session.Dispose();
         Session = null;
      }
   }
}