using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Infrastructure.Services
{
   public interface ISerializationContextFactory
   {
      /// <summary>
      /// Returns a new <see cref="SerializationContext"/> to be used in serialization or deserialization
      /// </summary>
      /// <param name="dataRepositories">Optional dataRepositories that will be available when deserializing an object. This is required to resolve references</param>
      /// <param name="externalReferences">Optional references that will be available when deserializing an object. This is required to resolve references to those objects</param>
      /// <param name="addProjectSimulations">If a project is defined, should references to simulations defined in the project be added to the contect? Default is <c>true</c></param>
      /// <param name="addProjectObservedData">If a project is defined, should references to observed data defined in the project be added to the contect? Default is <c>true</c></param>
      /// <returns></returns>
      SerializationContext Create(IEnumerable<DataRepository> dataRepositories = null, IEnumerable<IWithId> externalReferences = null, bool addProjectSimulations = true, bool addProjectObservedData = true);
   }

   public class SerializationContextFactory : ISerializationContextFactory
   {
      private readonly IDimensionFactory _dimensionFactory;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IContainer _container;
      private readonly ICloneManagerForModel _cloneManagerForModel;

      public SerializationContextFactory(
         IDimensionFactory dimensionFactory,
         IObjectBaseFactory objectBaseFactory,
         IContainer container,
         ICloneManagerForModel cloneManagerForModel)
      {
         _dimensionFactory = dimensionFactory;
         _objectBaseFactory = objectBaseFactory;
         _container = container;
         _cloneManagerForModel = cloneManagerForModel;
      }

      public SerializationContext Create(IEnumerable<DataRepository> dataRepositories = null, IEnumerable<IWithId> externalReferences = null, bool addProjectSimulations = true, bool addProjectObservedData = true)
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
            if(addProjectObservedData)
               allRepositories.AddRange(project.AllDataRepositories());

            if(addProjectSimulations)
               project.All<ISimulation>().Each(withIdRepository.Register);
         }

         if (dataRepositories != null)
            allRepositories.AddRange(dataRepositories);

         allRepositories.Each(withIdRepository.Register);

         return SerializationTransaction.Create(_dimensionFactory, _objectBaseFactory, withIdRepository, _cloneManagerForModel, allRepositories);
      }
   }
}