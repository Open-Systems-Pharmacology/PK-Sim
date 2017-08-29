using OSPSuite.Assets;
using OSPSuite.Core;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Comparisons;
using OSPSuite.Presentation.Presenters.Journal;
using OSPSuite.Presentation.Presenters.Main;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Format;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Presenters.ContextMenus;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Presenters.Individuals.Mappers;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Presenters.Snapshots;
using PKSim.Presentation.Repositories;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using JournalDiagramMainPresenter = PKSim.Presentation.Presenters.Main.JournalDiagramMainPresenter;
using JournalPresenter = PKSim.Presentation.Presenters.Main.JournalPresenter;
using MainComparisonPresenter = PKSim.Presentation.Presenters.Main.MainComparisonPresenter;

namespace PKSim.Presentation
{
   public class PresenterRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<PresenterRegister>();

            //Main Presenters that should be registered as singletong
            scan.ExcludeNamespaceContainingType<PKSimMainViewPresenter>();

            //Context menu will be registered with another convention
            scan.ExcludeNamespaceContainingType<IndividualSimulationContextMenu>();

            //UI Commands will be registered with another convention
            scan.ExcludeNamespaceContainingType<IExitCommand>();

            //This specifc objects needs to be register as Singleton
            scan.ExcludeType<CloseSubjectPresenterInvoker>();
            scan.ExcludeType<ExportToPDFInvoker>();
            scan.ExcludeType<ButtonGroupRepository>();
            scan.ExcludeType<MenuBarItemRepository>();

            //This objects were already registered in Bootstrap
            scan.ExcludeType<ApplicationController>();
            scan.ExcludeType<PKSimProgressUpdater>();
            scan.ExcludeType<StartOptions>();

            scan.RegisterAs(LifeStyle.Transient);
            scan.WithConvention<PKSimRegistrationConvention>();
         });


         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<PresenterRegister>();

            scan.IncludeType<CloseSubjectPresenterInvoker>();
            scan.IncludeType<ExportToPDFInvoker>();
            scan.IncludeType<ButtonGroupRepository>();
            scan.IncludeType<MenuBarItemRepository>();

            scan.RegisterAs(LifeStyle.Singleton);
            scan.WithConvention<OSPSuiteRegistrationConvention>();
         });


         container.Register<IMainViewPresenter, PKSimMainViewPresenter>(LifeStyle.Singleton);
         container.Register<IBuildingBlockExplorerPresenter, IMainViewItemPresenter, BuildingBlockExplorerPresenter>(LifeStyle.Singleton);
         container.Register<IHistoryPresenter, IMainViewItemPresenter, HistoryPresenter>(LifeStyle.Singleton);
         container.Register<IJournalDiagramMainPresenter, IMainViewItemPresenter, JournalDiagramMainPresenter>(LifeStyle.Singleton);
         container.Register<IJournalPresenter, IMainViewItemPresenter, JournalPresenter>(LifeStyle.Singleton);
         container.Register<IMenuAndToolBarPresenter, IMainViewItemPresenter, MenuAndToolBarPresenter>(LifeStyle.Singleton);
         container.Register<ISimulationExplorerPresenter, IMainViewItemPresenter, SimulationExplorerPresenter>(LifeStyle.Singleton);
         container.Register<IStatusBarPresenter, IMainViewItemPresenter, StatusBarPresenter>(LifeStyle.Singleton);
         container.Register<IMainComparisonPresenter, IMainViewItemPresenter, MainComparisonPresenter>(LifeStyle.Singleton);

         registerContextMenus(container);

         registerUICommands(container);

         registerSingleStartPresenters(container);

         //Building block tasks have a special registration mechanism
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<PresenterRegister>();
            scan.IncludeNamespaceContainingType<BuildingBlockTask>();
            scan.WithConvention<BuildingBlockTaskRegistrationConvention>();
         });

         //Abstract Factory 
         container.RegisterFactory<IMultiParameterEditPresenterFactory>();
         container.RegisterFactory<IHeavyWorkPresenterFactory>();
         container.RegisterFactory<ISimulationAnalysisPresenterFactory>();

         //Open generic type
         container.Register(typeof(IBuildingBlockSelectionPresenter<>), typeof(BuildingBlockSelectionPresenter<>));
         container.Register(typeof(IMoleculeExpressionTask<>), typeof(MoleculeExpressionTask<>));
         container.Register(typeof(IRootNodeToIndividualExpressionsPresenterMapper<>), typeof(RootNodeToIndividualExpressionsPresenterMapper<>));
         container.Register(typeof(IIndividualEnzymeExpressionsPresenter<>), typeof(IndividualEnzymeExpressionsPresenter<>));
         container.Register(typeof(IIndividualOtherProteinExpressionsPresenter<>), typeof(IndividualOtherProteinExpressionsPresenter<>));
         container.Register(typeof(IIndividualTransporterExpressionsPresenter<>), typeof(IndividualTransporterExpressionsPresenter<>));
         container.Register(typeof(IIndividualMoleculePropertiesPresenter<>), typeof(IndividualMoleculePropertiesPresenter<>));
         container.Register(typeof(IOntogenySelectionPresenter<>), typeof(OntogenySelectionPresenter<>));
         container.Register(typeof(ILoadFromSnapshotPresenter<>), typeof(LoadFromSnapshotPresenter<>));

         //generic types
         container.Register<ISimulationOutputSelectionPresenter<IndividualSimulation>, IndividualSimulationSettingsPresenter>();
         container.Register<ISimulationOutputSelectionPresenter<PopulationSimulation>, PopulationSimulationSettingsPresenter>();

         container.Register<IFormatterFactory, FormatterFactory>();

         ApplicationIcons.DefaultIcon = ApplicationIcons.PKSim;
         Captions.SimulationPath = PKSimConstants.UI.Simulation;
         Captions.ContainerPath = PKSimConstants.UI.Organ;
         Captions.BottomCompartmentPath = PKSimConstants.UI.Compartment;
         Captions.NamePath = PKSimConstants.UI.Name;
         Captions.MoleculePath = PKSimConstants.UI.Molecule;

         //specific PKSim Implementations
         container.Register<IPathToPathElementsMapper, PKSimPathToPathElementsMapper>();
         container.Register<IDataColumnToPathElementsMapper, PKSimDataColumnToPathElementsMapper>();
         container.Register<IQuantityPathToQuantityDisplayPathMapper, PKSimQuantityPathToQuantityDisplayPathMapper>();
      }

      private static void registerSingleStartPresenters(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<PresenterRegister>();
            scan.IncludeNamespaceContainingType<AboutPresenter>();
            scan.WithConvention(new RegisterTypeConvention<ISingleStartPresenter>(registerWithDefaultConvention: false));
         });
      }

      private static void registerUICommands(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<PresenterRegister>();
            scan.IncludeNamespaceContainingType<IExitCommand>();
            scan.WithConvention<ConcreteTypeRegistrationConvention>();
         });
      }

      private static void registerContextMenus(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<PresenterRegister>();
            scan.IncludeNamespaceContainingType<IndividualSimulationContextMenu>();
            scan.WithConvention<AllInterfacesAndConcreteTypeRegistrationConvention>();
         });
      }
   }
}