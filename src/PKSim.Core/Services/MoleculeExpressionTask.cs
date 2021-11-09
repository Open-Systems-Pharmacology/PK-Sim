using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IMoleculeExpressionTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      /// <summary>
      ///    Add a molecule of type <typeparamref name="TMolecule" /> to the given individual named
      ///    <paramref name="moleculeName" />
      /// </summary>
      /// <typeparam name="TMolecule">Type of molecule to add. The molecule will be created depending on this type </typeparam>
      /// <param name="simulationSubject">Simulation subject where the molecule will be added</param>
      /// <param name="moleculeName">Name of the molecule to add</param>
      ICommand AddMoleculeTo<TMolecule>(TSimulationSubject simulationSubject, string moleculeName) where TMolecule : IndividualMolecule;

      /// <summary>
      ///    Add a molecule of type <typeparamref name="TMolecule" /> to the given individual named based on the
      ///    <paramref name="expressionProfile" />
      /// </summary>
      /// <typeparam name="TMolecule">Type of molecule to add. The molecule will be created depending on this type </typeparam>
      /// <param name="simulationSubject">Simulation subject where the molecule will be added</param>
      /// <param name="expressionProfile">Expression profile to add to the simulation subject</param>
      ICommand AddExpressionProfile<TMolecule>(TSimulationSubject simulationSubject, ExpressionProfile expressionProfile) where TMolecule : IndividualMolecule;

      /// <summary>
      ///    Add a molecule of type to the given individual named after the <paramref name="moleculeTemplate" /> template given
      ///    as
      ///    parameter
      /// </summary>
      /// <param name="simulationSubject">Simulation subject where the molecule will be added</param>
      /// <param name="moleculeTemplate">New Molecule will be added based on this parameter (type and name). </param>
      ICommand AddMoleculeTo(TSimulationSubject simulationSubject, IndividualMolecule moleculeTemplate);

      ICommand AddMoleculeTo(TSimulationSubject simulationSubject, IndividualMolecule molecule, QueryExpressionResults queryExpressionResults);

      /// <summary>
      ///    Set the value of the relative expression for  <paramref name="relativeExpressionParameter" />
      /// </summary>
      /// <param name="relativeExpressionParameter">
      ///    Relative expression parameter
      /// </param>
      /// <param name="value">relative expression</param>
      ICommand SetRelativeExpressionFor(IParameter relativeExpressionParameter, double value);

      /// <summary>
      ///    Remove the given molecule from the simulationSubject
      /// </summary>
      /// <param name="moleculeToRemove">Molecule to be removed</param>
      /// <param name="simulationSubject">Simulation subject containing the molecule to be removed</param>
      ICommand RemoveMoleculeFrom(IndividualMolecule moleculeToRemove, TSimulationSubject simulationSubject);

      /// <summary>
      ///    Updates the transport direction type for the transporter container given as parameter
      /// </summary>
      ICommand SetTransportDirection(TransporterExpressionContainer transporterContainer, TransportDirectionId transportDirection);

      /// <summary>
      ///    Update the localization of the protein
      /// </summary>
      ICommand SetExpressionLocalizationFor(IndividualProtein protein, Localization localization, TSimulationSubject simulationSubject);

      /// <summary>
      ///    Updates the transporter type for all organ defines for transporter with the given transporter type (only if the
      ///    transporter type is defined)
      /// </summary>
      ICommand SetTransporterTypeFor(IndividualTransporter transporter, TransportType transportType);

      ICommand EditMolecule(IndividualMolecule moleculeToEdit, QueryExpressionResults queryResults,
         TSimulationSubject simulationSubject);

      ICommand RenameMolecule(IndividualMolecule molecule, string newName, TSimulationSubject simulationSubject);
   }

   public class MoleculeExpressionTask<TSimulationSubject> : IMoleculeExpressionTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      private readonly IContainerTask _containerTask;
      private readonly IExecutionContext _executionContext;
      private readonly IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly ITransportContainerUpdater _transportContainerUpdater;
      private readonly ISimulationSubjectExpressionTask<TSimulationSubject> _simulationSubjectExpressionTask;
      private readonly IOntogenyTask<TSimulationSubject> _ontogenyTask;
      private readonly IMoleculeParameterTask _moleculeParameterTask;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IParameterSetUpdater _parameterSetUpdater;

      public MoleculeExpressionTask(IExecutionContext executionContext,
         IIndividualMoleculeFactoryResolver individualMoleculeFactoryResolver,
         IContainerTask containerTask,
         IOntogenyRepository ontogenyRepository,
         ITransportContainerUpdater transportContainerUpdater,
         ISimulationSubjectExpressionTask<TSimulationSubject> simulationSubjectExpressionTask,
         IOntogenyTask<TSimulationSubject> ontogenyTask,
         IMoleculeParameterTask moleculeParameterTask,
         IEntityPathResolver entityPathResolver,
         IParameterSetUpdater parameterSetUpdater)
      {
         _executionContext = executionContext;
         _individualMoleculeFactoryResolver = individualMoleculeFactoryResolver;
         _containerTask = containerTask;
         _ontogenyRepository = ontogenyRepository;
         _transportContainerUpdater = transportContainerUpdater;
         _simulationSubjectExpressionTask = simulationSubjectExpressionTask;
         _ontogenyTask = ontogenyTask;
         _moleculeParameterTask = moleculeParameterTask;
         _entityPathResolver = entityPathResolver;
         _parameterSetUpdater = parameterSetUpdater;
      }

      public ICommand SetRelativeExpressionFor(IParameter relativeExpressionParameter, double value)
      {
         return new SetRelativeExpressionCommand(relativeExpressionParameter, value).Run(_executionContext);
      }

      public ICommand RemoveMoleculeFrom(IndividualMolecule molecule, TSimulationSubject simulationSubject)
      {
         return _simulationSubjectExpressionTask.RemoveMoleculeFrom(molecule, simulationSubject);
      }

      public ICommand AddMoleculeTo<TMolecule>(TSimulationSubject simulationSubject, string moleculeName) where TMolecule : IndividualMolecule
      {
         var moleculeFactory = _individualMoleculeFactoryResolver.FactoryFor<TMolecule>();
         var molecule = moleculeFactory.AddMoleculeTo(simulationSubject, moleculeName);
         return addMoleculeTo(molecule, simulationSubject);
      }

      public ICommand AddExpressionProfile<TMolecule>(TSimulationSubject simulationSubject, ExpressionProfile expressionProfile) where TMolecule : IndividualMolecule
      {
         var moleculeFactory = _individualMoleculeFactoryResolver.FactoryFor<TMolecule>();
         var molecule = moleculeFactory.AddMoleculeTo(simulationSubject, expressionProfile.MoleculeName);
         simulationSubject.AddExpressionProfile(expressionProfile);
         updateFromExpressionProfile(simulationSubject, expressionProfile);
         return _simulationSubjectExpressionTask.AddMoleculeTo(molecule, simulationSubject);       
      }

      private ICommand updateFromExpressionProfile(TSimulationSubject simulationSubject, ExpressionProfile expressionProfile)
      {
         var molecule = simulationSubject.MoleculeByName(expressionProfile.MoleculeName);
         var allMoleculeParametersPathCache = allMoleculeParametersFor(simulationSubject.Individual, molecule);
         var allExpressionProfileMoleculeParametersCache = allMoleculeParametersFor(expressionProfile.Individual, expressionProfile.Molecule);

         updateTransporterDirections(molecule, simulationSubject, expressionProfile);
         updateGlobalMoleculeSettings(molecule, expressionProfile.Molecule, simulationSubject);
         return _parameterSetUpdater.UpdateValues(allExpressionProfileMoleculeParametersCache, allMoleculeParametersPathCache);
      }

      private PathCache<IParameter> allMoleculeParametersFor(Individual individual, IndividualMolecule molecule)
      {
         var allMoleculeParameters = individual.AllMoleculeParametersFor(molecule);
         return new PathCache<IParameter>(_entityPathResolver).For(allMoleculeParameters);
      }

      public ICommand AddMoleculeTo(TSimulationSubject simulationSubject, IndividualMolecule moleculeTemplate)
      {
         var moleculeFactory = _individualMoleculeFactoryResolver.FactoryFor(moleculeTemplate);
         var molecule = moleculeFactory.AddMoleculeTo(simulationSubject, moleculeTemplate.Name);
         return addMoleculeTo(molecule, simulationSubject);
      }

      public ICommand EditMolecule(IndividualMolecule moleculeToEdit, QueryExpressionResults queryResults, TSimulationSubject simulationSubject)
      {
         //TODO
         //name has changed, needs to ensure unicity
         // ICommand renameCommand = null;
         // if (!string.Equals(moleculeToEdit.Name, queryResults.ProteinName))
         // {
         //    var newName = _containerTask.CreateUniqueName(simulationSubject, queryResults.ProteinName, true);
         //    renameCommand = RenameMolecule(moleculeToEdit, newName, simulationSubject);
         // }

         moleculeToEdit.QueryConfiguration = queryResults.QueryConfiguration;
         var editCommand = _simulationSubjectExpressionTask.EditMolecule(moleculeToEdit, queryResults, simulationSubject);
         // if (renameCommand == null)
         return editCommand;

         // return new PKSimMacroCommand(new[] {renameCommand, editCommand}).WithHistoryEntriesFrom(editCommand);
      }

      public ICommand RenameMolecule(IndividualMolecule molecule, string newName, TSimulationSubject simulationSubject)
      {
         return _simulationSubjectExpressionTask.RenameMolecule(molecule, newName, simulationSubject);
      }

      public ICommand AddMoleculeTo(TSimulationSubject simulationSubject, IndividualMolecule molecule, QueryExpressionResults queryResults)
      {
         var moleculeName = _containerTask.CreateUniqueName(simulationSubject, queryResults.ProteinName, true);
         //Required to rename here as we created a temp molecule earlier to create the structure;
         _simulationSubjectExpressionTask.RenameMolecule(molecule, moleculeName, simulationSubject);
         molecule.QueryConfiguration = queryResults.QueryConfiguration;

         var command = _simulationSubjectExpressionTask.AddMoleculeTo(molecule, simulationSubject, queryResults);
         setDefaultFor(molecule, simulationSubject, queryResults.ProteinName);
         return command;
      }

      private ICommand addMoleculeTo(IndividualMolecule molecule, TSimulationSubject simulationSubject)
      {
         setDefaultFor(molecule, simulationSubject, molecule.Name);
         return _simulationSubjectExpressionTask.AddMoleculeTo(molecule, simulationSubject);
      }

      public ICommand SetTransportDirection(TransporterExpressionContainer transporterContainer, TransportDirectionId transportDirection)
      {
         return new SetTransportDirectionCommand(transporterContainer, transportDirection, _executionContext).Run(_executionContext);
      }

      public ICommand SetExpressionLocalizationFor(IndividualProtein protein, Localization localization, TSimulationSubject simulationSubject)
      {
         return new SetExpressionLocalizationCommand(protein, localization, simulationSubject, _executionContext).Run(_executionContext);
      }

      public ICommand SetTransporterTypeFor(IndividualTransporter transporter, TransportType transportType)
      {
         return new SetTransportTypeCommand(transporter, transportType, _executionContext).Run(_executionContext);
      }

      private void setDefaultFor(IndividualMolecule molecule, TSimulationSubject simulationSubject, string moleculeName)
      {
         setDefaultSettingsForTransporter(molecule, simulationSubject, moleculeName);
         setDefaultOntogeny(molecule, simulationSubject, moleculeName);
         _moleculeParameterTask.SetDefaultMoleculeParameters(molecule, moleculeName);
      }

      private void setDefaultSettingsForTransporter(IndividualMolecule molecule, TSimulationSubject simulationSubject, string moleculeName)
      {
         if (!(molecule is IndividualTransporter transporter)) return;

         _transportContainerUpdater.SetDefaultSettingsForTransporter(simulationSubject, transporter, moleculeName);
      }

      private void setDefaultOntogeny(IndividualMolecule molecule, TSimulationSubject simulationSubject, string moleculeName)
      {
         var ontogeny = _ontogenyRepository.AllFor(simulationSubject.Species.Name).FindByName(moleculeName);
         if (ontogeny == null) return;
         _ontogenyTask.SetOntogenyForMolecule(molecule, ontogeny, simulationSubject);
      }

      private void updateTransporterDirections(IndividualMolecule molecule, TSimulationSubject simulationSubject, ExpressionProfile expressionProfile)
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
            if (individualContainer != null)
               individualContainer.TransportDirection = expressionProfileContainer.TransportDirection;
         }
      }

      private PathCache<TransporterExpressionContainer> allTransporterExpressionContainerFor(Individual individual, IndividualMolecule molecule)
      {
         var allTransporterExpressionContainer = individual.AllMoleculeContainersFor<TransporterExpressionContainer>(molecule);
         return new PathCache<TransporterExpressionContainer>(_entityPathResolver).For(allTransporterExpressionContainer);
      }

      private void updateGlobalMoleculeSettings(IndividualMolecule molecule, IndividualMolecule expressionProfileMolecule, TSimulationSubject simulationSubject)
      {
         molecule.Ontogeny = expressionProfileMolecule.Ontogeny;
         _ontogenyTask.SetOntogenyForMolecule(molecule, molecule.Ontogeny, simulationSubject);

         switch (molecule)
         {
            case IndividualProtein protein:
               var expressionProfileProtein = expressionProfileMolecule.DowncastTo<IndividualProtein>();
               protein.Localization = expressionProfileProtein.Localization;
               break;
            case IndividualTransporter transporter:
               var expressionProfileTransporter = expressionProfileMolecule.DowncastTo<IndividualTransporter>();
               transporter.TransportType = expressionProfileTransporter.TransportType;
               break;
         }
      }
   }
}