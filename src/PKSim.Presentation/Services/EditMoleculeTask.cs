using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.Presentation.Services
{
   public interface IEditMoleculeTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      /// <summary>
      ///    Add a molecule of type <typeparamref name="TMolecule" /> to the given individual
      /// </summary>
      /// <typeparam name="TMolecule">Type of molecule to add. The molecule will be created depending on this type </typeparam>
      /// <param name="simulationSubject">Simulation subject where the molecule will be added</param>
      ICommand AddExpressionProfile<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule;

      /// <summary>
      ///    Remove the given molecule from the simulationSubject
      /// </summary>
      /// <param name="moleculeToRemove">Molecule to be removed</param>
      /// <param name="simulationSubject">Simulation subject containing the molecule to be removed</param>
      ICommand RemoveMoleculeFrom(IndividualMolecule moleculeToRemove, TSimulationSubject simulationSubject);
   }

   public class EditMoleculeTask<TSimulationSubject> : IEditMoleculeTask<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      private readonly IMoleculeExpressionTask<TSimulationSubject> _moleculeExpressionTask;
      private readonly IApplicationController _applicationController;

      public EditMoleculeTask(
         IMoleculeExpressionTask<TSimulationSubject> moleculeExpressionTask,
         IApplicationController applicationController)
      {
         _moleculeExpressionTask = moleculeExpressionTask;
         _applicationController = applicationController;
      }

      public ICommand AddExpressionProfile<TMolecule>(TSimulationSubject simulationSubject) where TMolecule : IndividualMolecule
      {
         //no database defined for the species. return the simple configuration
         return simpleMolecule<TMolecule>(simulationSubject);
      }

      public ICommand RemoveMoleculeFrom(IndividualMolecule moleculeToRemove, TSimulationSubject simulationSubject)
      {
         return _moleculeExpressionTask.RemoveExpressionProfileFor(moleculeToRemove, simulationSubject);
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
   }
}