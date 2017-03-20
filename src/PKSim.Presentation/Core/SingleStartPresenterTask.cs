using System;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.Core
{
   public class SingleStartPresenterTask : ISingleStartPresenterTask
   {
      private readonly IOpenSingleStartPresenterInvoker _openSingleStartPresenterInvoker;
      private readonly IApplicationController _applicationController;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IHeavyWorkManager _heavyWorkManager;

      public SingleStartPresenterTask(IOpenSingleStartPresenterInvoker openSingleStartPresenterInvoker, IApplicationController applicationController,
         ILazyLoadTask lazyLoadTask, IHeavyWorkManager heavyWorkManager)
      {
         _openSingleStartPresenterInvoker = openSingleStartPresenterInvoker;
         _applicationController = applicationController;
         _lazyLoadTask = lazyLoadTask;
         _heavyWorkManager = heavyWorkManager;
      }

      public void StartForSubject<T>(T subject)
      {
         if (subject == null) return;
         var lazyLoadable = subject as ILazyLoadable;
         if (lazyLoadable != null && !lazyLoadable.IsLoaded)
         {
            _heavyWorkManager.Start(() => _lazyLoadTask.Load(lazyLoadable));
         }

         var presenter = _openSingleStartPresenterInvoker.OpenPresenterFor(subject);
         try
         {
            presenter.Edit(subject);
         }
         catch (Exception)
         {
            //exception while loading the subject. We need to close the presenter to avoid memory leaks
            _applicationController.Close(subject);
            throw;
         }
      }
   }
}