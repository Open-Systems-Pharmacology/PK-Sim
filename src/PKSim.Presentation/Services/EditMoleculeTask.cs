using System;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Core;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.ProteinExpression;

namespace PKSim.Presentation.Services
{
   public interface IEditMoleculeTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      /// <summary>
      ///    Add a molecule of type <typeparamref name="TMolecule" /> to the given individual
      /// </summary>
      /// <typeparam name="TMolecule">Type of molecule to add. The molecule will be created depending on this type </typeparam>
      /// <param name="simulationSubject">Simulation subject where the molecule will be added</param>
      ICommand AddMoleculeTo<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule;

      /// <summary>
      ///    Edit the given molecule defined in the simulationSubject
      /// </summary>
      /// <param name="molecule">Edited molecule</param>
      /// <param name="simulationSubject">Simulation subject  containing the edited molecule</param>
      ICommand EditMolecule(IndividualMolecule molecule, TSimulationSubject simulationSubject);

      /// <summary>
      ///    Add a default molecule of type <typeparamref name="TMolecule" /> to the given <paramref name="simulationSubject" />
      ///    bypassing the expression
      ///    database
      /// </summary>
      /// <typeparam name="TMolecule">Type of molecule to add. The molecule will be created depending on this type </typeparam>
      /// <param name="simulationSubject">Simulation subject where the molecule will be added</param>
      ICommand AddDefaultMolecule<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule;

      /// <summary>
      ///    return true if a protein expression database was defined for the <paramref name="simulationSubject" />, otherwise
      ///    false
      /// </summary>
      bool CanQueryProteinExpressionsFor(TSimulationSubject simulationSubject);

      /// <summary>
      ///    Remove the given molecule from the simulationSubject
      /// </summary>
      /// <param name="moleculeToRemove">Molecule to be removed</param>
      /// <param name="simulationSubject">Simulation subject containing the molecule to be removed</param>
      ICommand RemoveMoleculeFrom(IndividualMolecule moleculeToRemove, TSimulationSubject simulationSubject);


      /// <summary>
      ///    Remove the given molecule from the simulationSubject
      /// </summary>
      /// <param name="molecule">Molecule to rename</param>
      /// <param name="newName">new name of the molecule</param>
      /// <param name="simulationSubject">Simulation subject containing the molecule to be renamed</param>
      ICommand RenameMolecule(IndividualMolecule molecule, string newName, TSimulationSubject simulationSubject);
   }

   public class EditMoleculeTask<TSimulationSubject> : IEditMoleculeTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      private readonly IMoleculeExpressionTask<TSimulationSubject> _moleculeExpressionTask;
      private readonly IMoleculeToQueryExpressionSettingsMapper _queryExpressionSettingsMapper;
      private readonly IExecutionContext _executionContext;
      private readonly IGeneExpressionsDatabasePathManager _geneExpressionsDatabasePathManager;
      private readonly IApplicationController _applicationController;
      private readonly IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;

      public EditMoleculeTask(
         IMoleculeExpressionTask<TSimulationSubject> moleculeExpressionTask,
         IMoleculeToQueryExpressionSettingsMapper queryExpressionSettingsMapper,
         IExecutionContext executionContext,
         IGeneExpressionsDatabasePathManager geneExpressionsDatabasePathManager,
         IApplicationController applicationController,
         IIndividualMoleculeFactoryResolver individualMoleculeFactoryResolver)
      {
         _moleculeExpressionTask = moleculeExpressionTask;
         _queryExpressionSettingsMapper = queryExpressionSettingsMapper;
         _geneExpressionsDatabasePathManager = geneExpressionsDatabasePathManager;
         _applicationController = applicationController;
         _individualMoleculeFactoryResolver = individualMoleculeFactoryResolver;
         _executionContext = executionContext;
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

             return _moleculeExpressionTask.EditMolecule(molecule, editedProtein, queryResults, simulationSubject);
         }
      }

      public bool CanQueryProteinExpressionsFor(TSimulationSubject simulationSubject)
      {
         return _geneExpressionsDatabasePathManager.HasDatabaseFor(simulationSubject.Species);
      }

      public ICommand RemoveMoleculeFrom(IndividualMolecule moleculeToRemove, TSimulationSubject simulationSubject)
      {
         return _moleculeExpressionTask.RemoveMoleculeFrom(moleculeToRemove, simulationSubject);
      }

      public ICommand RenameMolecule(IndividualMolecule molecule, string newName, TSimulationSubject simulationSubject)
      {
         return _moleculeExpressionTask.RenameMolecule(molecule, newName, simulationSubject);
      }

      private ICommand simpleMolecule<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule
      {
         using (var presenter = _applicationController.Start<ISimpleMoleculePresenter>())
         {
            bool proteinCreated = presenter.CreateMoleculeFor<TMolecule>(simulationSubject);
            if (!proteinCreated)
               return new PKSimEmptyCommand();

            return _moleculeExpressionTask.AddMoleculeTo<TMolecule>(simulationSubject, presenter.MoleculeName);
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
            if (!presenter.Start())
            {
               //needs to remove the molecule that was added previously
               simulationSubject.RemoveMolecule(newMolecule);
               return new PKSimEmptyCommand();
            }

            var queryResults = presenter.GetQueryResults();
            return _moleculeExpressionTask.AddMoleculeTo(simulationSubject, newMolecule, queryResults);
         }
      }
   }
}