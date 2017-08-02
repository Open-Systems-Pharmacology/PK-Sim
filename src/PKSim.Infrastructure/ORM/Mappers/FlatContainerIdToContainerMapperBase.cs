using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;
using IContainer = OSPSuite.Core.Domain.IContainer;

namespace PKSim.Infrastructure.ORM.Mappers
{
    public abstract class FlatContainerIdToContainerMapperBase<T> where T : IContainer
    {
        private readonly IObjectBaseFactory _objectBaseFactory;
        private readonly IFlatContainerRepository _flatContainerRepository;
        private readonly IFlatContainerTagRepository _flatContainerTagRepository;

        protected FlatContainer FlatContainer { get; private set; }

        protected FlatContainerIdToContainerMapperBase()
            : this(IoC.Resolve<IObjectBaseFactory>(),
                IoC.Resolve<IFlatContainerRepository>(),
                IoC.Resolve<IFlatContainerTagRepository>())
        {
        }

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