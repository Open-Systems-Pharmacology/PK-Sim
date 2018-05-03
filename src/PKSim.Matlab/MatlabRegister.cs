using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Exceptions;
using PKSim.CLI.Core.MinimalImplementations;
using PKSim.Core;
using PKSim.Matlab.Mappers;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Matlab
{
   internal class MatlabRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.Register<IOntogenyFactorsRetriever, OntogenyFactorsRetriever>(LifeStyle.Singleton);
         container.Register<IJournalDiagramManagerFactory, CLIJournalDiagramManagerFactory>();
         container.Register<IDialogCreator, CLIDialogCreator>();
         container.Register<ICoreUserSettings, CLIUserSettings>();
         container.Register<IExceptionManager, CLIExceptionManager>();
         container.Register<IProgressUpdater, CLIProgressUpdater>();
         container.Register<IDisplayUnitRetriever, CLIDisplayUnitRetriever>();
         container.Register<IFullPathDisplayResolver, MatlabFullPathDisplayResolver>();
         container.Register<IMatlabPopulationSettingsToPopulationSettingsMapper, MatlabPopulationSettingsToPopulationSettingsMapper>();
         container.Register<ILazyLoadTask, MatlabLazyLoadTask>(LifeStyle.Singleton);
      }
   }
}