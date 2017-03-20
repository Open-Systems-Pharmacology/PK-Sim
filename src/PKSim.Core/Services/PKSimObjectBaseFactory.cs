using System;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using IoC = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Core.Services
{
   public interface IPKSimObjectBaseFactory : IObjectBaseFactory
   {
      IParameter CreateParameter();
      IDistributedParameter CreateDistributedParameter();
   }

   public class PKSimObjectBaseFactory : ObjectBaseFactory, IPKSimObjectBaseFactory
   {
      public PKSimObjectBaseFactory(IoC container, IDimensionRepository dimensionRepository, IIdGenerator idGenerator, ICreationMetaDataFactory creationMetaDataFactory)
         : base(container, dimensionRepository.DimensionFactory, idGenerator,creationMetaDataFactory)
      {
      }

      public override T CreateObjectBaseFrom<T>(Type objectType, string id)
      {
         if (objectType.IsAnImplementationOf<DistributedParameter>())
            return createParameter<PKSimDistributedParameter, T>(id);

         if (objectType.IsAnImplementationOf<Parameter>())
            return createParameter<PKSimParameter, T>(id);

         return base.CreateObjectBaseFrom<T>(objectType, id);
      }

      public IParameter CreateParameter()
      {
         return createParameter<PKSimParameter, IParameter>(GetId());
      }

      public IDistributedParameter CreateDistributedParameter()
      {
         return createParameter<PKSimDistributedParameter, IDistributedParameter>(GetId());
      }

      private TInterfaceParameter createParameter<TConcreteParameterType, TInterfaceParameter>(string id)
         where TConcreteParameterType : IObjectBase, new()
      {
         return new TConcreteParameterType().WithId(id).DowncastTo<TInterfaceParameter>();
      }
   }
}