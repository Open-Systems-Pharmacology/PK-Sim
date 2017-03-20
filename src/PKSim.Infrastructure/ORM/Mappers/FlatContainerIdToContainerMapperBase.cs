using System.Linq;
using OSPSuite.Utility.Container;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Descriptors;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;
using IContainer = OSPSuite.Core.Domain.IContainer;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public abstract class FlatContainerIdToContainerMapperBase<T> where T:IContainer 
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IFlatContainerTagRepository _flatContainerTagRepo;

      protected FlatContainer FlatContainer { get; private set; }

      protected FlatContainerIdToContainerMapperBase() 
         :this(IoC.Resolve<IObjectBaseFactory>(), 
               IoC.Resolve<IFlatContainerRepository>(),
               IoC.Resolve <IFlatContainerTagRepository>())
      {}

      protected FlatContainerIdToContainerMapperBase(IObjectBaseFactory objectBaseFactory, 
                                                  IFlatContainerRepository flatContainerRepository,
                                                  IFlatContainerTagRepository flatContainerTagRepo)
      {
         _objectBaseFactory = objectBaseFactory;
         _flatContainerRepository = flatContainerRepository;
         _flatContainerTagRepo = flatContainerTagRepo;
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
         var containerTags = from flatTag in _flatContainerTagRepo.All()
                             where flatTag.Id == id
                             select flatTag.Tag;

         foreach(var tag in containerTags)
         {
            container.AddTag(new Tag(tag));
         }
      }
   }
}
