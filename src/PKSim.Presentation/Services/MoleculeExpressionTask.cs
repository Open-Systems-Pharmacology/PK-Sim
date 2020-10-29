using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Core;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.ProteinExpression;

namespace PKSim.Presentation.Services
{
   public class MoleculeExpressionTask<TSimulationSubject> : IMoleculeExpressionTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      private readonly IApplicationController _applicationController;
      private readonly IContainerTask _containerTask;
      private readonly IExecutionContext _executionContext;
      private readonly IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;
      private readonly IGeneExpressionsDatabasePathManager _geneExpressionsDatabasePathManager;
      private readonly IMoleculeToQueryExpressionSettingsMapper _queryExpressionSettingsMapper;
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly ITransportContainerUpdater _transportContainerUpdater;
      private readonly ISimulationSubjectExpressionTask<TSimulationSubject> _simulationSubjectExpressionTask;
      private readonly IOntogenyTask<TSimulationSubject> _ontogenyTask;
      private readonly IMoleculeParameterTask _moleculeParameterTask;

      public MoleculeExpressionTask(IApplicationController applicationController, IExecutionContext executionContext,
         IIndividualMoleculeFactoryResolver individualMoleculeFactoryResolver,
         IMoleculeToQueryExpressionSettingsMapper queryExpressionSettingsMapper,
         IContainerTask containerTask,
         IGeneExpressionsDatabasePathManager geneExpressionsDatabasePathManager,
         IOntogenyRepository ontogenyRepository,
         ITransportContainerUpdater transportContainerUpdater,
         ISimulationSubjectExpressionTask<TSimulationSubject> simulationSubjectExpressionTask,
         IOntogenyTask<TSimulationSubject> ontogenyTask,
         IMoleculeParameterTask moleculeParameterTask)
      {
         _applicationController = applicationController;
         _executionContext = executionContext;
         _individualMoleculeFactoryResolver = individualMoleculeFactoryResolver;
         _queryExpressionSettingsMapper = queryExpressionSettingsMapper;
         _containerTask = containerTask;
         _geneExpressionsDatabasePathManager = geneExpressionsDatabasePathManager;
         _ontogenyRepository = ontogenyRepository;
         _transportContainerUpdater = transportContainerUpdater;
         _simulationSubjectExpressionTask = simulationSubjectExpressionTask;
         _ontogenyTask = ontogenyTask;
         _moleculeParameterTask = moleculeParameterTask;
      }

      public ICommand AddMoleculeTo<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule
      {
         //database was defined for this species
         if (CanQueryProteinExpressionsFor(simulationSubject))
            return proteinFromQuery<TMolecule>(simulationSubject);

         //no database defined for the species. return the simple configuration
         return simpleMolecule<TMolecule>(simulationSubject);
      }


      public ICommand AddDefaultMolecule<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule
      {
         return simpleMolecule<TMolecule>(simulationSubject);
      }

      public ICommand EditMolecule(IndividualMolecule molecule, TSimulationSubject simulationSubject)
      {
         using (_geneExpressionsDatabasePathManager.ConnectToDatabaseFor(simulationSubject.Species))
         using (var presenter = _applicationController.Start<IProteinExpressionsPresenter>())
         {
            presenter.InitializeSettings(_queryExpressionSettingsMapper.MapFrom(molecule, simulationSubject));
            presenter.Title = PKSimConstants.UI.EditProteinExpression;
            bool proteinEdited = presenter.Start();
            if (!proteinEdited)
               return new PKSimEmptyCommand();

            var editedProtein = _executionContext.Clone(molecule);
            var queryResults = presenter.GetQueryResults();

            //name has changed, needs to ensure unicity
            if (!string.Equals(editedProtein.Name, queryResults.ProteinName))
               editedProtein.Name = _containerTask.CreateUniqueName(simulationSubject, queryResults.ProteinName, true);

            editedProtein.QueryConfiguration = queryResults.QueryConfiguration;
            return editMolecule(molecule, editedProtein, queryResults, simulationSubject);
         }
      }

      private ICommand proteinFromQuery<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule
      {
         using (_geneExpressionsDatabasePathManager.ConnectToDatabaseFor(simulationSubject.Species))
         using (var presenter = _applicationController.Start<IProteinExpressionsPresenter>())
         {
            var moleculeFactory = _individualMoleculeFactoryResolver.FactoryFor<TMolecule>();
            var newMolecule = moleculeFactory.AddMoleculeTo(simulationSubject, "%TEMP%");
            presenter.InitializeSettings(_queryExpressionSettingsMapper.MapFrom(newMolecule, simulationSubject));
            presenter.Title = PKSimConstants.UI.AddProteinExpression(_executionContext.TypeFor(newMolecule));
            bool proteinCreated = presenter.Start();
            if (!proteinCreated)
            {
               //needs to remove the molecule that was added previously
               simulationSubject.RemoveMolecule(newMolecule);
               return new PKSimEmptyCommand();
            }

            var queryResults = presenter.GetQueryResults();
            var moleculeName = _containerTask.CreateUniqueName(simulationSubject, queryResults.ProteinName, true);
            //Required to rename here as we created a temp molecule earlier to create the structure;
            _simulationSubjectExpressionTask.RenameMolecule(newMolecule, simulationSubject, moleculeName);
            newMolecule.QueryConfiguration = queryResults.QueryConfiguration;
            return addMoleculeTo(newMolecule, simulationSubject, queryResults);
         }
      }

      public bool CanQueryProteinExpressionsFor(TSimulationSubject simulationSubject)
      {
         return _geneExpressionsDatabasePathManager.HasDatabaseFor(simulationSubject.Species);
      }

      public ICommand SetRelativeExpressionFor(IParameter relativeExpressionParameter, double value)
      {
         return new SetRelativeExpressionCommand(relativeExpressionParameter, value).Run(_executionContext);
      }

      public ICommand RemoveMoleculeFrom(IndividualMolecule molecule, TSimulationSubject simulationSubject)
      {
         return _simulationSubjectExpressionTask.RemoveMoleculeFrom(molecule, simulationSubject);
      }

      private ICommand simpleMolecule<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule
      {
         using (var presenter = _applicationController.Start<ISimpleMoleculePresenter>())
         {
            bool proteinCreated = presenter.CreateMoleculeFor<TMolecule>(simulationSubject);
            if (!proteinCreated)
               return new PKSimEmptyCommand();

            return AddMoleculeTo<TMolecule>(simulationSubject, presenter.MoleculeName);
         }
      }


      public ICommand AddMoleculeTo<TMolecule>(TSimulationSubject simulationSubject, string moleculeName) where TMolecule : IndividualMolecule
      {
         var moleculeFactory = _individualMoleculeFactoryResolver.FactoryFor<TMolecule>();
         var molecule = moleculeFactory.AddMoleculeTo(simulationSubject, moleculeName);
         return addMoleculeTo(molecule, simulationSubject);
      }

      private ICommand editMolecule<TMolecule>(TMolecule moleculeToEdit, TMolecule editedMolecule, QueryExpressionResults queryResults,
         TSimulationSubject simulationSubject)
         where TMolecule : IndividualMolecule
      {
         return _simulationSubjectExpressionTask.EditMolecule(moleculeToEdit, editedMolecule, queryResults, simulationSubject);
      }

      private ICommand addMoleculeTo(IndividualMolecule molecule, TSimulationSubject simulationSubject, QueryExpressionResults queryExpressionResults)
      {
         var command = _simulationSubjectExpressionTask.AddMoleculeTo(molecule, simulationSubject, queryExpressionResults);
         setDefaultFor(molecule, simulationSubject, queryExpressionResults.ProteinName);
         return command;
      }

      private ICommand addMoleculeTo(IndividualMolecule molecule, TSimulationSubject simulationSubject)
      {
         var command = _simulationSubjectExpressionTask.AddMoleculeTo(molecule, simulationSubject);
         setDefaultFor(molecule, simulationSubject, molecule.Name);
         return command;
      }

      public ICommand SetMembraneLocationFor(TransporterExpressionContainer transporterContainer, TransportType transportType,
         MembraneLocation membraneLocation)
      {
         return new SetMembraneTypeCommand(transporterContainer, transportType, membraneLocation, _executionContext).Run(_executionContext);
      }

      public ICommand SetExpressionLocalizationFor(IndividualProtein protein, Localization localization, TSimulationSubject simulationSubject)
      {
         return new SetExpressionLocalizationCommand(protein, localization, simulationSubject, _executionContext).Run(_executionContext);
      }

      public ICommand SetTransporterTypeFor(IndividualTransporter transporter, TransportType transportType)
      {
         return new SetTransportTypeInAllContainerCommand(transporter, transportType, _executionContext).Run(_executionContext);
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

         _transportContainerUpdater.SetDefaultSettingsForTransporter(transporter, simulationSubject.Species.Name, moleculeName);
      }

      private void setDefaultOntogeny(IndividualMolecule molecule, TSimulationSubject simulationSubject, string moleculeName)
      {
         var ontogeny = _ontogenyRepository.AllFor(simulationSubject.Species.Name).FindByName(moleculeName);
         if (ontogeny == null) return;
         _ontogenyTask.SetOntogenyForMolecule(molecule, ontogeny, simulationSubject);
      }
   }
}