using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Services;

namespace PKSim.Core.Model
{
   public interface IPKSimSpatialStructureFactory
   {
      /// <summary>
      ///    Create the spatial structure based on the individual and model properties
      /// </summary>
      /// <param name="individual">individual building block</param>
      /// <param name="simulation">Simulation</param>
      SpatialStructure CreateFor(Individual individual, Simulation simulation);
   }

   public class PKSimSpatialStructureFactory : SpatialStructureFactory, IPKSimSpatialStructureFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IModelContainerQuery _modelContainerQuery;
      private readonly IModelNeighborhoodQuery _modelNeighborhoodQuery;
      private readonly INeighborhoodFinalizer _neighborhoodFinalizer;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IParameterDefaultStateUpdater _parameterDefaultStateUpdater;

      public PKSimSpatialStructureFactory(
         IObjectBaseFactory objectBaseFactory,
         IParameterContainerTask parameterContainerTask,
         IModelContainerQuery modelContainerQuery,
         IModelNeighborhoodQuery modelNeighborhoodQuery,
         INeighborhoodFinalizer neighborhoodFinalizer,
         IEntityPathResolver entityPathResolver,
         IParameterDefaultStateUpdater parameterDefaultStateUpdater) : base(objectBaseFactory)

      {
         _objectBaseFactory = objectBaseFactory;
         _parameterContainerTask = parameterContainerTask;
         _modelContainerQuery = modelContainerQuery;
         _modelNeighborhoodQuery = modelNeighborhoodQuery;
         _neighborhoodFinalizer = neighborhoodFinalizer;
         _entityPathResolver = entityPathResolver;
         _parameterDefaultStateUpdater = parameterDefaultStateUpdater;
      }

      public SpatialStructure CreateFor(Individual individual, Simulation simulation)
      {
         var spatialStructure = Create();
         var organism = _objectBaseFactory.Create<Organism>();
         spatialStructure.AddTopContainer(organism);

         var eventContainer = _objectBaseFactory.Create<IContainer>()
            .WithName(Constants.EVENTS)
            .WithMode(ContainerMode.Logical);

         spatialStructure.AddTopContainer(eventContainer);

         //FIRST: Global molecule parameters
         addModelStructureTo(spatialStructure.GlobalMoleculeDependentProperties, individual.OriginData, simulation.ModelProperties, spatialStructure.FormulaCache);

         //SECOND: Then, individual structure
         addModelStructureTo(organism, individual.OriginData, simulation.ModelProperties, spatialStructure.FormulaCache);
         addNeighborhoods(spatialStructure, organism, individual, simulation.ModelProperties, spatialStructure.FormulaCache);

         //THIRD: update parameter values and ids
         updateParameterFromIndividual(spatialStructure, individual);

         _parameterDefaultStateUpdater.UpdateDefaultFor(spatialStructure);

         return spatialStructure;
      }

      private void updateParameterFromIndividual(SpatialStructure spatialStructure, Individual individual)
      {
         //Update parameter values for parameter that have been changed in individual
         var allIndividualParameters = new PathCache<IParameter>(_entityPathResolver).For(individual.GetAllChildren<IParameter>());
         var allContainerParameters = new PathCache<IParameter>(_entityPathResolver).For(spatialStructure.TopContainers.SelectMany(x => x.GetAllChildren<IParameter>()));
         var allNeighborhoodParameters = new PathCache<IParameter>(_entityPathResolver).For(spatialStructure.Neighborhoods.SelectMany(x => x.GetAllChildren<IParameter>()));

         copyParameterTags(allIndividualParameters, allContainerParameters);
         copyParameterTags(allIndividualParameters, allNeighborhoodParameters);
      }

      private void copyParameterTags(PathCache<IParameter> sourceParameters, PathCache<IParameter> targetParameters)
      {
         foreach (var sourceParameter in sourceParameters.KeyValues)
         {
            var targetParameter = targetParameters[sourceParameter.Key];
            if (targetParameter == null) continue;

            sourceParameter.Value.ParentContainer.Tags.Each(t => targetParameter.AddTag(t.Value));
         }
      }

      private void addNeighborhoods(SpatialStructure spatialStructure, Organism organism, Individual individual, ModelProperties modelProperties, IFormulaCache formulaCache)
      {
         var neighborhoodList = _modelNeighborhoodQuery.NeighborhoodsFor(individual.Neighborhoods, modelProperties).ToList();

         foreach (var neighborhood in neighborhoodList)
         {
            addNeighborhood(neighborhood, spatialStructure, individual.OriginData, modelProperties, formulaCache);
         }

         _neighborhoodFinalizer.SetNeighborsIn(organism, neighborhoodList);
      }

      private void addNeighborhood(NeighborhoodBuilder neighborhood, SpatialStructure spatialStructure, OriginData originData, ModelProperties modelProperties, IFormulaCache formulaCache)
      {
         spatialStructure.AddNeighborhood(neighborhood);
         _parameterContainerTask.AddParametersToSpatialStructureContainer(neighborhood, originData, modelProperties, formulaCache);
         _parameterContainerTask.AddParametersToSpatialStructureContainer(neighborhood.MoleculeProperties, originData, modelProperties, formulaCache);
      }

      private void addModelStructureTo(IContainer container, OriginData originData, ModelProperties modelProperties, IFormulaCache formulaCache)
      {
         _parameterContainerTask.AddParametersToSpatialStructureContainer(container, originData, modelProperties, formulaCache);

         foreach (var subContainer in _modelContainerQuery.SubContainersFor(originData.Population, modelProperties.ModelConfiguration, container))
         {
            container.Add(subContainer);
            addModelStructureTo(subContainer, originData, modelProperties, formulaCache);
         }
      }

      protected override SpatialStructure CreateSpatialStructure() => _objectBaseFactory.Create<PKSimSpatialStructure>().WithName(DefaultNames.SpatialStructure);
   }
}