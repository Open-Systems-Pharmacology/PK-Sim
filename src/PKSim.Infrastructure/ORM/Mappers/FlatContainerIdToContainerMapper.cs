using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatContainerIdToContainerMapper : IMapper<FlatContainerId, IContainer>
   {
   }

   public interface IFlatContainerIdToContainerMapperSpecification : IFlatContainerIdToContainerMapper, ISpecification<PKSimContainerType>
   {
   }

   public class FlatContainerIdToContainerMapper : IFlatContainerIdToContainerMapper
   {
      private readonly IContainerStringToContainerTypeMapper _containerTypeMapper;
      private readonly IList<IFlatContainerIdToContainerMapperSpecification> _allMappers;

      public FlatContainerIdToContainerMapper(IContainerStringToContainerTypeMapper containerTypeMapper,
         IRepository<IFlatContainerIdToContainerMapperSpecification> containerMapperRepository)
      {
         _containerTypeMapper = containerTypeMapper;
         _allMappers = containerMapperRepository.All().ToList();
      }

      public IContainer MapFrom(FlatContainerId flatContainerId)
      {
         var containerType = _containerTypeMapper.MapFrom(flatContainerId.Type);
         foreach (var containerMapper in _allMappers)
         {
            if (containerMapper.IsSatisfiedBy(containerType))
               return containerMapper.MapFrom(flatContainerId);
         }

         throw new ArgumentException(PKSimConstants.Error.CannotCreateContainerOfType(containerType.ToString()));
      }
   }
}