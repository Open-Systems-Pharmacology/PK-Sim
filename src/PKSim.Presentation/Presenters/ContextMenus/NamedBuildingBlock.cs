using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class NamedBuildingBlock<TBuildingBlock>  where TBuildingBlock : IPKSimBuildingBlock
   {
      public TBuildingBlock BuildingBlock { get; }
      public string Name { get; }

      public NamedBuildingBlock(TBuildingBlock buildingBlock, string name)
      {
         BuildingBlock = buildingBlock;
         Name = name;
      }
   }
}