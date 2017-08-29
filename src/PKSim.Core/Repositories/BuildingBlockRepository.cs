using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

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
      private readonly IProjectRetriever _workspace;

      public BuildingBlockRepository(IProjectRetriever workspace)
      {
         _workspace = workspace;
      }

      public IEnumerable<IPKSimBuildingBlock> All()
      {
         return All<IPKSimBuildingBlock>();
      }

      public IEnumerable<TBuildingBlock> All<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         if (_workspace.CurrentProject == null)
            return new List<TBuildingBlock>();

         return _workspace.CurrentProject.DowncastTo<PKSimProject>().All<TBuildingBlock>();
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