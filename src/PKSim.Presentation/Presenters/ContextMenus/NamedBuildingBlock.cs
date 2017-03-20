using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class NamedBuildingBlock<TBuildingBlock>  where TBuildingBlock : IPKSimBuildingBlock
   {
      public TBuildingBlock BuildingBlock { get; private set; }
      public string Name { get; private set; }

      public NamedBuildingBlock(TBuildingBlock buildingBlock, string name)
      {
         BuildingBlock = buildingBlock;
         Name = name;
      }
   }
}