using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public abstract class FlatContainerIdToContainerMapperBase<T> where T : class, IContainer
   {
      protected readonly IObjectBaseFactory _objectBaseFactory;
      protected readonly IFlatContainerRepository _flatContainerRepository;
      protected readonly IFlatContainerTagRepository _flatContainerTagRepository;
      protected FlatContainer FlatContainer { get; private set; }

      protected FlatContainerIdToContainerMapperBase(IObjectBaseFactory objectBaseFactory,
         IFlatContainerRepository flatContainerRepository,
         IFlatContainerTagRepository flatContainerTagRepository)
      {
         _objectBaseFactory = objectBaseFactory;
         _flatContainerRepository = flatContainerRepository;
         _flatContainerTagRepository = flatContainerTagRepository;
      }

      public T MapCommonPropertiesFrom(FlatContainerId flatContainerId)
      {
         FlatContainer = _flatContainerRepository.ContainerFrom(flatContainerId.Id);

         var container = _objectBaseFactory.Create<T>().WithName(FlatContainer.Name);

         addTagsToContainer(container, flatContainerId.Id);

         container.Mode = FlatContainer.IsLogical ? ContainerMode.Logical : ContainerMode.Physical;

         return container;
      }

      private void addTagsToContainer(T container, int id)
      {
         var containerTags = from flatTag in _flatContainerTagRepository.All()
            where flatTag.Id == id
            select flatTag.Tag;

         containerTags.Each(container.AddTag);
      }
   }
}