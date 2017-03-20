using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Exceptions;
using PKSim.Core;
using PKSim.Presentation.Core;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Matlab
{
   internal class MatlabRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.Register<ICoreUserSettings, MatlabUserSettings>(LifeStyle.Singleton);
         container.Register<IExceptionManager, MatlabExceptionManager>(LifeStyle.Singleton);
         container.Register<IOntogenyFactorsRetriever, OntogenyFactorsRetriever>(LifeStyle.Singleton);
         container.Register<IWorkspace, MatlabWorkspace>(LifeStyle.Singleton);
         container.Register<IProgressUpdater, MatlabProgressUpdater>();
         container.Register<IDisplayUnitRetriever, MatlabDisplayUnitRetriever>();
         container.Register<IFullPathDisplayResolver, MatlabFullPathDisplayResolver>();
         container.Register<ILazyLoadTask, MatlabLazyLoadTask>(LifeStyle.Singleton);
      }
   }
}