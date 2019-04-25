using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Repositories
{
   public interface IBuildingBlockRepository : IRepository<IPKSimBuildingBlock>
   {
      IEnumerable<TBuildingBlock> All<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock;
      TBuildingBlock ById<TBuildingBlock>(string templateId) where TBuildingBlock : class, IPKSimBuildingBlock;
      IPKSimBuildingBlock ById(string templateId);
      TBuildingBlock FirstOrDefault<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock;
   }

   public class BuildingBlockRepository : IBuildingBlockRepository
   {
      private readonly IPKSimProjectRetriever _projectRetriever;

      public BuildingBlockRepository(IPKSimProjectRetriever projectRetriever)
      {
         _projectRetriever = projectRetriever;
      }

      public IEnumerable<IPKSimBuildingBlock> All()
      {
         return All<IPKSimBuildingBlock>();
      }

      public IEnumerable<TBuildingBlock> All<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         if (_projectRetriever.Current == null)
            return new List<TBuildingBlock>();

         return _projectRetriever.Current.All<TBuildingBlock>();
      }

      public TBuildingBlock ById<TBuildingBlock>(string templateId) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return All<TBuildingBlock>().FindById(templateId);
      }

      public IPKSimBuildingBlock ById(string templateId)
      {
         return ById<IPKSimBuildingBlock>(templateId);
      }

      public TBuildingBlock FirstOrDefault<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return All<TBuildingBlock>().FirstOrDefault();
      }
   }
}