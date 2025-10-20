using System;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Helpers
{
   public static class ProjectChangedHelper
   {
      public static IDisposable Scope(IProject project)
         => new ScopeGuard(project);

      private sealed class ScopeGuard : IDisposable
      {
         private readonly IProject _project;
         private readonly bool _original;
         private bool _disposed;

         public ScopeGuard(IProject project)
         {
            _project = project;
            _original = _project.HasChanged;
         }

         public void Dispose()
         {
            if (_disposed) return;
            _project.HasChanged = _original;
            _disposed = true;
         }
      }
   }
}