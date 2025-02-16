using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.Presentation.Services
{
   public class IndividualTask : BuildingBlockTask<Individual>, IIndividualTask
   {
      public IndividualTask(IExecutionContext executionContext, IBuildingBlockTask buildingBlockTask, IApplicationController applicationController) :
         base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.Individual)
      {
      }

      public override Individual AddToProject() => AddToProject<ICreateIndividualPresenter>();

      public void ScaleIndividual(Individual individualToScale)
      {
         _buildingBlockTask.Load(individualToScale);

         using (var presenter = _applicationController.Start<IScaleIndividualPresenter>())
         {
            var scaleCommand = presenter.ScaleIndividual(individualToScale);

            //User cancel action. return
            if (scaleCommand.IsEmpty()) return;

            var scaledIndividual = presenter.Individual;
            var overallCommand = new PKSimMacroCommand
            {
               CommandType = PKSimConstants.Command.CommandTypeScale,
               ObjectType = PKSimConstants.ObjectTypes.Individual,
               Description = PKSimConstants.Command.ScaleIndividualDescription(individualToScale.Name, scaledIndividual.Name),
               BuildingBlockName = scaledIndividual.Name,
               BuildingBlockType = PKSimConstants.ObjectTypes.Individual
            };

            //Do not add to history as the add action should be part of the overall command
            var addToProjectCommand = _buildingBlockTask.AddToProject(scaledIndividual, addToHistory: false);
            overallCommand.Add(addToProjectCommand);

            //these needs to be done afterwards in order to be able to undo the scaling action
            var macroCommand = scaleCommand as IPKSimMacroCommand;
            macroCommand?.All().Each(overallCommand.Add);
            overallCommand.ReplaceNameTemplateWithName(scaledIndividual.Name);
            overallCommand.ReplaceTypeTemplateWithType(PKSimConstants.ObjectTypes.Individual);

            _buildingBlockTask.AddCommandToHistory(overallCommand);
         }
      }

      protected override void SaveAsTemplate(IReadOnlyList<Individual> individuals, TemplateDatabaseType templateDatabaseType)
      {
         //We need to save expression profiles for individuals
         var cache = new Cache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>>();
         individuals.Each(x => cache[x] = x.AllExpressionProfiles());
         _buildingBlockTask.SaveAsTemplate(cache, templateDatabaseType);
      }
   }
}