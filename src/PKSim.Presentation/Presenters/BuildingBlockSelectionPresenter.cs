using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Assets;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation.Presenters
{
   public interface IBuildingBlockSelectionPresenter : IPresenter<IBuildingBlockSelectionView>
   {
      IEnumerable<IPKSimBuildingBlock> AllAvailableBlocks { get; }
      string BuildingBlockType { get; }
      IPKSimBuildingBlock BuildingBlock { get; }
      bool DisplayNotification { set; }
      void Edit(IPKSimBuildingBlock buildingBlock);
      void CreateBuildingBlock();
      ApplicationIcon IconFor(IPKSimBuildingBlock buildingBlock);
      void LoadBuildingBlock();
      string DisplayFor(IPKSimBuildingBlock buildingBlock);
      IEnumerable<ToolTipPart> ToolTipPartsFor(int selectedIndex);
      bool AllowEmptySelection { set; }
   }

   public interface IBuildingBlockSelectionPresenter<TBuildingBlock> : IBuildingBlockSelectionPresenter where TBuildingBlock : IPKSimBuildingBlock
   {
   }

   public class BuildingBlockSelectionPresenter<TBuildingBlock> : AbstractPresenter<IBuildingBlockSelectionView, IBuildingBlockSelectionPresenter>, IBuildingBlockSelectionPresenter<TBuildingBlock>
      where TBuildingBlock : class, IPKSimBuildingBlock
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly IContainer _container;
      private readonly IBuildingBlockSelectionDisplayer _buildingBlockSelectionDisplayer;
      private readonly BuildingBlockSelectionDTO _buildingBlockDTO;
      private IPKSimBuildingBlock _editedBuildingBlock;
      private readonly TBuildingBlock _emptySelection;
      public string BuildingBlockType { get; }

      public BuildingBlockSelectionPresenter(IBuildingBlockSelectionView view, IObjectTypeResolver objectTypeResolver,
         IBuildingBlockRepository buildingBlockRepository, IContainer container,
         IObjectBaseFactory objectBaseFactory, IBuildingBlockSelectionDisplayer buildingBlockSelectionDisplayer)
         : base(view)
      {
         _buildingBlockRepository = buildingBlockRepository;
         _container = container;
         _buildingBlockSelectionDisplayer = buildingBlockSelectionDisplayer;
         BuildingBlockType = objectTypeResolver.TypeFor<TBuildingBlock>();
         _buildingBlockDTO = new BuildingBlockSelectionDTO {BuildingBockType = BuildingBlockType};
         _view.DisplayIcons = true;
         _emptySelection = objectBaseFactory.Create<TBuildingBlock>().WithName(PKSimConstants.UI.None);
      }

      public IEnumerable<IPKSimBuildingBlock> AllAvailableBlocks
      {
         get
         {
            var allTemplateBuildingBlocks = _buildingBlockRepository.All<TBuildingBlock>()
               .Select(x => x.DowncastTo<IPKSimBuildingBlock>()).ToList();

            if (_buildingBlockDTO.AllowEmptySelection)
               yield return _emptySelection;

            //not a template building block 
            if (_editedBuildingBlock != null && !allTemplateBuildingBlocks.Contains(_editedBuildingBlock))
               yield return _editedBuildingBlock;

            foreach (var templateBuildingBlock in allTemplateBuildingBlocks.OrderBy(x => x.Name))
            {
               yield return templateBuildingBlock;
            }
         }
      }

      public bool AllowEmptySelection
      {
         set
         {
            _buildingBlockDTO.AllowEmptySelection = value;
            //Refresh list is required so that the view rebinds to the list of available compounds
            _view.RefreshBuildingBlockList();
            if (!_buildingBlockDTO.AllowEmptySelection)
               resetSelectedBuildingBlock();
         }
      }

      public bool DisplayNotification
      {
         set { _buildingBlockDTO.ValidateBuildingBlock = value; }
      }

      public void Edit(IPKSimBuildingBlock buildingBlock)
      {
         _editedBuildingBlock = buildingBlock;
         resetSelectedBuildingBlock();
         _view.BindTo(_buildingBlockDTO);
      }

      private void resetSelectedBuildingBlock()
      {
         _buildingBlockDTO.BuildingBlock = _editedBuildingBlock ?? AllAvailableBlocks.FirstOrDefault();
      }

      public IPKSimBuildingBlock BuildingBlock
      {
         get
         {
            var selectedBuildingBlock = _buildingBlockDTO.BuildingBlock;
            if (_buildingBlockDTO.AllowEmptySelection && Equals(selectedBuildingBlock, _emptySelection))
               return null;

            return _buildingBlockDTO.BuildingBlock;
         }
      }

      public void CreateBuildingBlock()
      {
         var buildingBlockTask = _container.Resolve<IBuildingBlockTask<TBuildingBlock>>();
         buildingBlockTask.AddToProject();
         Edit(_buildingBlockRepository.All<TBuildingBlock>().LastOrDefault());
      }

      public ApplicationIcon IconFor(IPKSimBuildingBlock buildingBlock)
      {
         return _buildingBlockSelectionDisplayer.IconFor(buildingBlock, _emptySelection);
      }

      public void LoadBuildingBlock()
      {
         var buildingBlockTask = _container.Resolve<IBuildingBlockTask<TBuildingBlock>>();
         buildingBlockTask.LoadSingleFromTemplate();
         Edit(_buildingBlockRepository.All<TBuildingBlock>().LastOrDefault());
      }

      public string DisplayFor(IPKSimBuildingBlock buildingBlock)
      {
         return _buildingBlockSelectionDisplayer.DisplayNameFor(buildingBlock, _emptySelection);
      }

      public IEnumerable<ToolTipPart> ToolTipPartsFor(int selectedIndex)
      {
         var buildingBlock = AllAvailableBlocks.ElementAtOrDefault(selectedIndex) as TBuildingBlock;
         return _buildingBlockSelectionDisplayer.ToolTipFor(buildingBlock);
      }

   }
}