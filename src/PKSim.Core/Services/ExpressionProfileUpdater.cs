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
      void SynchronizeExpressionProfile(ISimulationSubject simulationSubject, ExpressionProfile expressionProfile);
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

      public void SynchronizeExpressionProfile(ISimulationSubject simulationSubject, ExpressionProfile expressionProfile)
      {
         _lazyLoadTask.Load(simulationSubject);
         var molecule = simulationSubject.MoleculeByName(expressionProfile.MoleculeName);
         
         //Global settings for molecule
         updateGlobalMoleculeSettings(molecule, expressionProfile, simulationSubject);

         //All molecule parameters
         updateMoleculeParameters(molecule, expressionProfile, simulationSubject);
         
         //Molecule containers
         updateTransporterDirections(molecule, expressionProfile, simulationSubject);
      }

      private void updateMoleculeParameters(IndividualMolecule molecule, ExpressionProfile expressionProfile, ISimulationSubject simulationSubject)
      {
         var allMoleculeParametersPathCache = allMoleculeParametersFor(simulationSubject, molecule);
         var allExpressionProfileMoleculeParametersCache = allMoleculeParametersFor(expressionProfile.Individual, expressionProfile.Molecule);


         //Updates between two building blocks. We do not update parameter origin
         _parameterSetUpdater.UpdateValues(allExpressionProfileMoleculeParametersCache, allMoleculeParametersPathCache, updateParameterOriginId: false);
      }

      public void SynchronizeExpressionProfileInAllSimulationSubjects(ExpressionProfile expressionProfile)
      {
         var allSimulationSubjectsForProfile = _projectRetriever.Current.All<ISimulationSubject>()
            .Where(x => x.Uses(expressionProfile))
            .ToList();

         allSimulationSubjectsForProfile.Each(x => SynchronizeExpressionProfile(x, expressionProfile));
      }

      public void SynchronizeExpressionProfileInAllSimulationSubjects(ISimulationSubject internalSimulationSubject)
      {
         var expressionProfile = internalSimulationSubject.OwnedBy as ExpressionProfile;
         if (expressionProfile == null) return;
         SynchronizeExpressionProfileInAllSimulationSubjects(expressionProfile);
      }

      private PathCache<IParameter> allMoleculeParametersFor(ISimulationSubject simulationSubject, IndividualMolecule molecule) 
         => _containerTask.PathCacheFor(simulationSubject.Individual.AllMoleculeParametersFor(molecule));

      private void updateTransporterDirections(IndividualMolecule molecule, ExpressionProfile expressionProfile, ISimulationSubject simulationSubject)
      {
         var transporter = expressionProfile.Molecule as IndividualTransporter;
         if (transporter == null)
            return;

         var allExpressionProfileTransporterContainer = allTransporterExpressionContainerFor(expressionProfile.Individual, expressionProfile.Molecule);
         var allIndividualTransporterContainer = allTransporterExpressionContainerFor(simulationSubject.Individual, molecule);

         foreach (var keyValuePair in allExpressionProfileTransporterContainer.KeyValues)
         {
            var expressionProfileContainer = keyValuePair.Value;
            var individualContainer = allIndividualTransporterContainer[keyValuePair.Key];
            individualContainer?.UpdatePropertiesFrom(expressionProfileContainer, _cloner);
         }
      }

      private PathCache<TransporterExpressionContainer> allTransporterExpressionContainerFor(Individual individual, IndividualMolecule molecule)
      {
         var allTransporterExpressionContainer = individual.AllMoleculeContainersFor<TransporterExpressionContainer>(molecule);
         return _containerTask.PathCacheFor(allTransporterExpressionContainer);
      }

      private void updateGlobalMoleculeSettings(IndividualMolecule molecule, ExpressionProfile expressionProfile, ISimulationSubject simulationSubject)
      {
         molecule.UpdatePropertiesFrom(expressionProfile.Molecule, _cloner);
         _ontogenyTask.SetOntogenyForMolecule(molecule, molecule.Ontogeny, simulationSubject);
      }
   }
}