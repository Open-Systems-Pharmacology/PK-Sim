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
      public ObservedDataContextMenu(DataRepository dataRepository, Simulation activeSimulation)
         : base(dataRepository, activeSimulation)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(DataRepository dataRepository, Simulation activeSimulation)
      {
         yield return CreateMenuButton.WithCaption(MenuNames.Edit)
            .WithCommandFor<EditSubjectUICommand<DataRepository>, DataRepository>(dataRepository)
            .WithIcon(ApplicationIcons.Edit);

         yield return CreateMenuButton.WithCaption(MenuNames.Rename)
            .WithCommandFor<RenameObservedDataUICommand, DataRepository>(dataRepository)
            .WithIcon(ApplicationIcons.Rename);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SaveAsTemplate)
            .WithCommandFor<SaveObservedDataToTemplateDatabaseCommand, DataRepository>(dataRepository)
            .WithIcon(ApplicationIcons.SaveAsTemplate)
            .AsGroupStarter();

         yield return CreateMenuButton.WithCaption(Captions.ExportToExcel.WithEllipsis())
            .WithCommandFor<ExportObservedDataToExcelCommand, DataRepository>(dataRepository)
            .WithIcon(ApplicationIcons.Excel);
            
         yield return CreateMenuButton.WithCaption(MenuNames.ExportToPDF)
            .WithCommandFor<ExportToPDFCommand<DataRepository>, DataRepository>(dataRepository)
            .WithIcon(ApplicationIcons.PDF);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportToPKML)
            .WithCommandFor<ExportObservedDataToPkmlCommand, DataRepository>(dataRepository)
            .WithIcon(ApplicationIcons.PKMLSave);

         yield return GenericMenu.AddToJournal(dataRepository);

         yield return GenericMenu.ExportSnapshotMenuFor(dataRepository);

         if (activeSimulation != null && !activeSimulation.UsesObservedData(dataRepository))
            yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AddObservedDataToSimulation(activeSimulation.Name))
               .WithCommand(IoC.Resolve<AddObservedDataToActiveSimulationUICommand>().For(dataRepository).For(activeSimulation))
               .WithIcon(ApplicationIcons.Simulation);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Delete)
            .WithCommandFor<DeleteObservedDataUICommand, DataRepository>(dataRepository)
            .WithIcon(ApplicationIcons.Delete)
            .AsGroupStarter();
      }
   }

   public class ObservedDataTreeNodeContextMenuFactory : IContextMenuSpecificationFactory<ITreeNode>
   {
      private readonly IActiveSubjectRetriever _activeSubjectRetriever;

      public ObservedDataTreeNodeContextMenuFactory(IActiveSubjectRetriever activeSubjectRetriever)
      {
         _activeSubjectRetriever = activeSubjectRetriever;
      }

      public bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return treeNode.IsAnImplementationOf<ObservedDataNode>();
      }

      public IContextMenu CreateFor(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ObservedDataContextMenu(treeNode.DowncastTo<ObservedDataNode>().Tag.Repository,_activeSubjectRetriever.Active< Simulation>());
      }
   }
}