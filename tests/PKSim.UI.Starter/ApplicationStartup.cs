using DevExpress.LookAndFeel;
using Microsoft.Extensions.Logging;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization.Diagram;
using OSPSuite.Core.Services;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Exceptions;
using PKSim.BatchTool;
using PKSim.CLI.Core.MinimalImplementations;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Presentation;
using PKSim.Presentation.Views.Individuals;
using PKSim.Presentation.Views.Parameters;
using PKSim.UI.Views.Individuals;
using PKSim.UI.Views.Parameters;
using CoreRegister = PKSim.Core.CoreRegister;
using IWorkspace = OSPSuite.Core.IWorkspace;
using PresenterRegister = PKSim.Presentation.PresenterRegister;

namespace PKSim.UI.Starter
{
   public static class ApplicationStartup
   {
      public static void Start()
      {
         BootStrapping.ApplicationStartup.Initialize(LogLevel.Debug);

         var container = IoC.Container;
         using (container.OptimizeDependencyResolution())
         {
            container.AddRegister(x => x.FromType<CoreRegister>());
            container.AddRegister(x => x.FromType<PresenterRegister>());
            container.AddRegister(x => x.FromType<InfrastructureRegister>());
            container.AddRegister(x => x.FromType<StarterRegister>());

            registerMinimalImplementations(container);
            registerUIImplementation(container);

            InfrastructureRegister.LoadSerializers(container);
            InfrastructureRegister.RegisterWorkspace(container);
            BootStrapping.ApplicationStartup.RegisterCommands(container);
            container.RegisterImplementationOf(new DefaultLookAndFeel().LookAndFeel);
         }

         container.Register<IToolTipCreator, OSPSuite.UI.Services.IToolTipCreator, ToolTipCreator>(LifeStyle.Transient);
         var skinManager = container.Resolve<ISkinManager>();
         var userSettings = container.Resolve<IPresentationUserSettings>();
         skinManager.ActivateSkin(userSettings, Constants.DEFAULT_SKIN);
      }

      private static void registerUIImplementation(IContainer container)
      {
         container.Register<IIndividualProteinExpressionsView, IndividualProteinExpressionsView>();
         container.Register<IIndividualMoleculePropertiesView, IndividualMoleculePropertiesView>();
         container.Register<IOntogenySelectionView, OntogenySelectionView>();
         container.Register<IMultiParameterEditView, MultiParameterEditView>();
         container.Register<IScaleParametersView, ScaleParametersView>();
      }

      private static void registerMinimalImplementations(IContainer container)
      {
         container.Register<ICoreWorkspace, IWorkspace, CLIWorkspace>(LifeStyle.Singleton);
         container.Register<IUserSettings, ICoreUserSettings, OSPSuite.Core.ICoreUserSettings, IPresentationUserSettings, BatchUserSettings>(
            LifeStyle.Singleton);
         container.Register<IProgressUpdater, NoneProgressUpdater>();
         container.Register<IDisplayUnitRetriever, CLIDisplayUnitRetriever>();
         container.Register<IOntogenyTask<Individual>, CLIIndividualOntogenyTask>();
         container.Register<IExceptionManager, CLIExceptionManager>();
         container.Register<IDiagramModelToXmlMapper, CLIDiagramModelToXmlMapper>();
         container.Register<IDiagramModel, CLIDiagramModel>();
         container.Register<IJournalDiagramManagerFactory, CLIJournalDiagramManagerFactory>();
      }
   }
}