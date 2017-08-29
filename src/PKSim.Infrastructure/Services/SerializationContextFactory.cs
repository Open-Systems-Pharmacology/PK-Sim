using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Converter.v5_2;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Infrastructure.Services
{
   public interface ISerializationContextFactory
   {
      SerializationContext Create(IEnumerable<DataRepository> dataRepositories = null, IEnumerable<IWithId> externalReferences = null);
   }

   public class SerializationContextFactory : ISerializationContextFactory
   {
      private readonly ISerializationDimensionFactory _dimensionFactory;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IContainer _container;
      private readonly ICloneManagerForModel _cloneManagerForModel;

      public SerializationContextFactory(
         ISerializationDimensionFactory dimensionFactory, 
         IObjectBaseFactory objectBaseFactory, 
         IContainer container,
         ICloneManagerForModel cloneManagerForModel)
      {
         _dimensionFactory = dimensionFactory;
         _objectBaseFactory = objectBaseFactory;
         _container = container;
         _cloneManagerForModel = cloneManagerForModel;
      }

      public SerializationContext Create(IEnumerable<DataRepository> dataRepositories = null, IEnumerable<IWithId> externalReferences = null)
      {
         var projectRetriever = _container.Resolve<IPKSimProjectRetriever>();
         var project = projectRetriever.Current;

         //do not use the pksim repository since we need to register the deserialized object afterwards
         //this repository is only used to resolve the references
         var withIdRepository = new WithIdRepository();
         externalReferences?.Each(withIdRepository.Register);

         var allRepositories = new List<DataRepository>();
         if (project != null)
         {
            allRepositories.AddRange(project.All<IndividualSimulation>()
               .Where(s => s.HasResults)
               .Select(s => s.DataRepository)
               .Union(project.AllObservedData));
         }

         if (dataRepositories != null)
            allRepositories.AddRange(dataRepositories);

         allRepositories.Each(withIdRepository.Register);

         return SerializationTransaction.Create(_dimensionFactory, _objectBaseFactory, withIdRepository, _cloneManagerForModel, allRepositories);
      }
   }
}