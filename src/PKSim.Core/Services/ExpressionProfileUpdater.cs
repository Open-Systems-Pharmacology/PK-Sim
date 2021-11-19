using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IExpressionProfileUpdater
   {
      ICommand UpdateMoleculeName(ExpressionProfile expressionProfile);
      /// <summary>
      /// Updates the value from the <paramref name="expressionProfile"/> into the <paramref name="simulationSubject"/>.
      /// ExpressionProfile => SimulationSubject
      /// </summary>
      /// <param name="simulationSubject">Simulation subject to update</param>
      /// <param name="expressionProfile">Expression profile used as source</param>
      void SynchroniseSimulationSubjectWithExpressionProfile(ISimulationSubject simulationSubject, ExpressionProfile expressionProfile);

      /// <summary>
      /// Updates the value from the <paramref name="simulationSubject"/> into the <paramref name="expressionProfile"/>.
      /// SimulationSubject => ExpressionProfile 
      /// </summary>
      /// <param name="expressionProfile">Expression profile to update</param>
      /// <param name="simulationSubject">Simulation subject used as source</param>
      void SynchronizeExpressionProfileWithSimulationSubject(ExpressionProfile expressionProfile, ISimulationSubject simulationSubject);

      void SynchronizeExpressionProfileInAllSimulationSubjects(ExpressionProfile expressionProfile);
      void SynchronizeExpressionProfileInAllSimulationSubjects(ISimulationSubject internalSimulationSubject);
   }

   public class ExpressionProfileUpdater : IExpressionProfileUpdater
   {
      private readonly ISimulationSubjectExpressionTask<Individual> _individualExpressionTask;
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IContainerTask _containerTask;
      private readonly IOntogenyTask _ontogenyTask;
      private readonly ICloner _cloner;
      private readonly IPKSimProjectRetriever _projectRetriever;
      private readonly ILazyLoadTask _lazyLoadTask;

      public ExpressionProfileUpdater(
         ISimulationSubjectExpressionTask<Individual> individualExpressionTask,
         IParameterSetUpdater parameterSetUpdater,
         IContainerTask containerTask,
         IOntogenyTask ontogenyTask,
         ICloner cloner,
         IPKSimProjectRetriever projectRetriever,
         ILazyLoadTask lazyLoadTask)
      {
         _individualExpressionTask = individualExpressionTask;
         _parameterSetUpdater = parameterSetUpdater;
         _containerTask = containerTask;
         _ontogenyTask = ontogenyTask;
         _cloner = cloner;
         _projectRetriever = projectRetriever;
         _lazyLoadTask = lazyLoadTask;
      }

      public ICommand UpdateMoleculeName(ExpressionProfile expressionProfile)
      {
         return _individualExpressionTask.RenameMolecule(expressionProfile.Molecule, expressionProfile.MoleculeName, expressionProfile.Individual);
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

         // ExpressionProfile => SimulationSubject, we want to make sure that the parameters in simulation subject are linked to their expression profile origin parameters
         synchronizeExpressionProfiles(expressionProfile.Molecule, expressionProfile.Individual, moleculeInIndividual, simulationSubject, updateParameterOriginId:true);
      }

      public void SynchronizeExpressionProfileWithSimulationSubject(ExpressionProfile expressionProfile, ISimulationSubject simulationSubject)
      {
         var moleculeInIndividual = simulationSubject.MoleculeByName(expressionProfile.MoleculeName);
         // SimulationSubject To ExpressionProfile. We do not update the parameter origin id in the expression profile
         synchronizeExpressionProfiles(moleculeInIndividual, simulationSubject, expressionProfile.Molecule, expressionProfile.Individual, updateParameterOriginId: false);

      }

      private void updateMoleculeParameters(IndividualMolecule sourceMolecule, ISimulationSubject sourceSimulationSubject, IndividualMolecule targetMolecule, ISimulationSubject targetSimulationSubject, bool updateParameterOriginId)
      {
         var allTargetMoleculeParameters= allMoleculeParametersFor(targetSimulationSubject, targetMolecule);
         var allSourceMoleculeParameters = allMoleculeParametersFor(sourceSimulationSubject, sourceMolecule);


         _parameterSetUpdater.UpdateValues(allSourceMoleculeParameters, allTargetMoleculeParameters, updateParameterOriginId);
      }

      public void SynchronizeExpressionProfileInAllSimulationSubjects(ExpressionProfile expressionProfile)
      {
         var allSimulationSubjectsForProfile = _projectRetriever.Current.All<ISimulationSubject>()
            .Where(x => x.Uses(expressionProfile))
            .ToList();

         allSimulationSubjectsForProfile.Each(x => SynchroniseSimulationSubjectWithExpressionProfile(x, expressionProfile));
      }

      public void SynchronizeExpressionProfileInAllSimulationSubjects(ISimulationSubject internalSimulationSubject)
      {
         var expressionProfile = internalSimulationSubject.OwnedBy as ExpressionProfile;
         if (expressionProfile == null) return;
         SynchronizeExpressionProfileInAllSimulationSubjects(expressionProfile);
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