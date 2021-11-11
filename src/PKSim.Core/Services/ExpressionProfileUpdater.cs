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
   }

   public class ExpressionProfileUpdater : IExpressionProfileUpdater
   {
      private readonly ISimulationSubjectExpressionTask<Individual> _individualExpressionTask;
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IOntogenyTask<Individual> _ontogenyTask;
      private readonly ICloner _cloner;
      private readonly IPKSimProjectRetriever _projectRetriever;

      public ExpressionProfileUpdater(
         ISimulationSubjectExpressionTask<Individual> individualExpressionTask,
         IParameterSetUpdater parameterSetUpdater,
         IEntityPathResolver entityPathResolver,
         IOntogenyTask<Individual> ontogenyTask,
         ICloner cloner,
         IPKSimProjectRetriever projectRetriever)
      {
         _individualExpressionTask = individualExpressionTask;
         _parameterSetUpdater = parameterSetUpdater;
         _entityPathResolver = entityPathResolver;
         _ontogenyTask = ontogenyTask;
         _cloner = cloner;
         _projectRetriever = projectRetriever;
      }

      public ICommand UpdateMoleculeName(ExpressionProfile expressionProfile)
      {
         return _individualExpressionTask.RenameMolecule(expressionProfile.Molecule, expressionProfile.MoleculeName, expressionProfile.Individual);
      }

      public void SynchronizeExpressionProfile(Individual individual, ExpressionProfile expressionProfile)
      {
         var molecule = individual.MoleculeByName(expressionProfile.MoleculeName);
         var allMoleculeParametersPathCache = allMoleculeParametersFor(individual, molecule);
         var allExpressionProfileMoleculeParametersCache = allMoleculeParametersFor(expressionProfile.Individual, expressionProfile.Molecule);

         updateTransporterDirections(molecule, individual, expressionProfile);
         updateGlobalMoleculeSettings(molecule, expressionProfile.Molecule, individual);
         _parameterSetUpdater.UpdateValues(allExpressionProfileMoleculeParametersCache, allMoleculeParametersPathCache);
      }

      public void SynchronizeExpressionProfileInAllIndividuals(ExpressionProfile expressionProfile)
      {
         var allSimulationSubjectsForProfile = _projectRetriever.Current.All<ISimulationSubject>()
            .Where(x => x.Uses(expressionProfile))
            .ToList();

         allSimulationSubjectsForProfile.Each(x => SynchronizeExpressionProfile(x.Individual, expressionProfile));
      }

      private PathCache<IParameter> allMoleculeParametersFor(Individual individual, IndividualMolecule molecule)
      {
         var allMoleculeParameters = individual.AllMoleculeParametersFor(molecule);
         return new PathCache<IParameter>(_entityPathResolver).For(allMoleculeParameters);
      }

      private void updateTransporterDirections(IndividualMolecule molecule, Individual individual, ExpressionProfile expressionProfile)
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
         return new PathCache<TransporterExpressionContainer>(_entityPathResolver).For(allTransporterExpressionContainer);
      }

      private void updateGlobalMoleculeSettings(IndividualMolecule molecule, IndividualMolecule expressionProfileMolecule, Individual individual)
      {
         molecule.UpdatePropertiesFrom(expressionProfileMolecule, _cloner);
         _ontogenyTask.SetOntogenyForMolecule(molecule, molecule.Ontogeny, individual);
      }
   }
}