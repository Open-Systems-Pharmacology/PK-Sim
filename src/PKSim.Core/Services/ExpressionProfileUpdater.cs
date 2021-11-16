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
      void SynchronizeExpressionProfile(Individual individual, ExpressionProfile expressionProfile);
      void SynchronizeExpressionProfileInAllIndividuals(ExpressionProfile expressionProfile);
      void SynchronizeExpressionProfileInAllIndividuals(ISimulationSubject internalSimulationSubject);
   }

   public class ExpressionProfileUpdater : IExpressionProfileUpdater
   {
      private readonly ISimulationSubjectExpressionTask<Individual> _individualExpressionTask;
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IContainerTask _containerTask;
      private readonly IOntogenyTask<Individual> _ontogenyTask;
      private readonly ICloner _cloner;
      private readonly IPKSimProjectRetriever _projectRetriever;
      private readonly ILazyLoadTask _lazyLoadTask;

      public ExpressionProfileUpdater(
         ISimulationSubjectExpressionTask<Individual> individualExpressionTask,
         IParameterSetUpdater parameterSetUpdater,
         IContainerTask containerTask,
         IOntogenyTask<Individual> ontogenyTask,
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

      public void SynchronizeExpressionProfile(Individual individual, ExpressionProfile expressionProfile)
      {
         _lazyLoadTask.Load(individual);
         var molecule = individual.MoleculeByName(expressionProfile.MoleculeName);
         
         //Global settings for molecule
         updateGlobalMoleculeSettings(molecule, expressionProfile, individual);

         //All molecule parameters
         updateMoleculeParameters(molecule, expressionProfile, individual);
         
         //Molecule containers
         updateTransporterDirections(molecule, expressionProfile, individual);
      }

      private void updateMoleculeParameters(IndividualMolecule molecule, ExpressionProfile expressionProfile, Individual individual)
      {
         var allMoleculeParametersPathCache = allMoleculeParametersFor(individual, molecule);
         var allExpressionProfileMoleculeParametersCache = allMoleculeParametersFor(expressionProfile.Individual, expressionProfile.Molecule);


         //Updates between two building blocks. We do not update parameter origin
         _parameterSetUpdater.UpdateValues(allExpressionProfileMoleculeParametersCache, allMoleculeParametersPathCache, updateParameterOriginId: false);
      }

      public void SynchronizeExpressionProfileInAllIndividuals(ExpressionProfile expressionProfile)
      {
         var allSimulationSubjectsForProfile = _projectRetriever.Current.All<ISimulationSubject>()
            .Where(x => x.Uses(expressionProfile))
            .ToList();

         allSimulationSubjectsForProfile.Each(x => SynchronizeExpressionProfile(x.Individual, expressionProfile));
      }

      public void SynchronizeExpressionProfileInAllIndividuals(ISimulationSubject internalSimulationSubject)
      {
         var expressionProfile = internalSimulationSubject.OwnedBy as ExpressionProfile;
         if (expressionProfile == null) return;
         SynchronizeExpressionProfileInAllIndividuals(expressionProfile);
      }

      private PathCache<IParameter> allMoleculeParametersFor(Individual individual, IndividualMolecule molecule) 
         => _containerTask.PathCacheFor(individual.AllMoleculeParametersFor(molecule));

      private void updateTransporterDirections(IndividualMolecule molecule, ExpressionProfile expressionProfile, Individual individual)
      {
         var transporter = expressionProfile.Molecule as IndividualTransporter;
         if (transporter == null)
            return;

         var allExpressionProfileTransporterContainer = allTransporterExpressionContainerFor(expressionProfile.Individual, expressionProfile.Molecule);
         var allIndividualTransporterContainer = allTransporterExpressionContainerFor(individual, molecule);

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

      private void updateGlobalMoleculeSettings(IndividualMolecule molecule, ExpressionProfile expressionProfile, Individual individual)
      {
         molecule.UpdatePropertiesFrom(expressionProfile.Molecule, _cloner);
         _ontogenyTask.SetOntogenyForMolecule(molecule, molecule.Ontogeny, individual);
      }
   }
}