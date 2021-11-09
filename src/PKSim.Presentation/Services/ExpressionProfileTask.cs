using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.ExpressionProfiles;

namespace PKSim.Presentation.Services
{
   public class ExpressionProfileTask: BuildingBlockTask<ExpressionProfile>, IExpressionProfileTask
   {
      private readonly ISimulationSubjectExpressionTask<Individual> _individualExpressionTask;

      public ExpressionProfileTask(
         IExecutionContext executionContext, 
         IBuildingBlockTask buildingBlockTask, 
         IApplicationController applicationController,
         ISimulationSubjectExpressionTask<Individual> individualExpressionTask) :
         base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.ExpressionProfile)
      {
         _individualExpressionTask = individualExpressionTask;
      }

      public override ExpressionProfile AddToProject() => AddForMoleculeToProject<IndividualEnzyme>();


      public ExpressionProfile AddForMoleculeToProject<TMolecule>() where TMolecule : IndividualMolecule
      {
         return AddToProject<ICreateExpressionProfilePresenter>(x => x.Create<TMolecule>());
      }

      public ICommand UpdateMoleculeName(ExpressionProfile expressionProfile)
      {
         return _individualExpressionTask.RenameMolecule(expressionProfile.Molecule, expressionProfile.MoleculeName, expressionProfile.Individual);
      }
   }
}