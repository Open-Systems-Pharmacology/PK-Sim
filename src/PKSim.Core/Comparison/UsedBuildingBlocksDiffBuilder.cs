using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using OSPSuite.Core.Comparison;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Comparison
{
   public class UsedBuildingBlocksDiffBuilder : DiffBuilder<IEnumerable<UsedBuildingBlock>>
   {
      public override void Compare(IComparison<IEnumerable<UsedBuildingBlock>> comparison)
      {
         compareUsedBuildingBlocks(comparison, PKSimBuildingBlockType.Individual);
         compareUsedBuildingBlocks(comparison, PKSimBuildingBlockType.Population);
         compareUsedBuildingBlocks(comparison, PKSimBuildingBlockType.Compound);
         compareUsedBuildingBlocks(comparison, PKSimBuildingBlockType.Protocol);
         compareUsedBuildingBlocks(comparison, PKSimBuildingBlockType.Formulation);
         compareUsedBuildingBlocks(comparison, PKSimBuildingBlockType.Event);
      }

      private void compareUsedBuildingBlocks(IComparison<IEnumerable<UsedBuildingBlock>> comparison, PKSimBuildingBlockType buildingBlockType)
      {
         var buildingBlocks1 = ofType(comparison.Object1, buildingBlockType);
         var buildingBlocks2 = ofType(comparison.Object2, buildingBlockType);

         foreach (var keyValue in buildingBlocks1.KeyValues.Where(kv => !buildingBlocks2.Contains(kv.Key)))
         {
            addMissingItem(comparison, keyValue.Value, null, buildingBlockType);
         }

         foreach (var keyValue in buildingBlocks2.KeyValues.Where(kv => !buildingBlocks1.Contains(kv.Key)))
         {
            addMissingItem(comparison, null, keyValue.Value, buildingBlockType);
         }
      }

      private void addMissingItem(IComparison<IEnumerable<UsedBuildingBlock>> comparison, UsedBuildingBlock buildingBlock1, UsedBuildingBlock buildingBlock2, PKSimBuildingBlockType buildingBlockType)
      {
         var missingObject = buildingBlock1 ?? buildingBlock2;
         var missingItem = new MissingDiffItem
         {
            Object1 = buildingBlock1,
            Object2 = buildingBlock2,
            MissingObject1 = buildingBlock1,
            MissingObject2 = buildingBlock2,
            CommonAncestor = comparison.CommonAncestor,
            MissingObjectName = missingObject.Name,
            MissingObjectType = buildingBlockType.ToString()
         };

         comparison.Add(missingItem);
      }

      private Cache<string, UsedBuildingBlock> ofType(IEnumerable<UsedBuildingBlock> usedBuildingBlocks, PKSimBuildingBlockType buildingBlockType)
      {
         var cache = new Cache<string, UsedBuildingBlock>(x => x.Name, x => null);
         cache.AddRange(usedBuildingBlocks.Where(x => x.BuildingBlockType.Is(buildingBlockType)));
         return cache;
      }
   }
}