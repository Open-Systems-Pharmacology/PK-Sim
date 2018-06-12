using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Assets;
using OSPSuite.Core.Services;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Presentation.Services
{
   public class RenameBuildingBlockTask : IRenameBuildingBlockTask
   {
      private readonly IBuildingBlockTask _buildingBlockTask;
      private readonly IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;
      private readonly IApplicationController _applicationController;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IContainerTask _containerTask;
      private readonly IHeavyWorkManager _heavyWorkManager;
      private readonly IRenameAbsolutePathVisitor _renameAbsolutePathVisitor;
      private readonly IObjectReferencingRetriever _objectReferencingRetriever;
      private readonly IProjectRetriever _projectRetriever;
      private readonly IParameterIdentificationSimulationPathUpdater _simulationPathUpdater;
      private readonly IDataRepositoryNamer _dataRepositoryNamer;
      private readonly ICurveNamer _curveNamer;

      public RenameBuildingBlockTask(IBuildingBlockTask buildingBlockTask, IBuildingBlockInSimulationManager buildingBlockInSimulationManager,
         IApplicationController applicationController, ILazyLoadTask lazyLoadTask, IContainerTask containerTask,
         IHeavyWorkManager heavyWorkManager, IRenameAbsolutePathVisitor renameAbsolutePathVisitor, IObjectReferencingRetriever objectReferencingRetriever,
         IProjectRetriever projectRetriever, IParameterIdentificationSimulationPathUpdater simulationPathUpdater, IDataRepositoryNamer dataRepositoryNamer, ICurveNamer curveNamer)
      {
         _buildingBlockTask = buildingBlockTask;
         _buildingBlockInSimulationManager = buildingBlockInSimulationManager;
         _applicationController = applicationController;
         _lazyLoadTask = lazyLoadTask;
         _containerTask = containerTask;
         _heavyWorkManager = heavyWorkManager;
         _renameAbsolutePathVisitor = renameAbsolutePathVisitor;
         _objectReferencingRetriever = objectReferencingRetriever;
         _projectRetriever = projectRetriever;
         _simulationPathUpdater = simulationPathUpdater;
         _dataRepositoryNamer = dataRepositoryNamer;
         _curveNamer = curveNamer;
      }

      public void RenameSimulation(Simulation simulation, string newName)
      {
         _buildingBlockTask.Load(simulation);

         //rename model, all object path and results
         //was the presenter open for the simulation? if yes 
         var presenterWasOpen = _applicationController.HasPresenterOpenedFor(simulation);
         _applicationController.Close(simulation);

         //Model and root
         string oldName = simulation.Name;

         var individualSimulation = simulation as IndividualSimulation;
         if (individualSimulation != null)
            renameForIndividualSimulation(individualSimulation, newName);

         else
            renameSimulation(simulation, newName);

         _renameAbsolutePathVisitor.RenameAllAbsolutePathIn(simulation, oldName);

         _buildingBlockInSimulationManager.UpdateBuildingBlockNamesUsedIn(simulation);

         _simulationPathUpdater.UpdatePathsForRenamedSimulation(simulation, oldName, newName);

         //presenter was not open, nothing to do
         if (!presenterWasOpen)
            return;

         //edit the simulation back, since it was edited
         _buildingBlockTask.Edit(simulation);
      }

      private static void renameSimulation(Simulation simulation, string newName)
      {
         simulation.Name = newName;
      }

      private void renameForIndividualSimulation(IndividualSimulation individualSimulation, string newName)
      {
         _curveNamer.RenameCurvesWithOriginalNames(individualSimulation, () => renameIndividualSimulation(individualSimulation, newName), addSimulationName:true);
      }

      private void renameIndividualSimulation(IndividualSimulation individualSimulation, string newName)
      {
         var individualSimulationDataRepository = individualSimulation.DataRepository;
         if (individualSimulationDataRepository != null)
            _dataRepositoryNamer.Rename(individualSimulationDataRepository, newName);

         renameSimulation(individualSimulation, newName);
      }

      private bool pathContains(List<string> path, string oldCompoundName)
      {
         if (!path.Any()) return false;
         return path.LastIndexOf(oldCompoundName) > 0;
      }

      public void SynchronizeCompoundNameIn(Simulation targetSimulation, string oldCompoundName, string newCompoundName)
      {
         //the cache will be referencing quantity by path using the newCompoundName
         var quantityCache = _containerTask.CacheAllChildren<IQuantity>(targetSimulation.Model.Root);

         //all results with a possible entry equal to the old compound name
         var allQuantityResults = targetSimulation.Results
            .SelectMany(x => x.AllValues)
            .Where(x => pathContains(x.PathList.ToList(), oldCompoundName));

         foreach (var quantityValues in allQuantityResults)
         {
            //check if the quantity exists for the give path
            var newPath = new List<string>(quantityValues.PathList);
            //uses last so that we do not rename simulation name that could be the same as the compound name
            newPath[newPath.LastIndexOf(oldCompoundName)] = newCompoundName;

            var quantity = quantityCache[newPath.ToPathString()];
            if (!quantityIsCompound(quantity))
               continue;

            quantityValues.PathList = newPath;
         }
      }

      private static bool quantityIsCompound(IQuantity quantity)
      {
         return quantity != null && quantity.QuantityType.Is(QuantityType.Drug);
      }

      public void RenameUsageOfBuildingBlockInProject(IPKSimBuildingBlock templateBuildingBlock, string oldBuildingBlockName)
      {
         renameUsageOfBuildingBlockInSimulations(templateBuildingBlock);
         renameUsageOfBuildingBlockInObservedData(templateBuildingBlock, oldBuildingBlockName);
      }

      private void renameUsageOfBuildingBlockInObservedData(IPKSimBuildingBlock templateBuildingBlock, string oldBuildingBlockName)
      {
         var compound = templateBuildingBlock as Compound;
         if (compound == null) return;

         _projectRetriever.CurrentProject.AllObservedData.Each(x => renameMoleculeNameIn(x, oldBuildingBlockName, compound.Name));
      }

      private void renameMoleculeNameIn(DataRepository observedData, string oldBuildingBlockName, string newBuildingBlockName)
      {
         if (!string.Equals(observedData.ExtendedPropertyValueFor(ObservedData.MOLECULE), oldBuildingBlockName))
            return;

         observedData.ExtendedProperties[ObservedData.MOLECULE].ValueAsObject = newBuildingBlockName;
      }

      private void renameUsageOfBuildingBlockInSimulations(IPKSimBuildingBlock templateBuildingBlock)
      {
         var allSimulationUsingBuildingBlocks = _buildingBlockInSimulationManager.SimulationsUsing(templateBuildingBlock).ToList();

         //only starts heavywork manager if one simulation is not loaded
         if (allSimulationUsingBuildingBlocks.Any(x => !x.IsLoaded))
            _heavyWorkManager.Start(() => renameBuildingBlockInSimulation(allSimulationUsingBuildingBlocks, templateBuildingBlock), PKSimConstants.Information.RenamingBuildingBlock(templateBuildingBlock.BuildingBlockType.ToString()));
         else
            renameBuildingBlockInSimulation(allSimulationUsingBuildingBlocks, templateBuildingBlock);

         //needs to be done out of the heavy work manager to avoid cross threading issues
         allSimulationUsingBuildingBlocks.Each(s => _buildingBlockInSimulationManager.UpdateBuildingBlockNamesUsedIn(s));
      }

      private void renameBuildingBlockInSimulation(IEnumerable<Simulation> allSimulationUsingBuildingBlocks, IPKSimBuildingBlock templateBuildingBlock)
      {
         if (!buildingBlockIsDefinedAsContainerInSimulation(templateBuildingBlock)) return;
         allSimulationUsingBuildingBlocks.Each(s => renameContainerBuildingBlockInSimulation(s, templateBuildingBlock));
      }

      private void renameContainerBuildingBlockInSimulation(Simulation simulation, IPKSimBuildingBlock templateBuildingBlock)
      {
         _lazyLoadTask.Load(simulation);
         var usedBuildingBlock = simulation.UsedBuildingBlockByTemplateId(templateBuildingBlock.Id);

         foreach (var containerToRename in getContainersToRename(simulation, templateBuildingBlock, usedBuildingBlock.BuildingBlock.Name))
         {
            containerToRename.Name = _containerTask.CreateUniqueName(containerToRename.ParentContainer, templateBuildingBlock.Name, canUseBaseName: true);

            //now some parameters in the simulation might reference parameters defined in the container that was renamed. We need to update
            //the formula paths of these parameters
            renameFormulaPathReferencingContainerInSimulation(simulation, containerToRename, usedBuildingBlock.BuildingBlock.Name);

            //make sure we mark the simulation has changed so that it will be saved
            simulation.HasChanged = true;
         }
      }

      private void renameFormulaPathReferencingContainerInSimulation(Simulation simulation, IContainer renamedContainer, string oldContainerName)
      {
         renamedContainer.GetAllChildren<IFormulaUsable>().Each(reference =>
         {
            var allObjectReferencing = _objectReferencingRetriever.AllUsingFormulaReferencing(reference, simulation.Model);
            renameFormulaPathInFormulas(allObjectReferencing, renamedContainer.Name, oldContainerName);
         });
      }

      private void renameFormulaPathInFormulas(IEnumerable<IUsingFormula> referencingFormulas, string newName, string oldName)
      {
         referencingFormulas.Select(x => x.Formula).SelectMany(x => x.ObjectPaths)
            .Each(objecPath => objecPath.Replace(oldName, newName));
      }

      private IEnumerable<IContainer> getContainersToRename(Simulation simulation, IPKSimBuildingBlock templateBuildingBlock, string oldContainerName)
      {
         var rootContainer = simulation.Model.Root.EntityAt<IContainer>(getContainerPathToRename(templateBuildingBlock.BuildingBlockType));
         if (templateBuildingBlock.BuildingBlockType != PKSimBuildingBlockType.Formulation)
            yield return rootContainer.Container(oldContainerName);
         else
         {
            foreach (var formulationContainer in rootContainer.GetAllChildren<IContainer>(x => x.IsNamed(oldContainerName) && x.ContainerType == ContainerType.Formulation))
            {
               yield return formulationContainer;
            }
         }
      }

      private string getContainerPathToRename(PKSimBuildingBlockType buildingBlockType)
      {
         switch (buildingBlockType)
         {
            case PKSimBuildingBlockType.Formulation:
            case PKSimBuildingBlockType.Protocol:
               return Constants.APPLICATIONS;
            case PKSimBuildingBlockType.Event:
               return Constants.EVENTS;
            default:
               throw new ArgumentOutOfRangeException(nameof(buildingBlockType));
         }
      }

      private bool buildingBlockIsDefinedAsContainerInSimulation(IPKSimBuildingBlock buildingBlock)
      {
         return buildingBlock.BuildingBlockType.Is(PKSimBuildingBlockType.Formulation | PKSimBuildingBlockType.Event | PKSimBuildingBlockType.Protocol);
      }
   }
}