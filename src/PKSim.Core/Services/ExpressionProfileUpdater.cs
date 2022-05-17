using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IExpressionProfileUpdater
   {
      ICommand UpdateExpressionFromQuery(ExpressionProfile expressionProfile, QueryExpressionResults queryResults);

      /// <summary>
      ///    Update the molecule name in <paramref name="expressionProfile" /> to be <paramref name="newMoleculeName" />
      /// </summary>
      ICommand UpdateMoleculeName(ExpressionProfile expressionProfile, string newMoleculeName);

      /// <summary>
      ///    Updates the value from the <paramref name="expressionProfile" /> into the <paramref name="simulationSubject" />.
      ///    ExpressionProfile => SimulationSubject
      /// </summary>
      /// <param name="simulationSubject">Simulation subject to update</param>
      /// <param name="expressionProfile">Expression profile used as source</param>
      void SynchroniseSimulationSubjectWithExpressionProfile(ISimulationSubject simulationSubject, ExpressionProfile expressionProfile);

      /// <summary>
      ///    Updates the value from the <paramref name="expressionProfile" /> into all simulation subjects defined in the project
      ///    referencing <paramref name="expressionProfile" />.  ExpressionProfile => All SimulationSubject in Project using it
      /// </summary>
      /// <param name="expressionProfile">Expression profile used as source</param>
      void SynchronizeAllSimulationSubjectsWithExpressionProfile(ExpressionProfile expressionProfile);

      void SynchronizeAllSimulationSubjectsWithExpressionProfile(ISimulationSubject internalSimulationSubject);

      /// <summary>
      ///    Updates the value from the <paramref name="simulationSubject" /> into the <paramref name="expressionProfile" />.
      ///    SimulationSubject => ExpressionProfile. This is typically called for project conversion.
      /// </summary>
      /// <param name="expressionProfile">Expression profile to update</param>
      /// <param name="simulationSubject">Simulation subject used as source</param>
      void SynchronizeExpressionProfileWithSimulationSubject(ExpressionProfile expressionProfile, ISimulationSubject simulationSubject);

      /// <summary>
      ///    Updates the value from the <paramref name="sourceExpressionProfile" /> into the
      ///    <paramref name="targetExpressionProfile" />.
      ///    ExpressionProfile => ExpressionProfile. This is typically called when cloning an expression profile
      /// </summary>
      /// <param name="sourceExpressionProfile">Expression profile to update used as source</param>
      /// <param name="targetExpressionProfile">Expression profile to update</param>
      void SynchronizeExpressionProfileWithExpressionProfile(ExpressionProfile sourceExpressionProfile, ExpressionProfile targetExpressionProfile);
   }

   public class ExpressionProfileUpdater : IExpressionProfileUpdater
   {
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IContainerTask _containerTask;
      private readonly IOntogenyTask _ontogenyTask;
      private readonly ICloner _cloner;
      private readonly IPKSimProjectRetriever _projectRetriever;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IParameterIdUpdater _parameterIdUpdater;
      private readonly IExecutionContext _executionContext;
      private readonly IDiseaseStateImplementationFactory _diseaseStateImplementationFactory;

      public ExpressionProfileUpdater(
         IParameterSetUpdater parameterSetUpdater,
         IContainerTask containerTask,
         IOntogenyTask ontogenyTask,
         ICloner cloner,
         IPKSimProjectRetriever projectRetriever,
         ILazyLoadTask lazyLoadTask,
         IParameterIdUpdater parameterIdUpdater,
         IExecutionContext executionContext,
         IDiseaseStateImplementationFactory diseaseStateImplementationFactory)
      {
         _parameterSetUpdater = parameterSetUpdater;
         _containerTask = containerTask;
         _ontogenyTask = ontogenyTask;
         _cloner = cloner;
         _projectRetriever = projectRetriever;
         _lazyLoadTask = lazyLoadTask;
         _parameterIdUpdater = parameterIdUpdater;
         _executionContext = executionContext;
         _diseaseStateImplementationFactory = diseaseStateImplementationFactory;
      }

      public ICommand UpdateExpressionFromQuery(ExpressionProfile expressionProfile, QueryExpressionResults queryResults)
      {
         var (molecule, individual) = expressionProfile;
         molecule.QueryConfiguration = queryResults.QueryConfiguration;
         return new EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand(molecule, queryResults, individual)
            .Run(_executionContext);
      }

      public ICommand UpdateMoleculeName(ExpressionProfile expressionProfile, string newMoleculeName)
      {
         var command = new PKSimMacroCommand();

         var oldMoleculeName = expressionProfile.MoleculeName;
         //we are not renaming anything
         if (string.Equals(newMoleculeName, oldMoleculeName))
            return command;

         var (_, individual) = expressionProfile;
         var mainCommand = renameMoleculeReferences(individual, oldMoleculeName, newMoleculeName);
         command.Add(mainCommand);
         command.UpdatePropertiesFrom(mainCommand);
         allSimulationSubjectsUsing(expressionProfile).Each(x =>
         {
            _lazyLoadTask.Load(x);
            command.Add(renameMoleculeReferences(x, oldMoleculeName, newMoleculeName));
         });


         return command;
      }

      private IOSPSuiteCommand renameMoleculeReferences(ISimulationSubject simulationSubject, string oldMoleculeName, string newMoleculeName)
      {
         return new RenameMoleculeReferenceInSimulationSubjectCommand(simulationSubject, oldMoleculeName, newMoleculeName, _executionContext).Run(_executionContext);
      }

      private void synchronizeExpressionProfiles(IndividualMolecule sourceMolecule, ISimulationSubject sourceSimulationSubject, IndividualMolecule targetMolecule, ISimulationSubject targetSimulationSubject, bool updateParameterOriginId)
      {
         //Global settings for molecule
         updateGlobalMoleculeSettings(sourceMolecule, targetMolecule, targetSimulationSubject);

         //All molecule parameters
         updateMoleculeParameters(sourceMolecule, sourceSimulationSubject, targetMolecule, targetSimulationSubject, updateParameterOriginId);

         //Molecule containers
         updateTransporterDirections(sourceMolecule, sourceSimulationSubject, targetMolecule, targetSimulationSubject);
      }

      public void SynchroniseSimulationSubjectWithExpressionProfile(ISimulationSubject simulationSubject, ExpressionProfile expressionProfile)
      {
         _lazyLoadTask.Load(simulationSubject);
         var moleculeInIndividual = simulationSubject.MoleculeByName(expressionProfile.MoleculeName);
         var (sourceMolecule, sourceIndividual) = expressionProfile;
         // ExpressionProfile => SimulationSubject, we want to make sure that the parameters in simulation subject are linked to their expression profile origin parameters
         synchronizeExpressionProfiles(sourceMolecule, sourceIndividual, moleculeInIndividual, simulationSubject, updateParameterOriginId: true);

         //Once the synchronization was performed, apply changes to simulation subject molecules based on disease state
         updateMoleculeParametersForDiseaseState(simulationSubject, moleculeInIndividual);
      }

      private void updateMoleculeParametersForDiseaseState(ISimulationSubject simulationSubject, IndividualMolecule moleculeInIndividual)
      {
         var diseaseStateImplementation = _diseaseStateImplementationFactory.CreateFor(simulationSubject.Individual);
         diseaseStateImplementation.ApplyTo(moleculeInIndividual);
      }

      public void SynchronizeExpressionProfileWithSimulationSubject(ExpressionProfile expressionProfile, ISimulationSubject simulationSubject)
      {
         var moleculeInIndividual = simulationSubject.MoleculeByName(expressionProfile.MoleculeName);
         var (targetMolecule, targetIndividual) = expressionProfile;

         // SimulationSubject To ExpressionProfile. We do not update the parameter origin id in the target entities (expression profile)
         synchronizeExpressionProfiles(moleculeInIndividual, simulationSubject, targetMolecule, targetIndividual, updateParameterOriginId: false);

         //however we need to make sure that we reference the expression profile parameter in the source individual
         var allExpressionProfileParameters = allMoleculeParametersFor(targetIndividual, targetMolecule);
         var allIndividualParameters = allMoleculeParametersFor(simulationSubject, moleculeInIndividual);

         _parameterIdUpdater.UpdateParameterIds(allExpressionProfileParameters, allIndividualParameters);
      }

      public void SynchronizeExpressionProfileWithExpressionProfile(ExpressionProfile sourceExpressionProfile, ExpressionProfile targetExpressionProfile)
      {
         var (sourceMolecule, sourceIndividual) = sourceExpressionProfile;
         var (targetMolecule, targetIndividual) = targetExpressionProfile;

         // ExpressionProfile To ExpressionProfile. We do not update the parameter origin id in the target entities as we are updating one building block from another one
         synchronizeExpressionProfiles(sourceMolecule, sourceIndividual, targetMolecule, targetIndividual, updateParameterOriginId: false);
      }

      private void updateMoleculeParameters(IndividualMolecule sourceMolecule, ISimulationSubject sourceSimulationSubject, IndividualMolecule targetMolecule, ISimulationSubject targetSimulationSubject, bool updateParameterOriginId)
      {
         var allTargetMoleculeParameters = allMoleculeParametersFor(targetSimulationSubject, targetMolecule);
         var allSourceMoleculeParameters = allMoleculeParametersFor(sourceSimulationSubject, sourceMolecule);

         _parameterSetUpdater.UpdateValues(allSourceMoleculeParameters, allTargetMoleculeParameters, updateParameterOriginId);
      }

      public void SynchronizeAllSimulationSubjectsWithExpressionProfile(ExpressionProfile expressionProfile)
      {
         var allSimulationSubjectsForProfile = allSimulationSubjectsUsing(expressionProfile);
         allSimulationSubjectsForProfile.Each(x => SynchroniseSimulationSubjectWithExpressionProfile(x, expressionProfile));
      }

      private IReadOnlyList<ISimulationSubject> allSimulationSubjectsUsing(ExpressionProfile expressionProfile)
      {
         return _projectRetriever.Current?.All<ISimulationSubject>()
            .Where(x => x.Uses(expressionProfile))
            .ToArray() ?? Array.Empty<ISimulationSubject>();
      }

      public void SynchronizeAllSimulationSubjectsWithExpressionProfile(ISimulationSubject internalSimulationSubject)
      {
         var expressionProfile = internalSimulationSubject.OwnedBy as ExpressionProfile;
         if (expressionProfile == null) return;
         SynchronizeAllSimulationSubjectsWithExpressionProfile(expressionProfile);
      }

      private PathCache<IParameter> allMoleculeParametersFor(ISimulationSubject simulationSubject, IndividualMolecule molecule)
         => _containerTask.PathCacheFor(simulationSubject.Individual.AllMoleculeParametersFor(molecule));

      private void updateTransporterDirections(IndividualMolecule sourceMolecule, ISimulationSubject sourceSimulationSubject, IndividualMolecule targetMolecule, ISimulationSubject targetSimulationSubject)
      {
         var transporter = sourceMolecule as IndividualTransporter;
         if (transporter == null)
            return;

         var sourceTransporterExpressionContainers = allTransporterExpressionContainerFor(sourceSimulationSubject.Individual, sourceMolecule);
         var targetTransporterExpressionContainers = allTransporterExpressionContainerFor(targetSimulationSubject.Individual, targetMolecule);

         foreach (var keyValuePair in sourceTransporterExpressionContainers.KeyValues)
         {
            var sourceContainer = keyValuePair.Value;
            var targetContainer = targetTransporterExpressionContainers[keyValuePair.Key];
            targetContainer?.UpdatePropertiesFrom(sourceContainer, _cloner);
         }
      }

      private PathCache<TransporterExpressionContainer> allTransporterExpressionContainerFor(Individual individual, IndividualMolecule molecule)
      {
         var allTransporterExpressionContainer = individual.AllMoleculeContainersFor<TransporterExpressionContainer>(molecule);
         return _containerTask.PathCacheFor(allTransporterExpressionContainer);
      }

      private void updateGlobalMoleculeSettings(IndividualMolecule sourceMolecule, IndividualMolecule targetMolecule, ISimulationSubject targetSimulationSubject)
      {
         targetMolecule.UpdatePropertiesFrom(sourceMolecule, _cloner);
         _ontogenyTask.SetOntogenyForMolecule(targetMolecule, targetMolecule.Ontogeny, targetSimulationSubject);
      }
   }
}