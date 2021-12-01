using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatContainerRepository : IMetaDataRepository<FlatContainer>
   {
      FlatContainer ContainerFrom(int? containerId);
      FlatContainer ParentContainerFrom(int containerId);

      FlatContainer ContainerFrom(string containerPath);

      IObjectPath ContainerPathFrom(int? containerId);
   }

   public class FlatContainerRepository : MetaDataRepository<FlatContainer>, IFlatContainerRepository
   {
      private readonly ICache<int, FlatContainer> _flatContainersCachedById= new Cache<int, FlatContainer>(x=>x.Id,x=>null);
      private readonly ICache<string, FlatContainer> _flatContainersCachedByPath = new Cache<string, FlatContainer>(x=>x.ContainerPath,x=>null);

      public FlatContainerRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatContainer> mapper) : base(dbGateway, mapper, CoreConstants.ORM.VIEW_CONTAINERS)
      {
      }

      public FlatContainer ContainerFrom(int? containerId)
      {
         Start();
         if (containerId == null)
            return null;

         return _flatContainersCachedById[containerId.Value];
      }

      protected override void PerformPostStartProcessing()
      {
         //first set container cache by id
         AllElements().Each(_flatContainersCachedById.Add);

         //save path in all containers to avoid multiple evaluations
         AllElements().Each(fc => fc.ContainerPath = ContainerPathFrom(fc.Id).ToString());

         //last update cache
         AllElements().Each(_flatContainersCachedByPath.Add);
      }

      public FlatContainer ParentContainerFrom(int containerId)
      {
         var container = ContainerFrom(containerId);
         var parentContainerId = container.ParentId;

         if (parentContainerId == null)
            return null;

         return ContainerFrom(parentContainerId);
      }

      public FlatContainer ContainerFrom(string containerPath)
      {
         Start();
         return _flatContainersCachedByPath[containerPath];
      }

      public IObjectPath ContainerPathFrom(int? containerId)
      {
         Start();
         var path = new ObjectPath();
         while (containerId != null)
         {
            var container = ContainerFrom(containerId);

            if (container.Name != Constants.ROOT)
               path.AddAtFront(container.Name);

            containerId = container.ParentId;
         }

          return path;
      }

   
   }
}