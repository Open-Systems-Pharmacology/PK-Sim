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
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Binders;
using OSPSuite.UI.Mappers;
using OSPSuite.UI.Services;
using OSPSuite.UI.Views;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Format;
using PKSim.CLI.Core.MinimalImplementations;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Presentation;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Presenters.Individuals.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Presenters.Parameters.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.ExpressionProfiles;
using PKSim.Presentation.Views.Individuals;
using PKSim.Presentation.Views.Parameters;
using PKSim.UI.Views;
using PKSim.UI.Views.ExpressionProfiles;
using PKSim.UI.Views.Individuals;
using PKSim.UI.Views.Parameters;
using RenameObjectDTOFactory = OSPSuite.Presentation.DTO.RenameObjectDTOFactory;

namespace PKSim.UI.Starter
{
   public static class ApplicationStartup
   {
      static IContainer _container;

      public static IContainer Initialize(IShell shell)
      {
         if (_container != null)
            return _container;

         _container = initializeForStartup(shell);

         return _container;
      }

      private static IContainer initializeForStartup(IShell shell)
      {
         Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
         Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
         ApplicationIcons.DefaultIcon = ApplicationIcons.PKSim;


         var container = InfrastructureRegister.Initialize(registerContainerAsStatic: false);
         container.RegisterImplementationOf(SynchronizationContext.Current);

         container.Register<IApplicationController, ApplicationController>(LifeStyle.Singleton);
         container.Register<ICoreWorkspace, OSPSuite.Core.IWorkspace, IWorkspace, Workspace>(LifeStyle.Singleton);

         registerMinimalImplementations(container);


         using (container.OptimizeDependencyResolution())
         {
            var PKSimShell = new Shell(container);
            container.RegisterImplementationOf(shell);
            container.RegisterImplementationOf(PKSimShell);
            container.AddRegister(x => x.FromType<CoreRegister>());
            container.AddRegister(x => x.FromType<InfrastructureRegister>());
            container.Register<ICreateExpressionProfilePresenter, CreateExpressionProfilePresenter>();
            container.Register<ICreateExpressionProfileView, CreateExpressionProfileView>();
            container.Register<ICreateIndividualPresenter, CreateIndividualPresenter>();
            container.Register<ICreateIndividualView, CreateIndividualView>();
            container.Register<IExpressionProfileToExpressionProfileDTOMapper, ExpressionProfileToExpressionProfileDTOMapper>();
            container.Register<IMoleculePropertiesMapper, MoleculePropertiesMapper>();
            container.Register<ICoreUserSettings, IPresentationUserSettings, UIStarterUserSettings>(LifeStyle.Singleton);
            container.Register<IOntogenyTask, OntogenyTask>();
            container.Register<IEntityTask, EntityTask>();
            container.Register<IRenameObjectDTOFactory, RenameObjectDTOFactory>();
            container.Register<IHistoryManager, HistoryManager<IExecutionContext>>();
            container.Register<ISubPresenterItemManager<IIndividualItemPresenter>, SubPresenterItemManager<IIndividualItemPresenter>>();
            container.Register<IBuildingBlockPropertiesMapper, BuildingBlockPropertiesMapper>();
            container.Register<IObjectBaseDTOFactory, ObjectBaseDTOFactory>();
            container.Register<IIndividualSettingsPresenter, IndividualSettingsPresenter>();
            container.Register<IIndividualSettingsView, IndividualSettingsView>();
            container.Register<IToolTipCreator, ToolTipCreator>();
            container.Register<IToolTipPartCreator, ToolTipPartCreator>();
            container.Register<IReportPartToToolTipPartsMapper, ReportPartToToolTipPartsMapper>();
            container.Register<IIndividualDefaultValueUpdater, IndividualDefaultValuesUpdater>();
            container.Register<IIndividualSettingsDTOToOriginDataMapper, IndividualSettingsDTOToOriginDataMapper>();
            container.Register<IParameterToParameterDTOMapper, ParameterToParameterDTOMapper>();
            container.Register<ISubPopulationToSubPopulationDTOMapper, SubPopulationToSubPopulationDTOMapper>();
            container
               .Register<IParameterValueVersionToCategoryParameterValueVersionDTOMapper,
                  ParameterValueVersionToCategoryParameterValueVersionDTOMapper>();
            container.Register<ICalculationMethodToCategoryCalculationMethodDTOMapper, CalculationMethodToCategoryCalculationMethodDTOMapper>();
            container.Register<IIndividualToIIndividualSettingsDTOMapper, IndividualToIIndividualSettingsDTOMapper>();
            container.Register<IOriginDataParameterToParameterDTOMapper, OriginDataParameterToParameterDTOMapper>();
            container.Register<IIndividualSettingsDTOToIndividualMapper, IndividualSettingsDTOToIndividualMapper>();
            container.Register<IEditValueOriginPresenter, EditValueOriginPresenter>();
            container.Register<IEditValueOriginView, EditValueOriginView>();
            container.Register<ValueOriginBinder<ValueOriginDTO>, ValueOriginBinder<ValueOriginDTO>>();
            container.Register<IValueOriginPresenter, ValueOriginPresenter>();
            container.Register<IValueOriginView, ValueOriginView>();
            container.Register<OSPSuite.UI.Services.IToolTipCreator, ToolTipCreator>();
            container.Register<IIndividualParametersPresenter, IndividualParametersPresenter>();
            container.Register<IIndividualParametersView, IndividualParametersView>();
            container.Register<IParameterGroupsPresenter, ParameterGroupsPresenter>();
            container.Register<IParameterGroupsView, ParameterGroupsView>();
            container.Register<IParameterGroupNodeCreator, ParameterGroupNodeCreator>();
            container.Register<IParameterGroupToTreeNodeMapper, ParameterGroupToTreeNodeMapper>();
            container.Register<ITreeNodeFactory, TreeNodeFactory>();
            container.Register<IDynamicGroupExpander, DynamicGroupExpander>();
            container.Register<IParameterContainerToTreeNodeMapper, ParameterContainerToTreeNodeMapper>();
            container.Register<INodeToCustomizableParametersPresenterMapper, NodeToCustomizableParametersPresenterMapper>();
            container.Register<IContainerToCustomableParametersPresenterMapper, ContainerToCustomableParametersPresenterMapper>();
            container.Register<IParameterGroupToCustomizableParametersPresenter, ParameterGroupToCustomizableParametersPresenter>();
            container.RegisterFactory<IMultiParameterEditPresenterFactory>();
            container.Register<INoItemInSelectionPresenter, NoItemInSelectionPresenter>();
            container.Register<INoItemInSelectionView, NoItemInSelectionView>();
            container.Register<IUserSettings, UserSettings>();
            container.Register<INumericFormatterOptions, NumericFormatterOptions>();
            container.Register<ISkinManager, SkinManager>();
            container.Register<DirectoryMapSettings, DirectoryMapSettings>();
            container.Register<IPresentationSettingsTask, PresentationSettingsTask>();
            container.Register<IWorkspace, Infrastructure.Workspace>();
            container.Register<IWithWorkspaceLayout, Infrastructure.Workspace>();
            container.Register<IWorkspaceLayout, WorkspaceLayout>();
            container.Register<ITreeNodeContextMenuFactory, TreeNodeContextMenuFactory>();
            container.Register<IIndividualMoleculesPresenter, IndividualMoleculesPresenter>();
            container.Register<IMoleculesView, MoleculesView>();
            container.Register<IEditMoleculeTask<Individual>, EditMoleculeTask<Individual>>();
            container
               .Register<IRootNodeToIndividualExpressionsPresenterMapper<Individual>, RootNodeToIndividualExpressionsPresenterMapper<Individual>>();
            container.Register<IFavoriteParametersPresenter, FavoriteParametersPresenter>();
            container.Register<IFavoriteParametersView, FavoriteParametersView>();
            container.Register<IMultiParameterEditPresenter, MultiParameterEditPresenter>();
            container.Register<IMultiParameterEditView, MultiParameterEditView>();
            container.Register<ValueOriginBinder<ParameterDTO>, ValueOriginBinder<ParameterDTO>>();
            container.Register<PathElementsBinder<ParameterDTO>, PathElementsBinder<ParameterDTO>>();
            container.Register<IScaleParametersPresenter, ScaleParametersPresenter>();
            container.Register<IScaleParametersView, ScaleParametersView>();
            container.Register<IEditParameterPresenterTask, EditParameterPresenterTask>();
            container.Register<IParameterContextMenuFactory, ParameterContextMenuFactory>();
            container.Register<IIndividualToIndividualBuildingBlockMapper, IndividualToIndividualBuildingBlockMapper>();

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