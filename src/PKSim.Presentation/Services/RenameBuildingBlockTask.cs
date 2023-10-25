using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using static OSPSuite.Core.Domain.Constants.ContainerName;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Presentation.Services
{
   public class RenameBuildingBlockTask : IRenameBuildingBlockTask
   {
      private readonly IBuildingBlockTask _buildingBlockTask;
      private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;
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
      private readonly IExpressionProfileUpdater _expressionProfileUpdater;

      public RenameBuildingBlockTask(
         IBuildingBlockTask buildingBlockTask,
         IBuildingBlockInProjectManager buildingBlockInProjectManager,
         IApplicationController applicationController,
         ILazyLoadTask lazyLoadTask, IContainerTask containerTask,
         IHeavyWorkManager heavyWorkManager,
         IRenameAbsolutePathVisitor renameAbsolutePathVisitor,
         IObjectReferencingRetriever objectReferencingRetriever,
         IProjectRetriever projectRetriever,
         IParameterIdentificationSimulationPathUpdater simulationPathUpdater,
         IDataRepositoryNamer dataRepositoryNamer,
         ICurveNamer curveNamer,
         IExpressionProfileUpdater expressionProfileUpdater)
      {
         _buildingBlockTask = buildingBlockTask;
         _buildingBlockInProjectManager = buildingBlockInProjectManager;
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
         _expressionProfileUpdater = expressionProfileUpdater;
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

         _buildingBlockInProjectManager.UpdateBuildingBlockNamesUsedIn(simulation);

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
         _curveNamer.RenameCurvesWithOriginalNames(individualSimulation, () => renameIndividualSimulation(individualSimulation, newName), addSimulationName: true);
      }

      private void renameIndividualSimulation(IndividualSimulation individualSimulation, string newName)
      {
         var individualSimulationDataRepository = individualSimulation.DataRepository;
         if (individualSimulationDataRepository != null)
            _dataRepositoryNamer.Rename(individualSimulationDataRepository, newName);

         renameSimulation(individualSimulation, newName);
      }

      public void RenameBuildingBlock(IPKSimBuildingBlock templateBuildingBlock, string newName)
      {
         //Perform rename of molecule in expression profile first as it relies on the old name still being present
         renameMoleculeNameInExpressionProfile(templateBuildingBlock, newName);
         renameUsageOfBuildingBlockInObservedData(templateBuildingBlock, newName);

         //update the name
         templateBuildingBlock.Name = newName;

         //Update dependencies
         renameUsageOfBuildingBlockInSimulations(templateBuildingBlock);
      }

      private void renameMoleculeNameInExpressionProfile(IPKSimBuildingBlock templateBuildingBlock, string newExpressionProfileName)
      {
         var expressionProfile = templateBuildingBlock as ExpressionProfile;
         if (expressionProfile == null)
            return;

         var (newMoleculeName, _, _) = NamesFromExpressionProfileName(newExpressionProfileName);
         _expressionProfileUpdater.UpdateMoleculeName(expressionProfile, newMoleculeName);
      }

      private void renameUsageOfBuildingBlockInObservedData(IPKSimBuildingBlock templateBuildingBlock, string newName)
      {
         var compound = templateBuildingBlock as Compound;
         if (compound == null) return;

         _projectRetriever.CurrentProject.AllObservedData.Each(x => renameMoleculeNameIn(x, compound.Name, newName));
      }

      private void renameMoleculeNameIn(DataRepository observedData, string oldBuildingBlockName, string newBuildingBlockName)
      {
         if (!string.Equals(observedData.ExtendedPropertyValueFor(Constants.ObservedData.MOLECULE), oldBuildingBlockName))
            return;

         observedData.ExtendedProperties[Constants.ObservedData.MOLECULE].ValueAsObject = newBuildingBlockName;
      }

      private void renameUsageOfBuildingBlockInSimulations(IPKSimBuildingBlock templateBuildingBlock)
      {
         var allSimulationUsingBuildingBlocks = _buildingBlockInProjectManager.SimulationsUsing(templateBuildingBlock).ToList();

         //only starts heavy-work manager if one simulation is not loaded
         if (allSimulationUsingBuildingBlocks.Any(x => !x.IsLoaded))
            _heavyWorkManager.Start(() => renameBuildingBlockInSimulation(allSimulationUsingBuildingBlocks, templateBuildingBlock), PKSimConstants.Information.RenamingBuildingBlock(templateBuildingBlock.BuildingBlockType.ToString()));
         else
            renameBuildingBlockInSimulation(allSimulationUsingBuildingBlocks, templateBuildingBlock);

         //needs to be done out of the heavy work manager to avoid cross threading issues
         allSimulationUsingBuildingBlocks.Each(s => _buildingBlockInProjectManager.UpdateBuildingBlockNamesUsedIn(s));
      }

      private void renameBuildingBlockInSimulation(IEnumerable<Simulation> allSimulationUsingBuildingBlocks, IPKSimBuildingBlock templateBuildingBlock)
      {
         if (!buildingBlockIsDefinedAsContainerInSimulation(templateBuildingBlock))
            return;

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
            renameFormulaPathInFormulas(allObjectReferencing, reference, renamedContainer.Name, oldContainerName);
         });
      }

      private void renameFormulaPathInFormulas(IReadOnlyCollection<IUsingFormula> referencingFormulas, IFormulaUsable reference, string newName, string oldName)
      {
         foreach (var usingFormula in referencingFormulas)
         {
            var objectPath = formulaUsablePathReferencing(usingFormula, reference);

            //Reference is always found by construction and this should never happen
            if (objectPath == null)
               return;

            //The reference is used. The path needs to be updated only if it is a path referencing the old name
            if (!objectPath.Contains(oldName))
               continue;

            objectPath.ReplaceWith(reference.EntityPath().ToPathArray());
         }
      }

      private FormulaUsablePath formulaUsablePathReferencing(IUsingFormula usingFormula, IFormulaUsable reference)
      {
         var parameter = usingFormula as IParameter;
         return formulaUsablePathReferencing(usingFormula.Formula, reference) ??
                formulaUsablePathReferencing(parameter?.RHSFormula, reference);
      }

      private FormulaUsablePath formulaUsablePathReferencing(IFormula formula, IFormulaUsable reference)
      {
         if (formula == null)
            return null;

         var referenceIndex = formula.ObjectReferences.Select(x => x.Object).IndexOf(reference);
         return (referenceIndex >= 0) ? formula.ObjectPaths[referenceIndex] : null;
      }

      private IEnumerable<IContainer> getContainersToRename(Simulation simulation, IPKSimBuildingBlock templateBuildingBlock, string oldContainerName)
      {
         var containersToRename = new List<IContainer>();

         var rootContainer = simulation.Model.Root.EntityAt<IContainer>(getContainerPathToRename(templateBuildingBlock.BuildingBlockType));
         if (templateBuildingBlock.BuildingBlockType != PKSimBuildingBlockType.Formulation)
            containersToRename.Add(rootContainer.Container(oldContainerName));
         else
         {
            rootContainer.GetAllChildren<IContainer>(x => x.IsNamed(oldContainerName) && x.ContainerType == ContainerType.Formulation).Each(formulationContainer =>
               containersToRename.Add(formulationContainer));
         }

         return containersToRename.Where(x => x != null);
      }

      private string getContainerPathToRename(PKSimBuildingBlockType buildingBlockType)
      {
         switch (buildingBlockType)
         {
            case PKSimBuildingBlockType.Formulation:
            case PKSimBuildingBlockType.Protocol:
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