using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Queries
{
   public class ModelNeighborhoodQuery : IModelNeighborhoodQuery
   {
      private readonly IFlatModelContainerRepository _modelContainerRepository;
      private readonly INeighborhoodBuilderFactory _neighborhoodBuilderFactory;

      public ModelNeighborhoodQuery(IFlatModelContainerRepository modelContainerRepository, INeighborhoodBuilderFactory neighborhoodBuilderFactory)
      {
         _modelContainerRepository = modelContainerRepository;
         _neighborhoodBuilderFactory = neighborhoodBuilderFactory;
      }

      public IEnumerable<NeighborhoodBuilder> NeighborhoodsFor(IContainer individualNeighborhoods, ModelProperties modelProperties)
      {
         //ToList is important here as we do not want to always return the value from the database if the query is called in a linq expression
         return modelNeighborhoodsFor(modelProperties.ModelConfiguration.ModelName, individualNeighborhoods)
            .Select(mapFrom).ToList();
      }

      private NeighborhoodBuilder mapFrom(IContainer neighborhood)
      {
         var neighborhoodBuilder = _neighborhoodBuilderFactory.Create().WithName(neighborhood.Name);

         neighborhood.Tags.Each(tag => neighborhoodBuilder.AddTag(tag.Value));

         return neighborhoodBuilder;
      }

      private IEnumerable<IContainer> modelNeighborhoodsFor(string modelName, IContainer individualNeighborhoods)
      {
         var neighborhoodNames = new List<string>();

         IEnumerable<FlatModelContainer> allNeighborhoodContainersForModel =
            from flatModelContainer in _modelContainerRepository.All()
            where flatModelContainer.Model == modelName
            where flatModelContainer.Type == CoreConstants.ContainerType.NEIGHBORHOOD
            select flatModelContainer;

         foreach (FlatModelContainer flatNeighborhoodContainer in allNeighborhoodContainersForModel)
         {
            if (individualNeighborhoods.ContainsName(flatNeighborhoodContainer.Name))
            {
               //neighborhood available in individuum (species)
               neighborhoodNames.Add(flatNeighborhoodContainer.Name);
               continue;
            }

            //neighborhood available in model but not in individual (species)
            //Reaction depends on usage in individual flag

            if (flatNeighborhoodContainer.UsageInIndividual == CoreConstants.ORM.USAGE_IN_INDIVIDUAL_REQUIRED)
               throw new ArgumentException(PKSimConstants.Error.ModelContainerNotAvailable(flatNeighborhoodContainer.Name));

            if (flatNeighborhoodContainer.UsageInIndividual == CoreConstants.ORM.USAGE_IN_INDIVIDUAL_OPTIONAL)
               continue;

            if (flatNeighborhoodContainer.UsageInIndividual == CoreConstants.ORM.USAGE_IN_INDIVIDUAL_EXTENDED)
            {
               throw new ArgumentException(PKSimConstants.Error.ExtendedNeighborhoodNotAllowed(flatNeighborhoodContainer.Name));
            }

            throw new ArgumentException(PKSimConstants.Error.UnknownUsageInIndividualFlag(flatNeighborhoodContainer.UsageInIndividual));
         }

         return individualNeighborhoods.GetChildren<IContainer>(c => neighborhoodNames.Contains(c.Name));
      }
   }
}