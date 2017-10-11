using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.ProteinExpression;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public class MoleculeExpressionTask<TSimulationSubject> : IMoleculeExpressionTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      private readonly IApplicationController _applicationController;
      private readonly IContainerTask _containerTask;
      private readonly IExecutionContext _executionContext;
      private readonly IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;
      private readonly IProteinExpressionsDatabasePathManager _proteinExpressionsDatabasePathManager;
      private readonly IMoleculeToQueryExpressionSettingsMapper _queryExpressionSettingsMapper;
      private readonly IOntogenyRepository _ontogenyRepository;
      private readonly ITransportContainerUpdater _transportContainerUpdater;
      private readonly IMoleculeParameterRepository _moleculeParameterRepository;
      private readonly ISimulationSubjectExpressionTask<TSimulationSubject> _simulationSubjectExpressionTask;
      private readonly IOntogenyTask<TSimulationSubject> _ontogenyTask;

      public MoleculeExpressionTask(IApplicationController applicationController, IExecutionContext executionContext,
         IIndividualMoleculeFactoryResolver individualMoleculeFactoryResolver,
         IMoleculeToQueryExpressionSettingsMapper queryExpressionSettingsMapper,
         IContainerTask containerTask,
         IProteinExpressionsDatabasePathManager proteinExpressionsDatabasePathManager,
         IOntogenyRepository ontogenyRepository,
         ITransportContainerUpdater transportContainerUpdater,
         IMoleculeParameterRepository moleculeParameterRepository,
         ISimulationSubjectExpressionTask<TSimulationSubject> simulationSubjectExpressionTask,
         IOntogenyTask<TSimulationSubject> ontogenyTask)
      {
         _applicationController = applicationController;
         _executionContext = executionContext;
         _individualMoleculeFactoryResolver = individualMoleculeFactoryResolver;
         _queryExpressionSettingsMapper = queryExpressionSettingsMapper;
         _containerTask = containerTask;
         _proteinExpressionsDatabasePathManager = proteinExpressionsDatabasePathManager;
         _ontogenyRepository = ontogenyRepository;
         _transportContainerUpdater = transportContainerUpdater;
         _moleculeParameterRepository = moleculeParameterRepository;
         _simulationSubjectExpressionTask = simulationSubjectExpressionTask;
         _ontogenyTask = ontogenyTask;
      }

      public ICommand AddMoleculeTo<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule
      {
         var moleculeFactory = _individualMoleculeFactoryResolver.FactoryFor<TMolecule>();
         var newMolecule = moleculeFactory.CreateFor(simulationSubject);

         //databse was defined for this species
         if (CanQueryProteinExpressionsFor(simulationSubject))
            return proteinFromQuery(simulationSubject, newMolecule);

         //no database defined for the species. return the simple configuration
         return simpleMolecule<TMolecule>(simulationSubject, newMolecule);
      }

      public ICommand AddDefaultMolecule<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule
      {
         var moleculeFactory = _individualMoleculeFactoryResolver.FactoryFor<TMolecule>();
         return simpleMolecule<TMolecule>(simulationSubject, moleculeFactory.CreateFor(simulationSubject));
      }

      public ICommand EditMolecule(IndividualMolecule molecule, TSimulationSubject simulationSubject)
      {
         using (_proteinExpressionsDatabasePathManager.ConnectToDatabaseFor(simulationSubject.Species))
         using (var presenter = _applicationController.Start<IProteinExpressionsPresenter>())
         {
            presenter.InitializeSettings(_queryExpressionSettingsMapper.MapFrom(molecule));
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

      public bool CanQueryProteinExpressionsFor(TSimulationSubject simulationSubject)
      {
         return _proteinExpressionsDatabasePathManager.HasDatabaseFor(simulationSubject.Species);
      }

      public ICommand SetRelativeExpressionFor(IndividualMolecule molecule, string moleculeContainerName, double value)
      {
         return new SetRelativeExpressionAndNormalizeCommand(molecule, moleculeContainerName, value).Run(_executionContext);
      }

      public ICommand SetRelativeExpressionInSimulationFor(IParameter parameter, double value)
      {
         return new SetRelativeExpressionInSimulationAndNormalizedCommand(parameter, value).Run(_executionContext);
      }

      public ICommand RemoveMoleculeFrom(IndividualMolecule molecule, TSimulationSubject simulationSubject)
      {
         return _simulationSubjectExpressionTask.RemoveMoleculeFrom(molecule, simulationSubject);
      }

      private ICommand simpleMolecule<TMolecule>(TSimulationSubject simulationSubject, IndividualMolecule molecule) where TMolecule : IndividualMolecule
      {
         using (var presenter = _applicationController.Start<ISimpleMoleculePresenter>())
         {
            bool proteinCreated = presenter.CreateMoleculeFor<TMolecule>(simulationSubject);
            if (!proteinCreated)
               return new PKSimEmptyCommand();

            molecule.Name = presenter.MoleculeName;
            return addMoleculeTo(molecule, simulationSubject);
         }
      }

      private ICommand proteinFromQuery(TSimulationSubject simulationSubject, IndividualMolecule newMolecule)
      {
         using (_proteinExpressionsDatabasePathManager.ConnectToDatabaseFor(simulationSubject.Species))
         using (var presenter = _applicationController.Start<IProteinExpressionsPresenter>())
         {
            presenter.InitializeSettings(_queryExpressionSettingsMapper.MapFrom(newMolecule));
            presenter.Title = PKSimConstants.UI.AddProteinExpression(_executionContext.TypeFor(newMolecule));
            bool proteinCreated = presenter.Start();
            if (!proteinCreated)
               return new PKSimEmptyCommand();

            var queryResults = presenter.GetQueryResults();
            newMolecule.Name = _containerTask.CreateUniqueName(simulationSubject, queryResults.ProteinName, true);
            newMolecule.QueryConfiguration = queryResults.QueryConfiguration;
            return addMoleculeTo(newMolecule, simulationSubject, queryResults);
         }
      }

      private ICommand editMolecule<TMolecule>(TMolecule moleculeToEdit, TMolecule editedMolecule, QueryExpressionResults queryResults, TSimulationSubject simulationSubject)
         where TMolecule : IndividualMolecule
      {
         return _simulationSubjectExpressionTask.EditMolecule(moleculeToEdit, editedMolecule, queryResults, simulationSubject);
      }

      private ICommand addMoleculeTo<TMolecule>(TMolecule molecule, TSimulationSubject simulationSubject, QueryExpressionResults queryExpressionResults) where TMolecule : IndividualMolecule
      {
         var command = _simulationSubjectExpressionTask.AddMoleculeTo(molecule, simulationSubject, queryExpressionResults);
         setDefaultFor(molecule, simulationSubject, queryExpressionResults.ProteinName);
         return command;
      }

      private ICommand addMoleculeTo<TMolecule>(TMolecule molecule, TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule
      {
         var command = _simulationSubjectExpressionTask.AddMoleculeTo(molecule, simulationSubject);
         setDefaultFor(molecule, simulationSubject, molecule.Name);
         return command;
      }

      public ICommand SetMembraneLocationFor(TransporterExpressionContainer transporterContainer, TransportType transportType, MembraneLocation membraneLocation)
      {
         return new SetMembraneTypeCommand(transporterContainer, transportType, membraneLocation, _executionContext).Run(_executionContext);
      }

      public ICommand SetTissueLocationFor(IndividualProtein protein, TissueLocation tissueLocation)
      {
         return new SetTissueLocationCommand(protein, tissueLocation, _executionContext).Run(_executionContext);
      }

      public ICommand SetTransporterTypeFor(IndividualTransporter transporter, TransportType transportType)
      {
         return new SetTransportTypeInAllContainerCommand(transporter, transportType, _executionContext).Run(_executionContext);
      }

      public ICommand SetMembraneLocationFor(IndividualProtein protein, MembraneLocation membraneLocation)
      {
         return new SetProteinMembraneLocationCommand(protein, membraneLocation, _executionContext).Run(_executionContext);
      }

      public ICommand SetIntracellularVascularEndoLocation(IndividualProtein protein, IntracellularVascularEndoLocation vascularEndoLocation)
      {
         return new SetProteinIntracellularVascularEndoLocationCommand(protein, vascularEndoLocation, _executionContext).Run(_executionContext);
      }

      private void setDefaultFor(IndividualMolecule molecule, TSimulationSubject simulationSubject, string moleculeName)
      {
         setDefaultSettingsForTransporter(molecule, simulationSubject, moleculeName);
         setDefaultOntogeny(molecule, simulationSubject, moleculeName);
         setDefaulParameters(molecule, moleculeName);
      }

      private void setDefaulParameters(IndividualMolecule molecule, string moleculeName)
      {
         setDefaultParameter(moleculeName, molecule.ReferenceConcentration);
         setDefaultParameter(moleculeName, molecule.HalfLifeLiver);
         setDefaultParameter(moleculeName, molecule.HalfLifeIntestine);
      }

      private void setDefaultParameter(string moleculeName, IParameter parameter)
      {
         var value = _moleculeParameterRepository.ParameterValueFor(moleculeName, parameter.Name, parameter.DefaultValue);
         parameter.DefaultValue = value;
         parameter.Value = value;
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