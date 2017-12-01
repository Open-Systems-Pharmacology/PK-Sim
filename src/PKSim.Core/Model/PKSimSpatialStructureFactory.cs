using System.Linq;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public interface IPKSimSpatialStructureFactory
   {
      /// <summary>
      ///    Create the spatial structure based on the individual and model properties
      /// </summary>
      /// <param name="individual">individual building block</param>
      /// <param name="simulation">Simulation</param>
      ISpatialStructure CreateFor(Individual individual, Simulation simulation);
   }

   public class PKSimSpatialStructureFactory : SpatialStructureFactory, IPKSimSpatialStructureFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly IModelContainerQuery _modelContainerQuery;
      private readonly IModelNeighborhoodQuery _modelNeighborhoodQuery;
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IParameterIdUpdater _parameterIdUpdater;
      private readonly INeighborhoodFinalizer _neighborhoodFinalizer;
      private readonly IEntityPathResolver _entityPathResolver;

      public PKSimSpatialStructureFactory(IObjectBaseFactory objectBaseFactory, IParameterContainerTask parameterContainerTask,
         IModelContainerQuery modelContainerQuery, IModelNeighborhoodQuery modelNeighborhoodQuery,
         IParameterSetUpdater parameterSetUpdater, IParameterIdUpdater parameterIdUpdater,
         INeighborhoodFinalizer neighborhoodFinalizer, IEntityPathResolver entityPathResolver) : base(objectBaseFactory)

      {
         _objectBaseFactory = objectBaseFactory;
         _parameterContainerTask = parameterContainerTask;
         _modelContainerQuery = modelContainerQuery;
         _modelNeighborhoodQuery = modelNeighborhoodQuery;
         _parameterSetUpdater = parameterSetUpdater;
         _parameterIdUpdater = parameterIdUpdater;
         _neighborhoodFinalizer = neighborhoodFinalizer;
         _entityPathResolver = entityPathResolver;
      }

      public ISpatialStructure CreateFor(Individual individual, Simulation simulation)
      {
         var spatialStructure = Create().WithName(simulation.Name);
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

         return spatialStructure;
      }

      private void updateParameterFromIndividual(ISpatialStructure spatialStructure, Individual individual)
      {
         //Update parameter values for parameter that have been changed in individual
         var allIndividualParameter = new PathCache<IParameter>(_entityPathResolver).For(individual.GetAllChildren<IParameter>());
         var allContainerParameters = new PathCache<IParameter>(_entityPathResolver).For(spatialStructure.TopContainers.SelectMany(x => x.GetAllChildren<IParameter>()));
         var allNeighborhoodParameters = new PathCache<IParameter>(_entityPathResolver).For(spatialStructure.Neighborhoods.SelectMany(x => x.GetAllChildren<IParameter>()));

         _parameterSetUpdater.UpdateValues(allIndividualParameter, allContainerParameters);
         _parameterSetUpdater.UpdateValues(allIndividualParameter, allNeighborhoodParameters);
         _parameterIdUpdater.UpdateBuildingBlockId(allContainerParameters, individual);
         _parameterIdUpdater.UpdateBuildingBlockId(allNeighborhoodParameters, individual);

         copyParameterTags(allIndividualParameter, allContainerParameters);
         copyParameterTags(allIndividualParameter, allNeighborhoodParameters);
      }

      private void copyParameterTags(PathCache<IParameter> sourceParameters, PathCache<IParameter> targetParameters)
      {
         foreach (var sourceParameter in sourceParameters.KeyValues)
         {
            var targetParameter = targetParameters[sourceParameter.Key];
            if (targetParameter == null) continue;

            sourceParameter.Value.ParentContainer.Tags.Each(t=>targetParameter.AddTag(t.Value));
         }
      }

      private void addNeighborhoods(ISpatialStructure spatialStructure, Organism organism, Individual individual, ModelProperties modelProperties, IFormulaCache formulaCache)
      {
         var neighborhoodList = _modelNeighborhoodQuery.NeighborhoodsFor(individual.Neighborhoods, modelProperties).ToList();

         foreach (var neighborhood in neighborhoodList)
         {
            addNeighborhood(neighborhood, spatialStructure, individual.OriginData, modelProperties, formulaCache);
         }

         _neighborhoodFinalizer.SetNeighborsIn(organism, neighborhoodList);
      }

      private void addNeighborhood(INeighborhoodBuilder neighborhood, ISpatialStructure spatialStructure, OriginData originData, ModelProperties modelProperties, IFormulaCache formulaCache)
      {
         spatialStructure.AddNeighborhood(neighborhood);
         _parameterContainerTask.AddModelParametersTo(neighborhood, originData, modelProperties, formulaCache);
         _parameterContainerTask.AddModelParametersTo(neighborhood.MoleculeProperties, originData, modelProperties, formulaCache);
      }

      private void addModelStructureTo(IContainer container, OriginData originData, ModelProperties modelProperties, IFormulaCache formulaCache)
      {
         _parameterContainerTask.AddModelParametersTo(container, originData, modelProperties, formulaCache);

         foreach (var subContainer in _modelContainerQuery.SubContainersFor(originData.SpeciesPopulation, modelProperties.ModelConfiguration, container))
         {
            container.Add(subContainer);
            addModelStructureTo(subContainer, originData, modelProperties, formulaCache);
         }
      }

      protected override ISpatialStructure CreateSpatialStructure()
      {
         return _objectBaseFactory.Create<IPKSimSpatialStructure>();
      }
   }
}