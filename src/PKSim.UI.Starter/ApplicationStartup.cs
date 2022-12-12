using System.Globalization;
using System.Threading;
using OSPSuite.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Mappers;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Exceptions;
using PKSim.CLI.Core.MinimalImplementations;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Presentation;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.ExpressionProfiles;
using PKSim.UI.Views.ExpressionProfiles;

namespace PKSim.UI.Starter
{
   public static class ApplicationStartup
   {
      static IContainer _container;

      public static IContainer Initialize(IShell shell)
      {
         if (_container != null)
            return _container;

         _container = InitializeForStartup(shell);

         return _container;
      }

      public static IContainer InitializeForStartup(IShell shell)
      {
         Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
         Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
         ApplicationIcons.DefaultIcon = ApplicationIcons.PKSim;


         var container = InfrastructureRegister.Initialize(registerContainerAsStatic: false);
         container.RegisterImplementationOf(SynchronizationContext.Current);

         container.Register<IApplicationController, ApplicationController>(LifeStyle.Singleton);
         container.Register<ICoreWorkspace, OSPSuite.Core.IWorkspace, IWorkspace, Workspace>(LifeStyle.Singleton);

         // registerImplementationsFromCore(container);
         registerMinimalImplementations(container);


         using (container.OptimizeDependencyResolution())
         {
            container.RegisterImplementationOf(shell);
            container.AddRegister(x => x.FromType<CoreRegister>());
            container.AddRegister(x => x.FromType<InfrastructureRegister>());
            container.Register<ICreateExpressionProfilePresenter, CreateExpressionProfilePresenter>();
            container.Register<ICreateExpressionProfileView, CreateExpressionProfileView>();
            container.Register<IExpressionProfileToExpressionProfileDTOMapper, ExpressionProfileToExpressionProfileDTOMapper>();
            container.Register<IMoleculePropertiesMapper, MoleculePropertiesMapper>();
            container.Register<ICoreUserSettings, IPresentationUserSettings, UIStarterUserSettings>(LifeStyle.Singleton);
            container.Register<IOntogenyTask, OntogenyTask>();
            container.Register<IEntityTask, EntityTask>();
            container.Register<IRenameObjectDTOFactory, RenameObjectDTOFactory>();
            container.Register<IHistoryManager, HistoryManager<IExecutionContext>>();
            InfrastructureRegister.LoadSerializers(container);
         }

         return container;
      }

      // /// <summary>
      // /// Use this method to register all dependencies in Core.UI and Core.Presentation
      // /// </summary>
      // private static void registerImplementationsFromCore(IContainer container)
      // {
      //    container.Register<IExceptionManager, ExceptionManager>(LifeStyle.Singleton);
      //    container.AddRegister(x => x.FromType<UIRegister>());
      //    container.AddRegister(x => x.FromType<OSPSuite.Presentation.PresenterRegister>());
      // }

      /// <summary>
      ///    Use this method to register and refine the minimal implementations for dependencies
      /// </summary>
      private static void registerMinimalImplementations(IContainer container)
      {
         container.Register<IMRUProvider, MRUProvider>();
         container.Register<IImageListRetriever, ImageListRetriever>();
         container.Register<IApplicationIconsToImageCollectionMapper, ApplicationIconsToImageCollectionMapper>();
         container.Register<IDisplayUnitRetriever, CLIDisplayUnitRetriever>();
         container.Register<IExceptionManager, CLIExceptionManager>();
         container.Register<IDialogCreator, CLIDialogCreator>();
         container.Register<IDataImporter, CLIDataImporter>();
         container.Register<IJournalDiagramManagerFactory, CLIJournalDiagramManagerFactory>();
      }
   }

   internal class Workspace : CLIWorkspace, IWorkspace
   {
      public Workspace(IRegistrationTask registrationTask) : base(registrationTask)
      {
      }

      public IWorkspaceLayout WorkspaceLayout { get; set; }
   }
}