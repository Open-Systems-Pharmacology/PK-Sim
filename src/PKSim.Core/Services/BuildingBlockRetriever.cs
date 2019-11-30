using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IBuildingBlockRetriever
   {
      /// <summary>
      ///    Retrieve the building block containing the given entity
      /// </summary>
      IPKSimBuildingBlock BuildingBlockContaining(IEntity entity);

      /// <summary>
      ///    Retrieve the building block with the given id
      /// </summary>
      IPKSimBuildingBlock BuildingBlockWithId(string buildingBlockId);

      /// <summary>
      ///    Retrieve the building block with the given id and the given type
      /// </summary>
      TBuildingBlock BuildingBlockWithId<TBuildingBlock>(string buildingBlockId) where TBuildingBlock : class, IPKSimBuildingBlock;

      /// <summary>
      ///    Retrieve the id of the building block containing the given entity
      /// </summary>
      string BuildingBlockIdContaining(IEntity entity);
   }

   public class BuildingBlockRetriever : IBuildingBlockRetriever
   {
      private readonly IWithIdRepository _withIdRepository;

      public BuildingBlockRetriever(IWithIdRepository withIdRepository)
      {
         _withIdRepository = withIdRepository;
      }

      public IPKSimBuildingBlock BuildingBlockContaining(IEntity entity)
      {
         return findBuildingBlockFor(entity);
      }

      public IPKSimBuildingBlock BuildingBlockWithId(string buildingBlockId)
      {
         return BuildingBlockWithId<IPKSimBuildingBlock>(buildingBlockId);
      }

      public TBuildingBlock BuildingBlockWithId<TBuildingBlock>(string buildingBlockId) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         if (_withIdRepository.ContainsObjectWithId(buildingBlockId))
            return _withIdRepository.Get<IPKSimBuildingBlock>(buildingBlockId) as TBuildingBlock;

         return null;
      }

      public string BuildingBlockIdContaining(IEntity entity)
      {
         var buildingBlock = BuildingBlockContaining(entity);
         return buildingBlock != null ? buildingBlock.Id : string.Empty;
      }

      private IPKSimBuildingBlock findBuildingBlockFor(IEntity entity)
      {
         if (entity == null) return null;
         var buildingBlock = entity as IPKSimBuildingBlock;
         return buildingBlock ?? findBuildingBlockFor(entity.ParentContainer);
      }
   }
}