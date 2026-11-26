using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using static OSPSuite.Core.Domain.Constants.ContainerName;

namespace PKSim.Core.Services
{
   public class MissingBuildingBlockContentRecoveryResult
   {
      public static readonly MissingBuildingBlockContentRecoveryResult Empty = new MissingBuildingBlockContentRecoveryResult(new List<IPKSimBuildingBlock>(), new List<BuildingBlockWithMissingContent>(), string.Empty);

      /// <summary>
      ///    Building blocks that were rebuilt and added to the project
      /// </summary>
      public IReadOnlyList<IPKSimBuildingBlock> RebuiltBuildingBlocks { get; }

      /// <summary>
      ///    Building blocks that could not be rebuilt and were removed from the project
      /// </summary>
      public IReadOnlyList<BuildingBlockWithMissingContent> RemovedBuildingBlocks { get; }

      /// <summary>
      ///    Message describing the recovery to the user. Empty if nothing was recovered
      /// </summary>
      public string Message { get; }

      public MissingBuildingBlockContentRecoveryResult(IReadOnlyList<IPKSimBuildingBlock> rebuiltBuildingBlocks, IReadOnlyList<BuildingBlockWithMissingContent> removedBuildingBlocks, string message)
      {
         RebuiltBuildingBlocks = rebuiltBuildingBlocks;
         RemovedBuildingBlocks = removedBuildingBlocks;
         Message = message;
      }

      public bool Any => RebuiltBuildingBlocks.Any() || RemovedBuildingBlocks.Any();
   }

   public interface IMissingBuildingBlockContentRecoveryTask
   {
      /// <summary>
      ///    Recovers the building blocks that were skipped while loading <paramref name="project" /> because their content is
      ///    missing from the project file. An expression profile referenced by an individual defining the molecule is rebuilt
      ///    from the individual. Any other building block is dropped. The rebuilt building blocks are flagged as changed so
      ///    that they are persisted with the next save.
      /// </summary>
      MissingBuildingBlockContentRecoveryResult Recover(PKSimProject project);
   }

   public class MissingBuildingBlockContentRecoveryTask : IMissingBuildingBlockContentRecoveryTask
   {
      private readonly IMissingBuildingBlockContentRegistry _registry;
      private readonly IExpressionProfileFactory _expressionProfileFactory;
      private readonly IExpressionProfileUpdater _expressionProfileUpdater;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IRegistrationTask _registrationTask;
      private readonly IEventPublisher _eventPublisher;
      private readonly IObjectTypeResolver _objectTypeResolver;

      public MissingBuildingBlockContentRecoveryTask(
         IMissingBuildingBlockContentRegistry registry,
         IExpressionProfileFactory expressionProfileFactory,
         IExpressionProfileUpdater expressionProfileUpdater,
         ILazyLoadTask lazyLoadTask,
         IRegistrationTask registrationTask,
         IEventPublisher eventPublisher,
         IObjectTypeResolver objectTypeResolver)
      {
         _registry = registry;
         _expressionProfileFactory = expressionProfileFactory;
         _expressionProfileUpdater = expressionProfileUpdater;
         _lazyLoadTask = lazyLoadTask;
         _registrationTask = registrationTask;
         _eventPublisher = eventPublisher;
         _objectTypeResolver = objectTypeResolver;
      }

      public MissingBuildingBlockContentRecoveryResult Recover(PKSimProject project)
      {
         var allBuildingBlocksWithMissingContent = _registry.All();
         _registry.Clear();

         if (!allBuildingBlocksWithMissingContent.Any())
            return MissingBuildingBlockContentRecoveryResult.Empty;

         var rebuiltBuildingBlocks = new List<IPKSimBuildingBlock>();
         var removedBuildingBlocks = new List<BuildingBlockWithMissingContent>();

         foreach (var buildingBlockWithMissingContent in allBuildingBlocksWithMissingContent)
         {
            var expressionProfile = rebuildExpressionProfile(buildingBlockWithMissingContent, project);
            if (expressionProfile != null)
               rebuiltBuildingBlocks.Add(expressionProfile);
            else
               removedBuildingBlocks.Add(buildingBlockWithMissingContent);
         }

         return new MissingBuildingBlockContentRecoveryResult(rebuiltBuildingBlocks, removedBuildingBlocks, messageFor(rebuiltBuildingBlocks, removedBuildingBlocks));
      }

      private ExpressionProfile rebuildExpressionProfile(BuildingBlockWithMissingContent buildingBlockWithMissingContent, PKSimProject project)
      {
         if (buildingBlockWithMissingContent.BuildingBlockType != PKSimBuildingBlockType.ExpressionProfile)
            return null;

         var (moleculeName, _, _) = NamesFromExpressionProfileName(buildingBlockWithMissingContent.Name);
         if (string.IsNullOrEmpty(moleculeName))
            return null;

         //Only an individual can be used as source: it carries the molecule with all its parameters. A population
         //referencing the expression profile does not, so the profile cannot be rebuilt from it
         var individuals = buildingBlockWithMissingContent.SimulationSubjectIds
            .Select(project.BuildingBlockById<Individual>)
            .Where(x => x != null)
            .ToList();

         individuals.Each(_lazyLoadTask.Load);

         var individualsDefiningMolecule = individuals.Where(x => x.MoleculeByName(moleculeName) != null).ToList();
         if (!individualsDefiningMolecule.Any())
            return null;

         //The expression profile may already exist in the project (for instance when it was recreated by the project conversion).
         //In that case, we only make sure that the individuals are using it
         var existingExpressionProfile = project.BuildingBlockByName<ExpressionProfile>(buildingBlockWithMissingContent.Name);
         var expressionProfile = existingExpressionProfile ?? createExpressionProfile(buildingBlockWithMissingContent, individualsDefiningMolecule[0], project);

         individualsDefiningMolecule.Each(x => x.AddExpressionProfile(expressionProfile));

         //As for any shared expression profile, the individuals that were not the source of the rebuilt profile take over its values
         var allIndividualsAreSourceOfProfile = existingExpressionProfile == null && individualsDefiningMolecule.Count == 1;
         if (!allIndividualsAreSourceOfProfile)
            _expressionProfileUpdater.SynchronizeAllSimulationSubjectsWithExpressionProfile(expressionProfile);

         return expressionProfile;
      }

      private ExpressionProfile createExpressionProfile(BuildingBlockWithMissingContent buildingBlockWithMissingContent, Individual individual, PKSimProject project)
      {
         var (moleculeName, _, _) = NamesFromExpressionProfileName(buildingBlockWithMissingContent.Name);
         var molecule = individual.MoleculeByName(moleculeName);
         var expressionProfile = _expressionProfileFactory.Create(molecule.GetType(), individual.Species, molecule.Name);

         //keep the original id so that the references to the expression profile remain valid
         expressionProfile.Id = buildingBlockWithMissingContent.Id;
         expressionProfile.Name = buildingBlockWithMissingContent.Name;
         expressionProfile.Description = buildingBlockWithMissingContent.Description;
         expressionProfile.Version = buildingBlockWithMissingContent.Version;
         expressionProfile.StructureVersion = buildingBlockWithMissingContent.StructureVersion;

         _expressionProfileUpdater.SynchronizeExpressionProfileWithSimulationSubject(expressionProfile, individual);

         //the rebuilt expression profile does not exist in the project file yet and must be saved with the next save
         expressionProfile.IsLoaded = true;
         expressionProfile.HasChanged = true;

         project.AddBuildingBlock(expressionProfile);
         _registrationTask.Register(expressionProfile);
         _eventPublisher.PublishEvent(new BuildingBlockAddedEvent(expressionProfile, project));
         return expressionProfile;
      }

      private string messageFor(IReadOnlyList<IPKSimBuildingBlock> rebuiltBuildingBlocks, IReadOnlyList<BuildingBlockWithMissingContent> removedBuildingBlocks)
      {
         var rebuilt = rebuiltBuildingBlocks.Select(x => displayFor(_objectTypeResolver.TypeFor(x), x.Name)).ToList();
         var removed = removedBuildingBlocks.Select(x => displayFor(typeDisplayFor(x.BuildingBlockType), x.Name)).ToList();
         return PKSimConstants.Warning.BuildingBlocksWithMissingContentRecovered(rebuilt, removed);
      }

      private static string displayFor(string buildingBlockType, string name) => $"{buildingBlockType} '{name}'";

      private static string typeDisplayFor(PKSimBuildingBlockType buildingBlockType)
      {
         switch (buildingBlockType)
         {
            case PKSimBuildingBlockType.Compound:
               return PKSimConstants.ObjectTypes.Compound;
            case PKSimBuildingBlockType.Individual:
               return PKSimConstants.ObjectTypes.Individual;
            case PKSimBuildingBlockType.Population:
               return PKSimConstants.ObjectTypes.Population;
            case PKSimBuildingBlockType.Protocol:
               return PKSimConstants.ObjectTypes.AdministrationProtocol;
            case PKSimBuildingBlockType.Formulation:
               return PKSimConstants.ObjectTypes.Formulation;
            case PKSimBuildingBlockType.Event:
               return PKSimConstants.ObjectTypes.Event;
            case PKSimBuildingBlockType.Simulation:
               return PKSimConstants.ObjectTypes.Simulation;
            case PKSimBuildingBlockType.ObserverSet:
               return PKSimConstants.ObjectTypes.ObserverSet;
            case PKSimBuildingBlockType.ExpressionProfile:
               return PKSimConstants.ObjectTypes.ExpressionProfile;
            default:
               return buildingBlockType.ToString();
         }
      }
   }
}
