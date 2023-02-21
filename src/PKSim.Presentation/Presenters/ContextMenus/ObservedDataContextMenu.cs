using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ObservedDataContextMenu : ContextMenu<DataRepository, Simulation>
   {
      public ObservedDataContextMenu(DataRepository dataRepository, Simulation activeSimulation, IContainer container)
         : base(dataRepository, activeSimulation, container)
      {
      }

      //somewhere here we should also add
      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(DataRepository dataRepository, Simulation activeSimulation)
      {
         yield return CreateMenuButton.WithCaption(MenuNames.Edit)
            .WithCommandFor<EditSubjectUICommand<DataRepository>, DataRepository>(dataRepository, _container)
            .WithIcon(ApplicationIcons.Edit);

         yield return CreateMenuButton.WithCaption(MenuNames.Rename)
            .WithCommandFor<RenameObservedDataUICommand, DataRepository>(dataRepository, _container)
            .WithIcon(ApplicationIcons.Rename);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SaveAsTemplate)
            .WithCommandFor<SaveObservedDataToTemplateDatabaseCommand, DataRepository>(dataRepository, _container)
            .WithIcon(ApplicationIcons.SaveAsTemplate)
            .AsGroupStarter();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ReloadAllRelated.WithEllipsis())
            .WithCommandFor<ReloadAllObservedDataCommand, DataRepository>(dataRepository, _container)
            .AsDisabledIf(string.IsNullOrEmpty(dataRepository.ConfigurationId))
            .WithIcon(ApplicationIcons.RefreshAll);

         yield return CreateMenuButton.WithCaption(Captions.ExportToExcel.WithEllipsis())
            .WithCommandFor<ExportObservedDataToExcelCommand, DataRepository>(dataRepository, _container)
            .WithIcon(ApplicationIcons.Excel);
            
         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportToPKML)
            .WithCommandFor<ExportObservedDataToPkmlCommand, DataRepository>(dataRepository, _container)
            .WithIcon(ApplicationIcons.PKMLSave);

         yield return GenericMenu.AddToJournal(dataRepository, _container);

         yield return GenericMenu.ExportSnapshotMenuFor(dataRepository, _container);

         if (activeSimulation != null && !activeSimulation.UsesObservedData(dataRepository))
            yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AddObservedDataToSimulation(activeSimulation.Name))
               .WithCommand(IoC.Resolve<AddObservedDataToActiveSimulationUICommand>().For(dataRepository).For(activeSimulation))
               .WithIcon(ApplicationIcons.Simulation);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Delete)
            .WithCommandFor<DeleteObservedDataUICommand, DataRepository>(dataRepository, _container)
            .WithIcon(ApplicationIcons.Delete)
            .AsGroupStarter();
      }
   }

   public class ObservedDataTreeNodeContextMenuFactory : IContextMenuSpecificationFactory<ITreeNode>
   {
      private readonly IActiveSubjectRetriever _activeSubjectRetriever;
      private readonly IContainer _container;

      public ObservedDataTreeNodeContextMenuFactory(IActiveSubjectRetriever activeSubjectRetriever, IContainer container)
      {
         _activeSubjectRetriever = activeSubjectRetriever;
         _container = container;
      }

      public bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return treeNode.IsAnImplementationOf<ObservedDataNode>();
      }

      public IContextMenu CreateFor(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ObservedDataContextMenu(treeNode.DowncastTo<ObservedDataNode>().Tag.Repository,_activeSubjectRetriever.Active< Simulation>(), _container);
      }
   }
}