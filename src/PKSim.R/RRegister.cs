using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization.Diagram;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Utility.Container;
using PKSim.CLI.Core;
using PKSim.CLI.Core.MinimalImplementations;
using PKSim.Core;
using PKSim.R.Services;

namespace PKSim.R
{
   internal class RRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddRegister(x => x.FromType<CLIRegister>());

         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<RRegister>();

            //Register Services
            scan.IncludeNamespaceContainingType<IOntogenyFactorsRetriever>();

            scan.WithConvention<PKSimRegistrationConvention>();
         });
         registerCLITypes(container);
         
      }

      private static void registerCLITypes(IContainer container)
      {
         // container.Register<IProgressUpdater, NoneProgressUpdater>();
         // container.Register<IDialogCreator, CLIDialogCreator>();
         // container.Register<IDisplayUnitRetriever, CLIDisplayUnitRetriever>();
         container.Register<IJournalDiagramManagerFactory, CLIJournalDiagramManagerFactory>();
         container.Register<IDiagramModel, CLIDiagramModel>();
         container.Register<IDataImporter, CLIDataImporter>();
         container.Register<IEntityValidationTask, CLIEntityValidationTask>();
         // container.Register<IOntogenyTask, CLIIndividualOntogenyTask>();
         container.Register<IDiagramModelToXmlMapper, CLIDiagramModelToXmlMapper>(LifeStyle.Singleton);
         // container.Register<IHistoryManager, HistoryManager<IExecutionContext>>();
         // container.Register<ICoreUserSettings, OSPSuite.Core.ICoreUserSettings, CLIUserSettings>(LifeStyle.Singleton);
         // container.Register<ICoreWorkspace, IWorkspace, CLIWorkspace>(LifeStyle.Singleton);
      }
   }
}