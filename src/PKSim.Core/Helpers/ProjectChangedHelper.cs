using System;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Helpers
{
   public static class ProjectChangedHelper
   {
      public static IDisposable Scope(IProjectRetriever projectRetriever)
         => new ScopeGuard(projectRetriever?.CurrentProject ?? throw new ArgumentNullException(nameof(projectRetriever)));

      private sealed class ScopeGuard : IDisposable
      {
         private readonly IProject _project;
         private readonly bool _original;
         private bool _disposed;

         public ScopeGuard(IProject project)
         {
            _project = project ?? throw new ArgumentNullException(nameof(project));
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