using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views;
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
      Task LoadBuildingBlockAsync();
      string DisplayFor(IPKSimBuildingBlock buildingBlock);
      IEnumerable<ToolTipPart> ToolTipPartsFor(int selectedIndex);
      bool AllowEmptySelection { set; }

      /// <summary>
      ///    Allows to filter the building block to show even more based on some criteria
      /// </summary>
      Func<IPKSimBuildingBlock, bool> ExtraFilter { get; set; }
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

      //By default, always allowed
      public Func<IPKSimBuildingBlock, bool> ExtraFilter { get; set; } = _ => true;

      public BuildingBlockSelectionPresenter(
         IBuildingBlockSelectionView view,
         IObjectTypeResolver objectTypeResolver,
         IBuildingBlockRepository buildingBlockRepository,
         IContainer container,
         IObjectBaseFactory objectBaseFactory,
         IBuildingBlockSelectionDisplayer buildingBlockSelectionDisplayer)
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
               .Where(ExtraFilter)
               .OrderBy(x => x.BuildingBlockType)
               .ThenBy(x => x.Name)
               .Select(x => x.DowncastTo<IPKSimBuildingBlock>())
               .ToList();

            if (_buildingBlockDTO.AllowEmptySelection)
               yield return _emptySelection;

            //not a template building block 
            if (_editedBuildingBlock != null && !allTemplateBuildingBlocks.Contains(_editedBuildingBlock))
               yield return _editedBuildingBlock;

            foreach (var templateBuildingBlock in allTemplateBuildingBlocks)
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
         set => _buildingBlockDTO.ValidateBuildingBlock = value;
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
         Edit(getNewlyAddedBuildingBlock());
      }

      public ApplicationIcon IconFor(IPKSimBuildingBlock buildingBlock)
      {
         return _buildingBlockSelectionDisplayer.IconFor(buildingBlock, _emptySelection);
      }

      public async Task LoadBuildingBlockAsync()
      {
         var buildingBlockTask = _container.Resolve<IBuildingBlockTask<TBuildingBlock>>();
         await buildingBlockTask.SecureAwait(x => x.LoadSingleFromTemplateAsync());
         Edit(getNewlyAddedBuildingBlock());
      }

      private IPKSimBuildingBlock getNewlyAddedBuildingBlock() => _buildingBlockRepository.All<TBuildingBlock>().Where(ExtraFilter).LastOrDefault();

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