using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Core;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Individuals;
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
      ///    Edit the given molecule defined in the simulationSubject
      /// </summary>
      /// <param name="molecule">Edited molecule</param>
      /// <param name="simulationSubject">Simulation subject  containing the edited molecule</param>
      /// <param name="moleculeName">Predefined name for the query</param>
      ICommand EditMolecule(IndividualMolecule molecule, TSimulationSubject simulationSubject, string moleculeName);

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
         //no database defined for the species. return the simple configuration
         return simpleMolecule<TMolecule>(simulationSubject);
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
         using (var presenter = _applicationController.Start<IExpressionProfileSelectionPresenter>())
         {
            var expressionProfile = presenter.SelectExpressionProfile<TMolecule>(simulationSubject);
            if (expressionProfile == null)
               return new PKSimEmptyCommand();

            return _moleculeExpressionTask.AddExpressionProfile(simulationSubject, expressionProfile);
         }
      }

      public ICommand EditMolecule(IndividualMolecule molecule, TSimulationSubject simulationSubject, string moleculeName)
      {
         using (_geneExpressionsDatabasePathManager.ConnectToDatabaseFor(simulationSubject.Species))
         using (var presenter = _applicationController.Start<IProteinExpressionsPresenter>())
         {
            presenter.InitializeSettings(_queryExpressionSettingsMapper.MapFrom(molecule, simulationSubject, moleculeName));
            presenter.Title = PKSimConstants.UI.EditProteinExpression;
            var success = presenter.Start();
            if (!success)
               return new PKSimEmptyCommand();

            var queryResults = presenter.GetQueryResults();

            return _moleculeExpressionTask.EditMolecule(molecule, queryResults, simulationSubject);
         }
      }

      public ICommand EditMolecule(IndividualMolecule molecule, TSimulationSubject simulationSubject)
      {
         using (_geneExpressionsDatabasePathManager.ConnectToDatabaseFor(simulationSubject.Species))
         using (var presenter = _applicationController.Start<IProteinExpressionsPresenter>())
         {
            presenter.InitializeSettings(_queryExpressionSettingsMapper.MapFrom(molecule, simulationSubject, molecule.Name));
            presenter.Title = PKSimConstants.UI.EditProteinExpression;
            var success = presenter.Start();
            if (!success)
               return new PKSimEmptyCommand();

            var queryResults = presenter.GetQueryResults();

            return _moleculeExpressionTask.EditMolecule(molecule, queryResults, simulationSubject);
         }
      }

      //
      // private ICommand proteinFromQuery<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule
      // {
      //    using (_geneExpressionsDatabasePathManager.ConnectToDatabaseFor(simulationSubject.Species))
      //    using (var presenter = _applicationController.Start<IProteinExpressionsPresenter>())
      //    {
      //       var moleculeFactory = _individualMoleculeFactoryResolver.FactoryFor<TMolecule>();
      //       var newMolecule = moleculeFactory.AddMoleculeTo(simulationSubject, "%TEMP%");
      //       presenter.InitializeSettings(_queryExpressionSettingsMapper.MapFrom(newMolecule, simulationSubject, ""));
      //       presenter.Title = PKSimConstants.UI.AddProteinExpression(_executionContext.TypeFor(newMolecule));
      //       if (!presenter.Start())
      //       {
      //          //needs to remove the molecule that was added previously
      //          simulationSubject.RemoveMolecule(newMolecule);
      //          return new PKSimEmptyCommand();
      //       }
      //
      //       var queryResults = presenter.GetQueryResults();
      //       return _moleculeExpressionTask.AddMoleculeTo(simulationSubject, newMolecule, queryResults);
      //    }
      // }
   }
}