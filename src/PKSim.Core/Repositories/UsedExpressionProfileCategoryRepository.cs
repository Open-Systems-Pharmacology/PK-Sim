using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using static PKSim.Assets.PKSimConstants.UI;

namespace PKSim.Core.Repositories
{
   public interface IUsedExpressionProfileCategoryRepository : IRepository<string>
   {
   }

   public class UsedExpressionProfileCategoryRepository : IUsedExpressionProfileCategoryRepository
   {
      private readonly IPKSimProjectRetriever _projectRetriever;

      public UsedExpressionProfileCategoryRepository(IPKSimProjectRetriever projectRetriever)
      {
         _projectRetriever = projectRetriever;
      }

      public IEnumerable<string> All()
      {
         //First add User defined molecules
         return allMoleculeDefinedInExpressionProfiles()
            .OrderBy(x => x)
            //Then predefined categories
            .Union(DefaultExpressionProfileCategories)
            .Distinct();
      }

      private IEnumerable<string> allMoleculeDefinedInExpressionProfiles()
      {
         return allBuildingBlocks<ExpressionProfile>().Select(x => x.Category);
      }

      private IEnumerable<TBuildingBlock> allBuildingBlocks<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return _projectRetriever.Current?.All<TBuildingBlock>() ?? Enumerable.Empty<TBuildingBlock>();
      }
   }
}